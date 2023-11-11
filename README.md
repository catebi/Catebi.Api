# Catebi.Web


## Scaffold CatebiContext

``` bash
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Username=catebi_admin;Password=password;Database=catebi" Npgsql.EntityFrameworkCore.PostgreSQL -o Db/Entities --context-dir Db/Context -c CatebiContext --schema ctb --no-onconfiguring --no-pluralize --force
```

```bash

# build image
docker-compose up -d --build

# save image to disk
docker save catebimap-catebi_api > catebi_api.tar

# save image to disk to some path
docker save -o ~/Documents/_personnel/catebi/vps/catebi_api/catebi_api.tar catebimap-catebi_api

# copy image to vps
scp -i /Users/aleksandrkarpov/Documents/_personnel/catebi/vps/catebi_ssh_vps /Users/aleksandrkarpov/Documents/_personnel/catebi/vps/catebi_api/catebi_api.tar root@VPS_IP_ADDRESS:/_catebi/api/

# connect to vps via ssh
ssh root@VPS_IP_ADDRESS -i ~/Users/../catebi_ssh_vps

# stop api container
docker stop catebi_api

# remove api container
docker rm catebi_api

# remove api image
docker rmi catebimap-catebi_api

# load image from disk
docker load < /_catebi/api/catebi_api.tar

# run api container with docker-compose file
docker-compose up -d
```

