variable "aws_access_key_id" {
  description = "AWS Access Key ID"
  type        = string
  sensitive   = true
}

variable "aws_secret_access_key" {
  description = "AWS Secret Access Key"
  type        = string
  sensitive   = true
}

variable "database_table_name" {
  description = "Database Table Name"
  type        = string
}

variable "aws_region" {
  description = "AWS Region"
  type        = string
}

variable "sqs_queue_url" {
  description = "SQS Queue URL"
  type        = string
}

variable "aws_s3_bucket_name" {
  description = "S3 Bucket Name"
  type        = string
}
