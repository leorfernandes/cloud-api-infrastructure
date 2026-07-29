resource "aws_ecr_repository" "inventoryapi" {
  name                 = "inventoryapi"
  image_tag_mutability = "IMMUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Project = "InventoryApi"
  }
}

# Keeps the repository from accumulating unlimited untagged images over time
resource "aws_ecr_lifecycle_policy" "inventoryapi_cleanup" {
  repository = aws_ecr_repository.inventoryapi.name

  policy = jsonencode({
    rules = [
      {
        rulePriority = 1
        description  = "Expire untagged images after 14 days"
        selection = {
          tagStatus   = "untagged"
          countType   = "sinceImagePushed"
          countUnit   = "days"
          countNumber = 14
        }
        action = {
          type = "expire"
        }
      }
    ]
  })
}