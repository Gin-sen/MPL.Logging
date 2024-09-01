#!/bin/bash

body="{\
    \"data_view\": {\
        \"name\": \"Filebeat\",\
        \"title\": \"filebeat-*\"\
    }\
}";


docker run --rm --network $(docker network ls -f "name=elastic" -f "driver=bridge" -q) alpine/curl -fsSL \
    -X POST https://kibana:5601/api/data_views/data_view \
    -H "Content-Type: application/json" \
    -u 'elastic:changeme' \
    -H 'Accept: */*' \
    -H 'Connection: keep-alive' \
    -H 'Content-Type: application/json' \
    -H 'Origin: https://kibana:5601' \
    -H 'Referer: https://kibana:5601/api/data_views/data_view' \
    -H 'kbn-version: 8.9.2' \
    --data-raw "${body}" \
    --insecure