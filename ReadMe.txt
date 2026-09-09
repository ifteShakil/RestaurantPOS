RestaurantPOS
===========

RestaurantPOS is a multi-branch restaurant point-of-sale system built with .NET 9, ASP.NET Core Web API, Entity Framework Core, SQL Server and Blazor Server.

1. Solution Architecture
------------------------

RestaurantPOS.sln
├── RestaurantPOS.API
│   └── ASP.NET Core Web API, authentication and business endpoints
├── RestaurantPOS.Core
│   └── Domain entities and repository/unit-of-work interfaces
├── RestaurantPOS.Infrastructure
│   └── EF Core DbContext, repositories, data seeding and migrations
├── RestaurantPOS.Shared
│   └── Shared DTOs used by the API and frontend
├── RestaurantPOS.Web
│   └── Blazor Server frontend with MudBlazor
└── RestaurantPOS.Maui
    └── .NET MAUI project structure exists; application screens are not implemented yet


2. Implemented Domain Models
----------------------------

The system currently supports the following domain areas:

Branch
- Branch name and unique branch code
- Country code
- Currency code and currency symbol
- Configurable VAT/tax rate
- Time zone
- Locale
- Address and phone information
- Active/inactive status

Menu
- Menu categories
- Menu items
- Menu item descriptions and base prices
- Menu item variants with additional prices
- Menu item availability
- Branch-specific menu item prices
- Branch-specific price override support

Restaurant Tables
- Table name
- Capacity
- Branch association
- Available and occupied status management

Users
- Full name and username
- Hashed password storage
- User roles:
  SuperAdmin
  BranchManager
  Cashier
  Waiter
  KitchenStaff
- Optional branch assignment
- Active/inactive status

Orders
- Branch association
- Restaurant table association
- Waiter association
- Order items and quantities
- Menu item variants
- Server-side price lookup
- Order notes
- Order status management
- Cancelled quantity tracking
- Daily ticket number
- Ability to add items to an existing order

Bills and Payments
- Bill generation from an order
- Subtotal calculation
- Branch-specific VAT/tax calculation
- Tax rate snapshot stored on the bill
- Discount amount
- Total amount
- Currency code stored with the bill
- Dynamic payment method support
- Transaction reference support


3. Data Access Layer
--------------------

The application uses the Repository and Unit of Work patterns.

Generic repository interface:
- IGenericRepository<T>

Generic repository operations:
- GetByIdAsync
- GetAllAsync
- AddAsync
- Update
- Delete
- ExistsAsync

Specialized repository interfaces include:
- IBranchRepository
- IOrderRepository
- IMenuItemRepository

The Unit of Work provides:
- Branch repository
- Order repository
- Menu item repository
- Menu category repository
- Bill repository
- Payment repository
- User repository
- Restaurant table repository
- One centralized SaveChangesAsync method


4. DTO and Mapping Layer
------------------------

Entities are not directly exposed through the API.

Shared DTOs are available for:
- Branch
- Menu category
- Menu item
- Menu item variant
- Restaurant table
- Order
- Order item
- Bill
- Payment
- User
- Login

AutoMapper is configured in AppMappingProfile.cs.

The mapping configuration includes:
- Entity-to-response DTO mapping
- Create DTO-to-entity mapping
- Status enum-to-string mapping
- Category name mapping
- Table name mapping
- Waiter name mapping
- Payment method name mapping
- Order total calculation
- User role and branch name mapping


5. API Controllers
------------------

The API includes the following controllers:

BranchController
- Branch CRUD operations
- Country filtering
- Branch information management

MenuCategoryController
- Menu category CRUD operations

MenuController
- Menu item CRUD operations
- Menu item variant support
- Branch-aware pricing
- Availability management

TableController
- Restaurant table CRUD operations
- Table status updates

UserController
- User CRUD operations
- Role assignment
- Password hashing using PasswordHasher

OrderController
- Active order retrieval by branch
- Order details retrieval
- New order creation
- Server-side menu price lookup
- Branch-specific price override
- Variant price calculation
- Order status updates
- Order item cancellation
- Adding items to an existing order
- Automatic table occupation when an order is created

BillController
- Bill generation
- VAT/tax calculation
- Currency snapshot
- Payment recording
- Order status update to Paid
- Automatic table release after payment

DashboardController
- Today's sales summary
- Active order count
- Occupied table count
- Total table count
- Branch currency information


6. Order and Pricing Logic
--------------------------

Order prices are calculated on the server.

The client does not control the final unit price. When an order is created:

1. The menu item is loaded from the database.
2. The base price is selected.
3. A branch-specific price override is applied when available.
4. Variant extra price is added when applicable.
5. The final price is stored in OrderItem.UnitPrice.

