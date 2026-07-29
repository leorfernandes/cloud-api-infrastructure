resource "aws_security_group" "postgres_sg" {
  name        = "inventoryapi-postgres-sg"
  description = "Allow Postgres access for InventoryApi"

  ingress {
    description = "Postgres"
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"] # NOTE: wide open for demo purposes only
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Project = "InventoryApi"
  }
}

resource "aws_db_instance" "postgres" {
  identifier        = "inventoryapi-db"
  engine            = "postgres"
  engine_version    = "16"
  instance_class    = "db.t3.micro"
  allocated_storage = 20

  db_name  = var.db_name
  username = var.db_username
  password = var.db_password

  vpc_security_group_ids = [aws_security_group.postgres_sg.id]

  publicly_accessible = false
  skip_final_snapshot = true

  tags = {
    Project = "InventoryApi"
  }
}