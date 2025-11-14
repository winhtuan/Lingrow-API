1. 
```bash
dotnet add .\Lingrow.BusinessLogicLayer\Lingrow.BusinessLogicLayer.csproj package Microsoft.AspNetCore.Cryptography.KeyDerivation --version 8.0.11
dotnet add Lingrow.Api package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add Lingrow.Api package Microsoft.IdentityModel.Tokens
dotnet add Lingrow.Api package AWSSDK.CognitoIdentityProvider

```