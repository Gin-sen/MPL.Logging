#!/bin/bash

docker cp $(docker ps -f 'name=es01' --format '{{.Names}}'):/usr/share/elasticsearch/config/certs/ca/ca.crt .

TMP_ELK_CRT=$(cat ca.crt) yq e -i '.ssl.certificate_authorities[0] |= strenv(TMP_ELK_CRT) | .ssl.certificate_authorities[0] style="literal"' tmp_crt.yaml

cert=$(cat tmp_crt.yaml);

body="{\
    \"name\":\"default\",\
    \"type\":\"elasticsearch\",\
    \"hosts\":[\"https://es01:9200\"],\
    \"is_default\":true,\
    \"is_default_monitoring\":true,\
    \"config_yaml\":\"""${cert//$'\n'/\\r\\n}""\",\
    \"proxy_id\":null}";

docker run --rm --network $(docker network ls -f "name=elastic" -f "driver=bridge" -q) alpine/curl -fsSL 'https://kibana:5601/api/fleet/outputs/fleet-default-output' \
    -X 'PUT' \
    -u 'elastic:changeme' \
    -H 'Accept: */*' \
    -H 'Connection: keep-alive' \
    -H 'Content-Type: application/json' \
    -H 'Origin: https://kibana:5601' \
    -H 'Referer: https://kibana:5601/app/fleet/settings/outputs/fleet-default-output' \
    -H 'kbn-version: 8.9.2' \
    --data-raw "${body}" \
    --insecure