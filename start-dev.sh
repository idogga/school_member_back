#!/bin/bash
docker-compose -f docker-compose-dev.yml down
docker-compose -f docker-compose-dev.yml up -d
sleep 60
docker-compose -f docker-compose-dev.yml exec sentry sentry upgrade
