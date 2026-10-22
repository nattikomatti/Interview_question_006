# interview-question-006 — Product Barcode (IT 06-1)

Single-page app to add product codes and display them as Code 39 barcodes.

- Product code: 16 characters, digits and uppercase English letters only, format `XXXX-XXXX-XXXX-XXXX`
- Delete asks for confirmation first
- Barcodes are rendered as SVG by a hand-written Code 39 encoder (no third-party barcode library)

## Stack

- ASP.NET Core (.NET 10) Web API + EF Core 10 (SQL Server)
- Angular 21 (served through the ASP.NET Core SPA proxy)

## Solution layout

| Project | Responsibility |
|---|---|
| `Interview_question_006.Domain` | `Product` entity, `ProductCode` value object (validation + formatting) |
| `Interview_question_006.Application` | `ProductService`, repository / barcode interfaces, DTOs, exceptions |
| `Interview_question_006.Infrastructure` | EF Core `AppDbContext`, migrations, `ProductRepository`, `Code39BarcodeGenerator` |
| `Interview_question_006.Server` | `ProductsController`, ProblemDetails error handling, hosting |
| `interview_question_006.client` | Angular UI |
| `Interview_question_006.Tests` | xUnit tests for the domain, application service and Code 39 encoder |

Root namespace: `Example.InterviewQuestion006.*`.

## Database

Table `dbo.Products`

| Column | Type | Notes |
|---|---|---|
| `Id` | `int IDENTITY` | Primary key |
| `ProductCode` | `char(16)` | Stored without dashes; unique index; check constraint `LEN = 16` and only `A-Z0-9` (binary collation) |
| `CreatedAt` | `datetime2` | Default `SYSUTCDATETIME()` |

## API

| Method | Route | Result |
|---|---|---|
| `GET` | `/api/products` | List of products |
| `POST` | `/api/products` `{ "productCode": "ABCD-1234-EFGH-5678" }` | `201`, `400` invalid, `409` duplicate |
| `DELETE` | `/api/products/{id}` | `204`, `404` |
| `GET` | `/api/products/{id}/barcode` | `image/svg+xml` |

## Running

1. Set the connection string `ConnectionStrings:DefaultConnection` in
   `Interview_question_006.Server/appsettings.json` (or `appsettings.Development.json` / user secrets).
2. Start `Interview_question_006.Server` (Visual Studio or `dotnet run --project Interview_question_006.Server`).
   In the Development environment the database is created / migrated automatically.
   To migrate manually: `dotnet ef database update -p Interview_question_006.Infrastructure -s Interview_question_006.Server`.

## Tests

```bash
dotnet test Interview_question_006.Tests
```

```bash
cd interview_question_006.client && npx ng test --watch=false
```
