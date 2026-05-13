# Inventory Counting Automation System

**Repository:** `inventory-counting-automation-dotnet`  
**Version:** `v1.0.0 - Backend MVP`

## Overview

Inventory Counting Automation System is a .NET 8 Web API backend MVP inspired by real ERP/MRP warehouse and manufacturing workflows. It focuses on automating the core physical inventory counting process: creating stock count sessions, uploading Excel count sheets, comparing counted quantities against system balances, generating variance reports, approving adjustments, and preserving adjustment history.

This version is backend-only and provides a clean API foundation with Swagger/OpenAPI documentation and flat DTO responses for predictable integration.

## Version 1 Scope

Version 1.0.0 focuses on the core backend workflow:

- Product management
- Warehouse management
- Stock balance tracking
- Stock count session creation
- Excel upload for counted quantities
- Variance report generation
- Approval workflow
- Automatic stock balance update
- Stock adjustment tracking
- Swagger API documentation
- Flat DTO responses

## Business Problem

Many warehouse and manufacturing operations still rely on manual Excel-based inventory counting. Teams often export product lists, enter counted quantities manually, compare results by hand, and then update stock records later. This creates operational risks:

- Counting errors and duplicated manual work
- Delayed stock corrections
- Poor traceability of inventory changes
- Limited visibility into shortages and surpluses
- Weak approval control before stock updates

In ERP/MRP-style environments, inventory accuracy directly affects purchasing, production planning, fulfillment, and financial reporting.

## Solution

The system supports a controlled stock counting workflow. Users create a stock count session for a warehouse, upload an Excel file containing counted quantities, and let the backend compare the uploaded counts against current system stock balances.

The API generates variance information, separates shortages, surpluses, and matched items, and updates stock balances only after the session is approved. When approval happens, the system records stock adjustments so inventory changes remain traceable.

## Features

- Manage products with active/inactive status
- Manage warehouses with active/inactive status
- Track stock balances by product and warehouse
- Create stock count sessions with generated session numbers
- Upload `.xlsx` count files using ClosedXML
- Validate Excel headers, product codes, quantities, and stock balances
- Generate variance reports from uploaded count lines
- Approve stock count sessions
- Automatically update stock balances after approval
- Store stock adjustment history
- Expose Swagger/OpenAPI documentation
- Return flat DTO responses to avoid nested JSON cycles

## Tech Stack

| Area | Technology |
|---|---|
| Runtime | .NET 8 |
| API | ASP.NET Core Web API |
| Data Access | Entity Framework Core |
| Database | SQL Server |
| Excel Processing | ClosedXML |
| API Documentation | Swagger / OpenAPI |
| Structure | Clean Architecture style |

## Architecture

The solution follows a Clean Architecture style structure, separating API contracts, domain models, application DTOs, and infrastructure concerns.

```text
src/
  InventoryCounting.Api
  InventoryCounting.Application
  InventoryCounting.Domain
  InventoryCounting.Infrastructure

docs/
  images/
  sample-data/
```

| Layer | Responsibility |
|---|---|
| API layer | Controllers, HTTP endpoints, Swagger setup, request handling |
| Application layer | DTOs and application-facing contracts |
| Domain layer | Core business entities and enums |
| Infrastructure layer | EF Core DbContext, SQL Server configuration, persistence mapping |

## Main Workflow

1. Create products.
2. Create a warehouse.
3. Add stock balance records.
4. Create a stock count session.
5. Upload an Excel count file.
6. Generate a variance report.
7. Approve the stock count session.
8. Update stock balances automatically.
9. Create stock adjustment history.

## Database Tables

| Table | Purpose |
|---|---|
| Products | Stores product master data |
| Warehouses | Stores warehouse master data |
| StockBalances | Stores current stock quantity by product and warehouse |
| StockCountSessions | Stores stock count session headers |
| StockCountLines | Stores uploaded counted quantities and variances |
| StockAdjustments | Stores approved inventory adjustment history |

## API Endpoints

