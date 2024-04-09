# Catebi.Api

## scaffold CatebiContext

``` bash
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Username=catebi_admin;Password=password;Database=catebi" Npgsql.EntityFrameworkCore.PostgreSQL -o Db/Entities --context-dir Db/Context -c CatebiContext --schema ctb --schema frgn --no-onconfiguring --no-pluralize --force
```

## how to run localy with docker-compose

### prerequisites

- install sdk .Net 8 [https://dotnet.microsoft.com/en-us/download/dotnet/8.0]
- install vscode [https://code.visualstudio.com/download]
- install docker desktop [https://docs.docker.com/get-docker/]
- install pgAdmin4 [https://www.pgadmin.org/download/]
- or install Azure Data Studio [https://learn.microsoft.com/en-us/azure-data-studio/download-azure-data-studio]
- or any other db ide

```bash

# 0. install sdk .Net 8
# https://dotnet.microsoft.com/en-us/download/dotnet/8.0

# 1. build api image and pull both into docker: catebi_api image and postgres image
docker-compose up -d --build

# 2. connect to postgres container via pgAdmin4 or other db ide with credentials
    - HOST=localhost
    - PORT=5432
    - USER=postgres
    - PASSWORD=password
    - POSTGRES_DB=postgres

# 3. run sql script to create db
db/schema/init_db_schema.sql

# 3.1 run first part of the script under postgres user and follow instructions
# 3.2 run second part of the script under catebi_admin user

# 4. create tables by running scripts from folder (under catebi_admin)
db/changelog/releases/1.0.0

# 5. fill tables with data by running scripts from folder (under catebi_admin)
db/changelog/releases/1.0.0/primary_filling_of_db_tables

# 6. run api in vscode with debug mode and go to url
https://localhost:7294/api/map/getcats

# 7.1 save notion api token to dotnet secrets. run in directory Catebi.Api/Catebi.Api
# auth token can be found in notion app in settings -> connections -> catebi.webapi -> copy internal integration token
# NotionCatsDbId and NotionVolunteersDbId can be found in Cats db -> copy database link -> notion.so/DB_GUID?v=some_guid&pvs=4 (you need DB_GUID)

# run commands in terminal in directory Catebi.Api/Catebi.Api
dotnet user-secrets set "NotionApi:AuthToken" "AuthToken"
dotnet user-secrets set "NotionApi:DatabaseIds:Cats" "NotionCatsDbId"
dotnet user-secrets set "NotionApi:DatabaseIds:Volunteers" "NotionVolunteersDbId"

# 7.2 sync data with notion by running urls in browser
https://localhost:7294/api/notionsync/syncdicts
https://localhost:7294/api/notionsync/syncvolunteers
https://localhost:7294/api/notionsync/synccats

```

## usefull commands

```bash

# build image and push to local docker hub
docker build --pull --rm --platform linux/amd64 -f "ci/docker/Dockerfile.catebi.api" -t catebi_api:latest .

# build image
docker-compose up -d --build

# save image to disk
docker save catebi_api > catebi_api.tar

# save image to disk to some path
docker save -o ~/Documents/../catebi_api/catebi_api.tar catebi_api

# copy image to vps
scp -i /Users/../vps/catebi_ssh_vps /Users/../catebi_api/catebi_api.tar root@VPS_IP_ADDRESS:/_catebi/api/

# connect to vps via ssh
ssh root@VPS_IP_ADDRESS -i ~/Users/../catebi_ssh_vps

# stop api container
docker stop catebi_api

# remove api container
docker rm catebi_api

# remove api image
docker rmi catebi_api

# load image from disk
docker load < /_catebi/api/catebi_api.tar

# run api container with docker-compose file
docker-compose up -d
```