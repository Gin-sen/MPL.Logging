# MPL.Logging

# Prise en compte de l'environnement

Le package de log et l'API devraient être configuré pour ne pas activer les APM et les logs au format ECS (json) en mode Development.

Env :
- Development                  Pas d'APM, pas de logs au format ECS 
- Docker                       APM et logs au format ECS pour des raison de developpement du package (TODO: voir si ca peut se régler a coup de directives de pre-processor (#if DEBUG))
- Inté, Recette, Autre, ...    APM + logs au format ECS 

# Mise en place du cluster ELK local

## Initialiser le cluster ELK 

Lancer profil ELK dans VS.

ou

Lancer la commande :

`docker compose up -d setup es01 kibana metricbeat01 filebeat01 logstash01 fleet-server`

## Activer les APM

Aller sur https://localhost:5601 (elastic / changeme)
Aller sur la page Fleet > Settings > Output > Cliquer sur modifier

Lancer la commande : `docker cp <projet-docker-compose>-es01-1:/usr/share/elasticsearch/config/certs/ca/ca.crt .`
Lancer la commande et copiez le resultat : `openssl x509 -fingerprint -sha256 -noout -in ca.crt | awk -F"=" {' print $2 '} | sed s/://g`
Lancer la commande et copiez le resultat : `cat ca.crt`

Dans l'interface de Kibana :
- modifier http://localhost:9200 en https://es01:9200
- coller le resultat de la commande openssl
- coller ce yaml dans la partie finale de la configuration et y ajouter le certificat (résultat de la commande cat) :

```yaml
ssl:
  certificate_authorities:
  - |
    -----BEGIN CER.....
    .......
    ......
```
- sauvegardez, vous pouvez maintenant utiliser les APM sur votre cluster local (url: http://fleet-server:8220, token: supersecrettoken).

## Ajouter une dataview pour les logs filebeat

Aller dans Discover, cliquer sur les dataviews en haut a gauche et sélectionner "créer une dataview".

Nommer la dataview `filebeat` et renseignez `filebeat*` dans le champ de pattern, sauvegarder.