### Products

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Products` | Get active products |
| GET | `/api/Products/{id}` | Get one active product |
| POST | `/api/Products` | Create product |
| PUT | `/api/Products/{id}` | Update product |
| DELETE | `/api/Products/{id}` | Soft delete product |

### Warehouses

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Warehouses` | Get active warehouses |
| GET | `/api/Warehouses/{id}` | Get one active warehouse |
| POST | `/api/Warehouses` | Create warehouse |
| PUT | `/api/Warehouses/{id}` | Update warehouse |
| DELETE | `/api/Warehouses/{id}` | Soft delete warehouse |

### Stock Balances

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/StockBalances` | Get all stock balances |
| GET | `/api/StockBalances/{id}` | Get one stock balance |
| GET | `/api/StockBalances/warehouse/{warehouseId}` | Get balances for one warehouse |
| POST | `/api/StockBalances` | Create stock balance |
| PUT | `/api/StockBalances/{id}` | Update quantity |

### Stock Count Sessions

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/StockCountSessions` | Get stock count sessions |
| GET | `/api/StockCountSessions/{id}` | Get one stock count session |
| POST | `/api/StockCountSessions` | Create stock count session |
| GET | `/api/StockCountSessions/{id}/lines` | Get session lines |
| POST | `/api/StockCountSessions/{id}/upload-excel` | Upload counted quantities from Excel |
| GET | `/api/StockCountSessions/{id}/variance-report` | Get variance report |
| POST | `/api/StockCountSessions/{id}/approve` | Approve session and update stock |

### Stock Adjustments

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/StockAdjustments` | Get all stock adjustments |
| GET | `/api/StockAdjustments/{id}` | Get one stock adjustment |
| GET | `/api/StockAdjustments/session/{sessionId}` | Get adjustments for one stock count session |

## Sample Requests

### Create Product

`POST /api/Products`

```json
{
  "code": "PRD-001",
  "name": "Cement Bag",
  "unit": "Bag"
}
```

### Create Warehouse

`POST /api/Warehouses`

```json
{
  "name": "Main Warehouse",
  "location": "Riyadh"
}
```

### Create Stock Balance

`POST /api/StockBalances`

```json
{
  "productId": 1,
  "warehouseId": 1,
  "quantity": 100
}
```

### Create Stock Count Session

`POST /api/StockCountSessions`

```json
{
  "warehouseId": 1,
  "countDate": "2026-05-13T00:00:00",
  "createdBy": "admin@demo.com"
}
```

### Approve Stock Count Session

`POST /api/StockCountSessions/1/approve`

```json
{
  "approvedBy": "admin@demo.com"
}
```

## Sample Excel Format

The Excel upload endpoint expects a `.xlsx` file with the first worksheet using this format:

| ProductCode | CountedQuantity |
|---|---:|
| PRD-001 | 95 |
| PRD-002 | 60 |
| PRD-003 | 200 |

Rules:

- `ProductCode` must match an active product.
- `CountedQuantity` must be numeric and non-negative.
- File format must be `.xlsx`.
- Headers must be exactly `ProductCode` and `CountedQuantity`.

## How to Run Locally

### Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or Rider
- EF Core tools

### Commands

```powershell
dotnet restore
dotnet build
dotnet ef database update --project src/InventoryCounting.Infrastructure --startup-project src/InventoryCounting.Api
dotnet run --project src/InventoryCounting.Api
```

Swagger is available at:

```text
https://localhost:{port}/swagger
```

## Demo Screenshots

Screenshots can be added under `docs/images/`.

![Swagger](docs/images/swagger.png)
![Stock Balances](docs/images/stock-balances.png)
![Upload Excel](docs/images/upload-excel.png)
![Variance Report](docs/images/variance-report.png)
![Stock Adjustments](docs/images/stock-adjustments.png)

## Future Improvements

- JWT authentication
- Role-based access control
- Admin, Inventory Clerk, and Inventory Manager roles
- Audit logs
- Export variance report to Excel
- Angular frontend
- Dashboard analytics
- Docker support
- Unit and integration tests
- Barcode scanning

## License

This project is currently provided as a backend MVP reference implementation. Add a license file before using it in a production or commercial environment.
