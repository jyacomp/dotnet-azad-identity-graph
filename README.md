# dotnet-azad-identity-graph
dotnet 5 |  azure ad | identity | graph

```
# .\src\Infrastructure
dotnet new classlib -o .\src\Infrastructure\Infrastructure.Identity -f net5.0
dotnet add .\src\Infrastructure\Infrastructure.Identity package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add .\src\Infrastructure\Infrastructure.Identity package Microsoft.Extensions.Configuration.Abstractions
dotnet add .\src\Infrastructure\Infrastructure.Identity package Microsoft.Identity.Web.MicrosoftGraph -v 1.15.2
dotnet add .\src\Infrastructure\Infrastructure.Identity package Microsoft.Identity.Web -v 1.16.0

rm .\src\Infrastructure\Infrastructure.Identity\Class1.cs
mkdir .\src\Infrastructure\Infrastructure.Identity\Graph
echo 'namespace Infrastructure.Identity.Graph { }' > .\src\Infrastructure\Infrastructure.Identity\Graph\GraphUserService.cs
echo 'namespace Infrastructure.Identity.Graph { }' > .\src\Infrastructure\Infrastructure.Identity\Graph\GraphEventService.cs

echo 'namespace Infrastructure.Identity { }' > .\src\Infrastructure\Infrastructure.Identity\DependencyInjection.cs

# .\src\Presentation
dotnet new mvc -o .\src\Presentation\WebApp -f net5.0 -au MultiOrg
dotnet add .\src\Presentation\WebApp reference .\src\Infrastructure\Infrastructure.Identity


dotnet new sln
dotnet sln add (ls -r .\**\*.csproj)
```
