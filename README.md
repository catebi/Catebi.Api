# Catebi.Web


## Scaffold CatebiContext

``` bash
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Username=catebi_admin;Password=password;Database=catebi" Npgsql.EntityFrameworkCore.PostgreSQL -o Db/Entities --context-dir Db/Context -c CatebiContext --schema ctb --no-onconfiguring --no-pluralize --force
```