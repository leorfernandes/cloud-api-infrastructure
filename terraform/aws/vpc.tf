resource "aws_vpc" "main" {
  cidr_block = "10.0.0.0/16"

  tags = {
    Project = "InventoryApi"
    Name    = "inventoryapi-vpc"
  }
}

resource "aws_subnet" "subnet_a" {
  vpc_id            = aws_vpc.main.id
  cidr_block        = "10.0.1.0/24"
  availability_zone = "${var.aws_region}a"

  tags = {
    Project = "InventoryApi"
  }
}

resource "aws_subnet" "subnet_b" {
  vpc_id            = aws_vpc.main.id
  cidr_block        = "10.0.2.0/24"
  availability_zone = "${var.aws_region}b"

  tags = {
    Project = "InventoryApi"
  }
}