#!/bin/bash
echo ECS_CLUSTER=my-ecs-cluster >> /etc/ecs/ecs.config
yum install -y ecs-init
systemctl enable --now ecs