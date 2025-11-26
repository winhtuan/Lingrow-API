1. Install nuget

```bash

dotnet add .\Lingrow.DataAccessLayer package Microsoft.EntityFrameworkCore --version 8.0.21
dotnet add .\Lingrow.DataAccessLayer package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.8
dotnet add .\Lingrow.DataAccessLayer package Microsoft.EntityFrameworkCore.Tools --version 8.0.21
dotnet add .\Lingrow.DataAccessLayer package Microsoft.EntityFrameworkCore.Design --version 8.0.21
dotnet add .\Lingrow.Api package Microsoft.EntityFrameworkCore.Design --version 8.0.21

```

2. First run

```bash

dotnet ef migrations add InitialCreate --project .\Lingrow.DataAccessLayer\Lingrow.DataAccessLayer.csproj --startup-project .\Lingrow.Api\Lingrow.Api.csproj --output-dir Migrations

dotnet ef database update --project .\Lingrow.DataAccessLayer\Lingrow.DataAccessLayer.csproj --startup-project .\Lingrow.Api\Lingrow.Api.csproj

dotnet ef migrations remove --project .\Lingrow.DataAccessLayer\Lingrow.DataAccessLayer.csproj --startup-project .\Lingrow.Api\Lingrow.Api.csproj

```

3. Update database

```bash

dotnet ef migrations add Lingrow_v1_5 --project .\Lingrow.DataAccessLayer\Lingrow.DataAccessLayer.csproj --startup-project .\Lingrow.Api\Lingrow.Api.csproj --output-dir Migrations

dotnet ef database update --project .\Lingrow.DataAccessLayer\Lingrow.DataAccessLayer.csproj --startup-project .\Lingrow.Api\Lingrow.Api.csproj

```

4. Generation sql file

```bash

dotnet ef migrations script --idempotent --project .\Lingrow.DataAccessLayer\Lingrow.DataAccessLayer.csproj --startup-project .\Lingrow.Api\Lingrow.Api.csproj -o Lingrow.sql

```
