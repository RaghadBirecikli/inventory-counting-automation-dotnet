# Inventory Counting Automation System

**Repository:** `inventory-counting-automation-dotnet`  
**Current Version:** `v1.1.0 - Audit Logs`

## Overview

Inventory Counting Automation System is a .NET 8 Web API backend project inspired by real ERP/MRP warehouse and manufacturing workflows.

The system automates the core physical inventory counting process by allowing users to create stock count sessions, upload Excel count sheets, compare counted quantities against system stock balances, generate variance reports, approve stock adjustments, update inventory balances, preserve inventory adjustment history, and track key workflow actions through audit logs.

This version is backend-only and provides a clean API foundation with Swagger/OpenAPI documentation, flat DTO responses, SQL Server persistence, and traceability for important inventory workflow events.

## Version History

| Version | Description |
|---|---|
| v1.0.0 | Backend MVP with inventory counting workflow |
| v1.1.0 | Added audit logging for key inventory workflow actions |

## Version 1.1 Scope

Version 1.1.0 includes the complete backend workflow from v1.0.0, plus audit logging.

Included features:

- Product management
- Warehouse management
- Stock balance tracking
- Stock count session creation
- Excel upload for counted quantities
- Variance report generation
- Approval workflow
- Automatic stock balance update
- Stock adjustment tracking
- Audit logging for key workflow actions
- Swagger API documentation
- Flat DTO responses

## Current Status

The backend MVP is complete and includes the full inventory counting workflow from stock count session creation to Excel upload, variance calculation, approval, stock balance update, stock adjustment tracking, and audit log tracking.

Authentication, role-based access, frontend UI, advanced reporting, dashboard features, and export capabilities are planned for future versions.

## Business Problem

Many warehouse and manufacturing operations still rely on manual Excel-based inventory counting. Teams often export product lists, enter counted quantities manually, compare results by hand, and then update stock records later.

This creates operational risks:

- Counting errors and duplicated manual work
- Delayed stock corrections
- Poor traceability of inventory changes
- Limited visibility into shortages and surpluses
- Weak approval control before stock updates
- Difficulty tracking who performed important inventory actions

In ERP/MRP-style environments, inventory accuracy directly affects purchasing, production planning, fulfillment, and financial reporting.

## Solution

The system supports a controlled stock counting workflow.

Users create a stock count session for a warehouse, upload an Excel file containing counted quantities, and let the backend compare the uploaded counts against current system stock balances.

The API generates variance information, classifies shortages, surpluses, and matched items, and updates stock balances only after the session is approved.

When approval happens, the system records stock adjustment history so inventory changes remain traceable.

Version 1.1.0 also adds audit logs for key workflow actions, such as stock count session creation, Excel upload, stock count approval, and stock balance updates.

## Features

- Manage products with active/inactive status
- Manage warehouses with active/inactive status
- Track stock balances by product and warehouse
- Create stock count sessions with generated session numbers
- Upload `.xlsx` count files using ClosedXML
- Validate Excel headers, product codes, counted quantities, and stock balances
- Generate variance reports from uploaded count lines
- Approve stock count sessions
- Automatically update stock balances after approval
- Store stock adjustment history
- Track key workflow actions through audit logs
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
````

| Layer                | Responsibility                                                   |
| -------------------- | ---------------------------------------------------------------- |
| API layer            | Controllers, HTTP endpoints, Swagger setup, request handling     |
| Application layer    | DTOs and application-facing contracts                            |
| Domain layer         | Core business entities and enums                                 |
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
10. Create audit logs for key workflow actions.

## Database Tables

| Table              | Purpose                                                     |
| ------------------ | ----------------------------------------------------------- |
| Products           | Stores product master data                                  |
| Warehouses         | Stores warehouse master data                                |
| StockBalances      | Stores current stock quantity by product and warehouse      |
| StockCountSessions | Stores stock count session headers                          |
| StockCountLines    | Stores uploaded counted quantities and calculated variances |
| StockAdjustments   | Stores approved inventory adjustment history                |
| AuditLogs          | Stores traceability records for important workflow actions  |

