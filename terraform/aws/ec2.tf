# Security group allowing inbound HTTP (8080, the API's port) and SSH (22)
resource "aws_security_group" "api_sg" {
  name        = "inventoryapi-ec2-sg"
  description = "Allow API and SSH access for InventoryApi EC2 instance"

  ingress {
    description = "API"
    from_port   = 8080
    to_port     = 8080
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"] # NOTE: wide open for demo purposes only
  }

  ingress {
    description = "SSH"
    from_port   = 22
    to_port     = 22
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

# Latest Amazon Linux 2023 AMI, resolved automatically rather than hardcoded
data "aws_ami" "amazon_linux" {
  most_recent = true
  owners      = ["amazon"]

  filter {
    name   = "name"
    values = ["al2023-ami-*-x86_64"]
  }

  filter {
    name   = "virtualization-type"
    values = ["hvm"]
  }
}

resource "aws_instance" "api_server" {
  ami                    = data.aws_ami.amazon_linux.id
  instance_type          = "t3.micro"
  vpc_security_group_ids = [aws_security_group.api_sg.id]

  # Installs Docker on first boot, so the instance is ready to run
  # the API image pulled from the ECR repo created in ecr.tf.
  user_data = <<-EOF
              #!/bin/bash
              dnf update -y
              dnf install -y docker
              systemctl enable docker
              systemctl start docker
              EOF

  tags = {
    Project = "InventoryApi"
    Name    = "inventoryapi-ec2"
  }
}