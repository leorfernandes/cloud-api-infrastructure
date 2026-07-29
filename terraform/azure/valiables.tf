variable "db_username" {
  description = "Master username for the database"
  type        = string
  default     = "postgres"
}

variable "db_password" {
  description = "Master password for the database (no default — must be supplied)"
  type        = string
  sensitive   = true
}