## API Endpoints

### Products

| Method | Endpoint             | Description            |
| ------ | -------------------- | ---------------------- |
| GET    | `/api/Products`      | Get active products    |
| GET    | `/api/Products/{id}` | Get one active product |
| POST   | `/api/Products`      | Create product         |
| PUT    | `/api/Products/{id}` | Update product         |
| DELETE | `/api/Products/{id}` | Soft delete product    |

### Warehouses

| Method | Endpoint               | Description              |
| ------ | ---------------------- | ------------------------ |
| GET    | `/api/Warehouses`      | Get active warehouses    |
| GET    | `/api/Warehouses/{id}` | Get one active warehouse |
| POST   | `/api/Warehouses`      | Create warehouse         |
| PUT    | `/api/Warehouses/{id}` | Update warehouse         |
| DELETE | `/api/Warehouses/{id}` | Soft delete warehouse    |

### Stock Balances

| Method | Endpoint                                     | Description                    |
| ------ | -------------------------------------------- | ------------------------------ |
| GET    | `/api/StockBalances`                         | Get all stock balances         |
| GET    | `/api/StockBalances/{id}`                    | Get one stock balance          |
| GET    | `/api/StockBalances/warehouse/{warehouseId}` | Get balances for one warehouse |
| POST   | `/api/StockBalances`                         | Create stock balance           |
| PUT    | `/api/StockBalances/{id}`                    | Update stock quantity          |

### Stock Count Sessions

| Method | Endpoint                                       | Description                          |
| ------ | ---------------------------------------------- | ------------------------------------ |
| GET    | `/api/StockCountSessions`                      | Get stock count sessions             |
| GET    | `/api/StockCountSessions/{id}`                 | Get one stock count session          |
| POST   | `/api/StockCountSessions`                      | Create stock count session           |
| GET    | `/api/StockCountSessions/{id}/lines`           | Get session lines                    |
| POST   | `/api/StockCountSessions/{id}/upload-excel`    | Upload counted quantities from Excel |
| GET    | `/api/StockCountSessions/{id}/variance-report` | Get variance report                  |
| POST   | `/api/StockCountSessions/{id}/approve`         | Approve session and update stock     |

### Stock Adjustments

| Method | Endpoint                                    | Description                                 |
| ------ | ------------------------------------------- | ------------------------------------------- |
| GET    | `/api/StockAdjustments`                     | Get all stock adjustments                   |
| GET    | `/api/StockAdjustments/{id}`                | Get one stock adjustment                    |
| GET    | `/api/StockAdjustments/session/{sessionId}` | Get adjustments for one stock count session |

### Audit Logs

| Method | Endpoint                                        | Description                          |
| ------ | ----------------------------------------------- | ------------------------------------ |
| GET    | `/api/AuditLogs`                                | Get all audit logs                   |
| GET    | `/api/AuditLogs/{id}`                           | Get one audit log                    |
| GET    | `/api/AuditLogs/entity/{entityName}/{entityId}` | Get audit logs for a specific entity |

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
  "countDate": "2026-05-14T00:00:00",
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
| ----------- | --------------: |
| PRD-001     |              95 |
| PRD-002     |              60 |
| PRD-003     |             200 |

Rules:

* `ProductCode` must match an active product.
* `CountedQuantity` must be numeric and non-negative.
* File format must be `.xlsx`.
* Headers must be exactly `ProductCode` and `CountedQuantity`.

## Variance Report Example

`GET /api/StockCountSessions/1/variance-report`

```json
[
  {
    "productCode": "PRD-001",
    "productName": "Cement Bag",
    "systemQuantity": 100,
    "countedQuantity": 95,
    "difference": -5,
    "varianceType": "Shortage"
  }
]
```

