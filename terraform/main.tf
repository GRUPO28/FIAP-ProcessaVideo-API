resource "aws_instance" "processavideo_ec2" {
     ami           = "ami-04e914639d0cca79a"
     instance_type = "t2.micro"
  
}