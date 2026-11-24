# RecipeExtractApp

C#/.NET backend App
Recipe clipper: paste a recipe link or text and get a clean, editable,
searchable recipe without reading through the whole blog post.

Full MVP scope: docs/mvp.md.

## UI
Thin Blazor Web App (Interactive Server) + MudBlazor front end that calls the REST API.

## Solution layout
- RecipeExtractApp.Domain: entities and rules, depends on nothing
- RecipeExtractApp.Application: use cases, interfaces, DTOs
- RecipeExtractApp.Infrastructure: EF Core, SQL Server, HTTP fetching, Claude API
- RecipeExtractApp.Api: ASP.NET Core Web API (controllers)
- RecipeExtractApp.UnitTests: xUnit
Dependencies point inward: Api => Infrastructure => Application => Domain.

## Stack
.NET 10, ASP.NET Core Web API, SQL Server + EF Core, Identity + JWT,
BackgroundService job queue, HtmlAgilityPack, schema.org JSON-LD first and
the Claude API (C# SDK, structured outputs) as fallback, xUnit + Testcontainers,
GitHub Actions, Azure (Container Apps, Azure SQL, Key Vault).

## Rules
- Package versions live only in Directory.Packages.props; .csproj files have no Version attributes.
- Shared build settings live in Directory.Build.props.
- Tests never call the real Claude API (use a fake extractor and saved HTML fixtures).
- Validate AI output before saving.
- Use decimal for ingredient quantities.