Variance type logic:

|     Difference | Variance Type |
| -------------: | ------------- |
|    Less than 0 | Shortage      |
| Greater than 0 | Surplus       |
|     Equal to 0 | Matched       |

## Stock Adjustment Example

`GET /api/StockAdjustments`

```json
[
  {
    "id": 1,
    "stockCountSessionId": 1,
    "sessionNumber": "SC-2026-0001",
    "productId": 1,
    "productCode": "PRD-001",
    "productName": "Cement Bag",
    "warehouseId": 1,
    "warehouseName": "Main Warehouse",
    "oldQuantity": 100,
    "newQuantity": 95,
    "adjustmentQuantity": -5,
    "reason": "Inventory count approval",
    "approvedBy": "admin@demo.com",
    "approvedAt": "2026-05-13T10:28:15.3368585"
  }
]
```

## Audit Log Example

`GET /api/AuditLogs`

```json
[
  {
    "id": 6,
    "action": "StockBalanceUpdated",
    "entityName": "StockBalance",
    "entityId": "1",
    "oldValue": "Quantity: 95.00",
    "newValue": "Quantity: 90.00",
    "createdBy": "admin2",
    "createdAt": "2026-05-14T06:32:26.6191252"
  },
  {
    "id": 7,
    "action": "StockCountSessionApproved",
    "entityName": "StockCountSession",
    "entityId": "3",
    "oldValue": "Status: Uploaded",
    "newValue": "Status: Approved, AdjustmentsCreated: 1",
    "createdBy": "admin2",
    "createdAt": "2026-05-14T06:32:26.6191252"
  },
  {
    "id": 5,
    "action": "ExcelUploaded",
    "entityName": "StockCountSession",
    "entityId": "3",
    "oldValue": "Status: Draft",
    "newValue": "Status: Uploaded, LinesCount: 1",
    "createdBy": "admin2",
    "createdAt": "2026-05-14T06:31:13.8360031"
  },
  {
    "id": 4,
    "action": "StockCountSessionCreated",
    "entityName": "StockCountSession",
    "entityId": "3",
    "oldValue": null,
    "newValue": "SessionNumber: SC-2026-0003, WarehouseId: 1, Status: Draft",
    "createdBy": "admin2",
    "createdAt": "2026-05-14T06:30:31.3523247"
  }
]
```

Tracked audit actions:

| Action                      | Description                                             |
| --------------------------- | ------------------------------------------------------- |
| `StockCountSessionCreated`  | Created when a new stock count session is created       |
| `ExcelUploaded`             | Created when counted quantities are uploaded from Excel |
| `StockCountSessionApproved` | Created when a stock count session is approved          |
| `StockBalanceUpdated`       | Created when stock quantity changes after approval      |

## How to Run Locally

### Prerequisites

* .NET 8 SDK
* SQL Server
* Visual Studio 2022 or Rider
* EF Core tools

Install EF Core tools if needed:

```powershell
dotnet tool install --global dotnet-ef
```

### Connection String

Update the connection string in:

```text
src/InventoryCounting.Api/appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ICAS;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

For SQL Server Express, you may use:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ICAS;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

For LocalDB, you may use:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ICAS;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Commands

Run these commands from the repository root:

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

## Future Improvements

* JWT authentication
* Role-based access control
* Admin, Inventory Clerk, and Inventory Manager roles
* Export variance report to Excel
* Angular frontend
* Dashboard analytics
* Docker support
* Unit and integration tests
* Barcode scanning

## Version Roadmap

| Version | Planned Scope                                    |
| ------- | ------------------------------------------------ |
| v1.0.0  | Backend MVP with inventory counting workflow     |
| v1.1.0  | Audit logs for key inventory workflow actions    |
| v1.2.0  | JWT authentication and role-based access control |
| v1.3.0  | Export reports to Excel                          |
| v2.0.0  | Angular frontend and dashboard                   |

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