This protects order totals from being changed by a client application.


7. Database
-----------

Database technology:
- Microsoft SQL Server
- Entity Framework Core
- EF Core migrations

The DbContext is AppDbContext.

Configured database features:
- Decimal precision for money and tax values
- Unique branch codes
- Unique usernames
- Unique payment method codes
- Unique menu item and branch price combinations
- Restrict delete behavior for important order and user relationships
- Migration-based database creation and updates

The API applies migrations during startup using MigrateAsync.


8. Seed Data
------------

Payment method seed data:
- Cash
- Card
- MTN Mobile Money

Default branch seed data:
- Branch name: Mbabane Main Branch
- Branch code: MBB-01
- Country: Eswatini
- Country code: SZ
- Currency: SZL
- Currency symbol: E
- VAT/tax rate: 15%
- Time zone: Africa/Mbabane
- Locale: en-SZ
- Address: Mbabane, Eswatini

The payment methods are seeded through EF Core HasData().
The default branch is seeded through DataSeeder.


9. Authentication and Authorization Status
------------------------------------------

JWT authentication infrastructure has been added to the API.

Implemented:
- JWT bearer authentication configuration
- Issuer validation
- Audience validation
- Lifetime validation
- Signing key validation
- Authentication middleware
- Authorization middleware
- Protected OrderController endpoints
- Protected BillController endpoints

The following items still require completion or verification:
- Complete login endpoint
- Complete token generation and login flow
- Role-based authorization policies
- Branch-level authorization
- Preventing users from accessing another branch's data
- Frontend token storage and automatic Authorization header handling


10. Blazor Web Frontend
-----------------------

The project contains a Blazor Server frontend using MudBlazor.

Implemented frontend services:
- API HttpClient registration
- ApiService for API communication
- AppState for current branch state
- API base URL configuration

Implemented or partially implemented pages:
- Dashboard
- Orders
- New order workflow
- Table management
- User management
- Shared layout and routing

Dashboard information includes:
- Today's sales
- Active orders
- Occupied tables
- Total tables
- Current branch name

The frontend communicates with the API through the shared DTOs and ApiService.


11. Internationalization
------------------------

The project is designed for multiple branches and international use.

Supported concepts:
- Multiple countries
- Multiple currencies
- Branch-specific currency codes
- Branch-specific currency symbols
- Branch-specific VAT/tax rates
- Branch-specific time zones
- Branch-specific locale codes
- Country-specific payment methods

The current seed configuration is focused on Eswatini and the SZL currency.


12. Current Limitations and Remaining Work
------------------------------------------

1. Complete JWT login flow
   Implement and verify login endpoint, token generation and frontend token handling.

2. Role-based authorization
   Add appropriate role restrictions to API endpoints and frontend pages.

3. Branch data isolation
   Ensure that users can only view and modify data belonging to their assigned branch.

4. Input validation
   Add DTO validation using attributes such as:
   - Required
   - Range
   - StringLength
   - EmailAddress where applicable

5. Global exception handling
   Add middleware that returns a consistent and safe error response instead of exposing stack traces.

6. Payment validation
   Validate payment amount, outstanding balance, payment method and required transaction references.

7. Transaction handling
   Use database transactions for bill generation and payment processing where necessary.

8. Automated testing
   Add unit tests and integration tests for:
   - Authentication
   - Authorization
   - Branch isolation
   - Order creation
   - Price calculation
   - Bill generation
   - Payment processing

9. .NET MAUI application
   Build the MAUI screens for:
   - Login
   - Menu
   - Order creation
   - Order details
   - Payment

10. SignalR kitchen notifications
    Send newly created and updated orders to the kitchen in real time.

11. Production deployment
    Prepare production configuration for:
    - SQL Server
    - API hosting
    - Blazor hosting
    - Secrets and JWT keys
    - Logging
    - HTTPS
    - Database migration strategy


13. Project Status
-----------------

Completed:
- Multi-project solution architecture
- Restaurant domain entities
- Repository and Unit of Work structure
- SQL Server and EF Core integration
- EF Core migrations
- Branch and payment method seed data
- DTO layer
- AutoMapper configuration
- Restaurant CRUD APIs
- Server-side order price calculation
- Bill and payment workflow
- Initial JWT configuration
- Blazor Server dashboard
- Blazor order management foundation
- Blazor table management foundation
- Blazor user management foundation
- MudBlazor UI integration

In progress:
- Complete authentication flow
- Role-based authorization
- Branch-level data isolation
- Validation
- Global error handling
- Automated tests
- Production readiness

Not started or incomplete:
- Full .NET MAUI application
- SignalR kitchen communication
- Complete deployment configuration