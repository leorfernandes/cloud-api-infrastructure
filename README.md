# InventoryApi
 
A small inventory-tracking REST API built to demonstrate a full cloud/DevOps
toolchain: a real relational domain, containerization, orchestration, and
automated testing.
 
## Stack
 
- **API:** C# / ASP.NET Core (.NET 10), EF Core, PostgreSQL
- **Containerization:** Docker (multi-stage build)
- **Orchestration:** Kubernetes (Deployments, Services, ConfigMap, Secret, PVC) — tested locally on minikube
- **Testing:** xUnit integration tests via `WebApplicationFactory`, isolated test database, ~87% line coverage on hand-written code (excludes generated migrations/OpenAPI code)
- **CI/CD:** GitHub Actions — planned next (build, test, and image build on every push)
## Domain
 
`User` → owns → `Item` → belongs to → `Category`
 
A small but real relational schema (foreign keys, joins) rather than a flat single-table demo.
 
## Project structure
 
```
InventoryApi/            API project (Models, Controllers, Dtos, Data)
InventoryApi.Tests/      xUnit integration tests
k8s/                      Kubernetes manifests
Dockerfile                Multi-stage build for the API
docker-compose.yml        API + Postgres, networked, for local dev
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

## Security notes

- `k8s/postgres-secret.yaml` and connection strings use plaintext placeholder
  credentials (`postgres`/`postgres`) intentionally — this is a local-only
  dev/demo setup with no publicly reachable database behind it. In a real
  deployment, these would come from a secrets manager (e.g. AWS Secrets
  Manager, Kubernetes Secrets sourced from a vault) rather than being
  committed to source control.
