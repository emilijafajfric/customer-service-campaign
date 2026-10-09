# Customer Service Campaign API

Recruitment technical assignment implemented with ASP.NET Core Web API.

## Overview

The application simulates a customer loyalty campaign for a telecommunications company.

During a one-week campaign, agents can reward loyal customers with a discount on future purchases. Each agent can reward up to five customers per day.

After the campaign, a CSV report containing successful purchases can be imported. The application merges reward and purchase data and exposes campaign results through a secured REST API.

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- JWT authentication
- SOAP integration
- xUnit

## Main Features

- Reward customers during an active campaign
- Maximum of 5 rewarded customers per agent per day
- Prevent duplicate rewards for the same customer within a campaign
- Validate customers through the external SOAP `FindPerson` service
- Import successful purchases from CSV
- Prevent duplicate purchase imports by purchase reference
- Expose campaign results combining reward and purchase data
- JWT authentication and role-based authorization
- Automated tests for core business flows

## Architecture

Business logic is kept separate from external integrations and persistence.

Customer validation is abstracted through:

```text
ICustomerService
    |
    +---- SoapCustomerService
              |
              +---- External FindPerson SOAP service
```

`RewardService` depends only on `ICustomerService`, so the external customer provider can be replaced without changing campaign business logic.

Persistence is handled with Entity Framework Core and SQLite.

## Assumptions

The assignment does not define every implementation detail, so the following assumptions were made:

- A campaign lasts seven calendar days.
- Each agent can reward a maximum of five customers per calendar day.
- A customer can be rewarded only once within the same campaign.
- Customer master data belongs to the external customer service and is not stored locally.
- The external customer ID is stored as `CustomerId`.
- A customer may have multiple successful purchases.
- The CSV structure was not specified, so a simple schema was defined.
- Agents can create rewards.
- Administrators can import purchases and view campaign results.

## External Customer Service

Customer validation uses the SOAP service provided in the assignment:

```text
https://www.crcind.com/csp/samples/SOAP.Demo.cls
```

The application calls the `FindPerson` operation using the customer ID.

The SOAP integration is implemented in `SoapCustomerService` and isolated behind `ICustomerService`.

## CSV Format

Expected format:

```csv
CustomerId,PurchaseReference,Amount,PurchaseDate
1,PUR-001,299.99,2026-11-07T10:30:00Z
2,PUR-002,149.50,2026-11-07T11:00:00Z
3,PUR-003,499.00,2026-11-08T09:15:00Z
```

Example files are available in the `samples` directory.

## Authentication

The API uses JWT Bearer authentication.

Demo users:

```text
Agent
Username: agent1
Password: Agent123!

Admin
Username: admin
Password: Admin123!
```

These credentials are intended only for local development and the recruitment assignment.

The JWT signing key is stored using .NET User Secrets and is not committed to source control.

Configure it with:

```bash
dotnet user-secrets init --project src/CustomerServiceCampaign.Api

dotnet user-secrets set "Jwt:Key" "YOUR_GENERATED_KEY" \
  --project src/CustomerServiceCampaign.Api
```

A signing key can be generated with:

```bash
openssl rand -base64 48
```

## Running the Application

Restore and build:

```bash
dotnet restore
dotnet build
```

Run the API:

```bash
dotnet run --project src/CustomerServiceCampaign.Api
```

The Development environment automatically applies EF Core migrations and seeds a sample campaign and agents.

Default local URL:

```text
http://localhost:5179
```

## Main API Endpoints

```text
POST /api/auth/login
POST /api/rewards
POST /api/purchases/import
GET  /api/campaigns/{campaignId}/results
```

Authorization:

```text
Agent
  POST /api/rewards

Admin
  POST /api/purchases/import
  GET  /api/campaigns/{campaignId}/results
```

Example requests are available in:

```text
src/CustomerServiceCampaign.Api/CustomerServiceCampaign.Api.http
```

## Tests

Run all tests with:

```bash
dotnet test
```

The test suite covers:

- reward creation
- unknown customer handling
- duplicate reward prevention
- daily reward limit
- purchase CSV import
- duplicate purchase handling
- invalid purchase data
- campaign result merging
- SOAP response handling

SOAP tests use a fake `HttpMessageHandler`, so automated tests do not depend on the availability of the external service.

## Design Notes

SQLite was chosen to keep the project easy to run and review without requiring an external database server.

Customer data is not duplicated locally because the external SOAP service is treated as the source of truth.

The current CSV parser supports the simple format required for this task. A production implementation could use a dedicated CSV library for more complex files.

The authentication implementation is intentionally lightweight for the assignment. A production solution would normally use a proper identity provider and secure user storage.

## Known Limitations

- Runtime customer validation depends on the availability of the external SOAP service.
- Daily reward limit enforcement is implemented in application logic and could require stronger transactional handling under concurrent load.
- Error handling could be centralized instead of being handled individually in controllers.
- SQLite is intended for local development rather than high-scale production use.