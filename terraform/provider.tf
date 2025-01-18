terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "5.62.0"
    }
  }

  backend "s3" {
    bucket = "github-pipe"
    key    = "hackathon-api/terraform.tfstate"
    region = "sa-east-1"
  }
}

provider "aws" {
  region = "us-east-1"

  default_tags {
    tags = {
      Project = "Infra para projeto processa vídeo"
      Owner   = "Grupo 28"
    }
  }
}