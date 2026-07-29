# Cloud API Infrastructure
 
A small inventory-tracking REST API used as a vehicle to demonstrate a full
cloud/DevOps toolchain: containerization, orchestration, CI/CD, and
infrastructure as code across two cloud providers.
 
## Stack
 
- **API:** C# / ASP.NET Core (.NET 10), EF Core, PostgreSQL
- **Containerization:** Docker (multi-stage build)
- **Orchestration:** Kubernetes (Deployments, Services, ConfigMap, Secret, PVC) — tested locally on minikube
- **Testing:** xUnit integration tests via `WebApplicationFactory`, isolated test database, ~87% line coverage on hand-written code (excludes generated migrations/OpenAPI code)
- **CI/CD:** GitHub Actions — test (Postgres service container) → build & push image to GHCR → Python smoke test against the running container
- **Infrastructure as Code:** Terraform
  - **AWS** (validated with real `terraform plan`, never applied): RDS Postgres, ECR, EC2 (simple single-instance path), and a full EKS setup (VPC, IAM roles, cluster, node group)
  - **Azure** (validated with `terraform validate`, no live account configured): Resource Group, PostgreSQL Flexible Server, ACR, and a VM with full networking (VNet, Subnet, NIC, NSG)
## Domain
 
`User` → owns → `Item` → belongs to → `Category`
 
A small but real relational schema (foreign keys, joins) rather than a flat single-table demo.
 
## Project structure
 
```
InventoryApi/            API project (Models, Controllers, Dtos, Data)
InventoryApi.Tests/      xUnit integration tests
k8s/                      Kubernetes manifests
terraform/aws/            AWS Terraform (RDS, ECR, EC2, EKS)
terraform/azure/          Azure Terraform (Resource Group, Flexible Server, ACR, VM)
scripts/                  Python smoke-test script
Dockerfile                Multi-stage build for the API
docker-compose.yml        API + Postgres, networked, for local dev
.github/workflows/ci.yml  CI/CD pipeline
```
 
## Running it locally
 
**Option A — Docker Compose (simplest):**
```bash
docker compose up -d
```
API available at `http://localhost:8080/swagger`.
 
**Option B — Kubernetes (minikube):**
```bash
minikube start
minikube image load inventoryapi
kubectl apply -f k8s/
minikube service inventoryapi
```
 
**Running the tests:**
```bash
dotnet test
```
Requires a local Postgres reachable at `localhost:5432` (see `docker-compose.yml`); tests run against a separate `inventorydb_test` database, created automatically.
 
**Terraform:**
```bash
cd terraform/aws     # or terraform/azure
terraform init
terraform plan        # AWS only — requires AWS credentials, never applied
terraform validate    # Azure — no live account needed
```
 
## CI/CD pipeline
 
On every push: xUnit tests run against a real Postgres service container →
if they pass, the Docker image is built and pushed to GHCR, tagged by
commit SHA → a Python script pulls that exact image, runs it, applies
migrations, and hits real endpoints to confirm it actually works.
 
## Security notes
 
- `k8s/postgres-secret.yaml` and local connection strings use plaintext
  placeholder credentials (`postgres`/`postgres`) intentionally — this is a
  local-only dev/demo setup with no publicly reachable database behind it.
  In a real deployment, these would come from a secrets manager (e.g. AWS
  Secrets Manager, a Kubernetes Secret sourced from a vault) rather than
  being committed to source control.
- The Azure VM's `admin_ssh_key` uses a throwaway local key pair generated
  solely to satisfy `terraform validate`'s format checks — it's gitignored
  and was never intended to authenticate anything real.
## Notes on scope
 
- AWS Terraform was validated with real `terraform plan` against an actual
  AWS account (read-only IAM permissions), but never `apply`'d — no AWS
  resources were ever created, so no cost was incurred.
- A live deployment (e.g. a small EC2 instance actually running the app)
  was deliberately scoped out to avoid real cloud costs.