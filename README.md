# Catebi.Web


## Scaffold CatebiContext

``` bash
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Username=catebi_admin;Password=password;Database=catebi" Npgsql.EntityFrameworkCore.PostgreSQL -o Db/Catebi/Entities --context-dir Db/Catebi/Context -c CatebiContext --schema ctb --table assessment --no-onconfiguring --no-pluralize --force
```