#! /bin/bash
 
docker-compose up -d
sleep 15

echo "Elastic is up and running"

# Seed data
docker build -f ./seed-data/Dockerfile -t seed-elastic-data ./seed-data
docker run seed-elastic-data
