1. Install nuget

```bash

dotnet add .\Plantpedia.DataAccessLayer package Microsoft.EntityFrameworkCore --version 8.*
dotnet add .\Plantpedia.DataAccessLayer package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.*
dotnet add .\Plantpedia.DataAccessLayer package Microsoft.EntityFrameworkCore.Tools --version 8.*
dotnet add .\Plantpedia.DataAccessLayer package Microsoft.EntityFrameworkCore.Design --version 8.*
dotnet add .\Plantpedia.Api package Microsoft.EntityFrameworkCore.Design --version 8.*

```

2. First run

```bash

dotnet ef migrations add InitialCreate --project .\Plantpedia.DataAccessLayer\Plantpedia.DataAccessLayer.csproj --startup-project .\Plantpedia.Api\Plantpedia.Api.csproj --output-dir Migrations

dotnet ef database update --project .\Plantpedia.DataAccessLayer\Plantpedia.DataAccessLayer.csproj --startup-project .\Plantpedia.Api\Plantpedia.Api.csproj

dotnet ef migrations remove --project .\Plantpedia.DataAccessLayer\Plantpedia.DataAccessLayer.csproj --startup-project .\Plantpedia.Api\Plantpedia.Api.csproj

```

3. Update database

```bash

dotnet ef migrations add Plantpedia_v1_0 --project .\Plantpedia.DataAccessLayer\Plantpedia.DataAccessLayer.csproj --startup-project .\Plantpedia.Api\Plantpedia.Api.csproj --output-dir Migrations

dotnet ef database update --project .\Plantpedia.DataAccessLayer\Plantpedia.DataAccessLayer.csproj --startup-project .\Plantpedia.Api\Plantpedia.Api.csproj

```

4. Generation sql file

```bash

dotnet ef migrations script --idempotent --project .\Plantpedia.DataAccessLayer\Plantpedia.DataAccessLayer.csproj --startup-project .\Plantpedia.Api\Plantpedia.Api.csproj -o plantpedia.sql

```
