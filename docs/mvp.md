# RecipeExtractApp: MVP scope

## Goal
Paste a recipe **link or text** and get back a clean, **editable** recipe
(ingredients, steps, times, servings) saved to a **searchable** personal collection,
with a link to the original page.

## Features (in scope)
1. Accounts: register, log in; users only see their own recipes.
2. Clip from URL: schema.org Recipe data (JSON-LD) first, Claude API fallback.
3. Clip from pasted text: sent to the Claude API.
4. Clip status: background processing with states Queued => Processing => Succeeded / Failed.
   If a site blocks fetching, the error suggests pasting the text instead.
5. Recipe view: title, image, servings, times, ingredients, steps, source link.
6. Edit and delete recipes (to fix extraction mistakes).
7. My recipes: list with search (title or ingredient), tag filter, paging.
8. Tags: user-created (e.g. dinner, vegetarian).
9. URL cache: re-clipping the same page never calls the AI again.
10. Limits: daily clip limit per user, fetch timeout and size limit.

## Out of scope (later versions)
Servings scaling, unit conversion, shopping lists, meal planning, collections,
sharing, browser extension, mobile app, photo uploads.

## Data model
| Table | Key fields |
|---|---|
| Users | ASP.NET Core Identity |
| Recipes | Id, UserId, Title, SourceUrl, SourceSite, ImageUrl, Servings, PrepMinutes, CookMinutes, Notes, ExtractionMethod (StructuredData / Ai / Manual), CreatedAt |
| Ingredients | Id, RecipeId, Position, Quantity (decimal?), Unit, Name, Note, OriginalText |
| Steps | Id, RecipeId, Position, Text |
| Tags | Id, UserId, Name |
| RecipeTags | RecipeId, TagId |
| ClipJobs | Id, UserId, InputUrl, InputText, Status, Error, RecipeId, IdempotencyKey, CreatedAt, UpdatedAt |
| ExtractionCache | Id, NormalizedUrl (unique), ExtractedJson, CreatedAt |

Notes:
- Keep OriginalText on every ingredient so parsing mistakes never lose the source wording.
- Every query filters by the current user.

## API
| Method | Route | Purpose |
|---|---|---|
| POST | /api/auth/register | Create account |
| POST | /api/auth/login | Get JWT |
| POST | /api/clips | Body { url } or { text }; returns 202 Accepted + job location; supports Idempotency-Key header |
| GET | /api/clips/{id} | Job status and resulting recipe id |
| GET | /api/recipes?search=&tag=&page=&pageSize= | List with search, filter, paging |
| GET | /api/recipes/{id} | One recipe |
| POST | /api/recipes | Manual entry |
| PUT | /api/recipes/{id} | Edit |
| DELETE | /api/recipes/{id} | Delete |
| GET | /api/tags | Current user's tags |
| GET | /health | Health check |

Errors use ProblemDetails. Validation with FluentValidation.

## Extraction pipeline
1. Normalize the URL (strip tracking parameters) and check ExtractionCache.
2. Fetch the page with HttpClient: timeout, size limit, honest user agent.
   Blocked or failed => job fails with "paste the recipe text instead".
3. Look for schema.org Recipe JSON-LD in script type="application/ld+json" blocks.
   Found => map to the domain model (no AI cost).
4. Otherwise clean the HTML (HtmlAgilityPack): remove scripts, nav, ads, comments; keep main text.
5. Call the Claude API (C# SDK) with structured outputs and the recipe JSON schema.
6. Validate: required fields present, ingredient names appear in the source text,
   quantities sensible. Failure => job fails, raw text kept for manual editing.
7. Save the recipe and cache the extraction.

Design: one IRecipeExtractor interface with StructuredDataExtractor and AiExtractor
implementations. Tests use a fake AI extractor and saved HTML fixtures, so tests never
call the real API.

## Reliability
- Retries with backoff and timeouts on page fetches and AI calls
  (Microsoft.Extensions.Http.Resilience).
- Idempotency key on POST /api/clips so double submits don't create two jobs.
- Rate limiting with ASP.NET Core's built-in middleware.
- Structured logging with a correlation id from request => worker => AI call.
- Health checks at /health.
- Job recovery: jobs stuck in Processing after a crash are re-queued on startup.
- Load test with NBomber: clips per minute and where it slows down.

## Screens (thin Blazor + MudBlazor)
1. Log in / register
2. Clip: URL box, paste-text box, live job status
3. My recipes: search box, tag chips, recipe cards
4. Recipe: clean reading view, ingredient checklist, numbered steps, source link
5. Edit recipe

## Milestones
1. Database model, EF Core, migrations, CI
2. Recipes CRUD API: search, tags, paging, validation, errors
3. Auth: Identity + JWT, per-user data
4. Clip jobs: queue, BackgroundService worker, status endpoint, idempotency
5. URL fetch + schema.org extraction + cache + resilience
6. HTML cleanup + Claude extraction + validation
7. Blazor screens
8. Sample-page tests, load test, Azure deploy with budget alerts

## Cost controls
- Structured data first; AI only as fallback.
- Cache extractions by URL.
- Daily clip limit per user.
- Spend limit in the Anthropic Console, budget alert in Azure.
- Tests never call the real Claude API.

## Done when
- a test set of 10 popular recipe sites gives correct results for at least 8;
  the other 2 fail with a helpful message.
- Pasted text produces a usable recipe.
- Search, tags, edit and delete work.
- dotnet test passes in CI (GitHub Actions).
- Deployed on Azure with budget alerts configured.