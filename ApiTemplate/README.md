# 🚀 ASP.NET Core API Template

A production-ready, enterprise-grade ASP.NET Core Web API template with authentication, authorization, logging, and scalable architecture.

---

## 📋 Table of Contents

- [Features](#-features)
- [Prerequisites](#-prerequisites)
- [Quick Start](#-quick-start)
- [Project Structure](#-project-structure)
- [Database Setup](#-database-setup)
- [Configuration](#️-configuration)
- [Authentication & Authorization](#-authentication--authorization)
- [Generic Repository Pattern](#-generic-repository-pattern) ⭐
- [Logging System](#-logging-system)
- [API Documentation](#-api-documentation)
- [Development Guide](#-development-guide)
- [Deployment](#-deployment)
- [Troubleshooting](#-troubleshooting)

---

## ✨ Features

### Core Features
- ✅ **JWT Authentication** - Secure token-based authentication
- ✅ **Role-Based Access Control (RBAC)** - Granular permission system
- ✅ **Comprehensive Logging** - Serilog with Console, File, and Database sinks
- ✅ **Audit Logging** - Track all security-sensitive operations
- ✅ **Rate Limiting** - Protect API from abuse
- ✅ **API Versioning** - Support multiple API versions
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Email Service** - Built-in email functionality
- ✅ **SignalR Support** - Real-time communication
- ✅ **Repository Pattern** - Clean, maintainable data access
- ✅ **Unit of Work Pattern** - Transaction management
- ✅ **FluentValidation** - Robust input validation
- ✅ **CORS Support** - Cross-origin resource sharing
- ✅ **Docker Support** - Containerized deployment
- ✅ **Performance Monitoring** - Automatic slow request detection

### Architecture Patterns
- **Clean Architecture** - Separation of concerns
- **Dependency Injection** - Loose coupling
- **Generic Repository** - Reusable data access layer
- **Middleware Pipeline** - Request/response processing
- **Factory Pattern** - Repository creation
- **Attribute-based Authorization** - `[RequirePermission]`

---

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

### Required
- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** or higher
- **[SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads)** or SQL Server Express
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** or [VS Code](https://code.visualstudio.com/)

### Optional
- **[Docker Desktop](https://www.docker.com/products/docker-desktop)** - For containerized deployment
- **[Postman](https://www.postman.com/)** or **[Insomnia](https://insomnia.rest/)** - For API testing
- **[SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)** - Database management

### Check Your Installation
```bash
# Check .NET version
dotnet --version

# Check SQL Server (should show version)
sqlcmd -S localhost -Q "SELECT @@VERSION"
```

---

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone <your-repo-url>
cd ApiTemplate
```

### 2. Setup Database

**Option A: Using SQL Server locally**
```bash
# Update connection string in appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ApiTemplateDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**Option B: Using Docker**
```bash
# Start SQL Server container
docker-compose up -d db

# Connection string is already configured in docker-compose.yml
```

### 3. Create Database Schema
```bash
# Open SSMS or Azure Data Studio and connect to your SQL Server
# Run the schema script
cd Shared/SQL
# Execute: table schema.sql

# Then create logging table
# Execute: CreateLoggingTable.sql
```

**OR** use Entity Framework migrations (if available):
```bash
dotnet ef database update
```

### 4. Configure Settings

Edit `ApiTemplate/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  },
  "JWTSetting": {
    "securitykey": "CHANGE_THIS_TO_A_LONG_RANDOM_STRING_AT_LEAST_32_CHARS"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "Your App Name"
  }
}
```

⚠️ **IMPORTANT:** Never commit `appsettings.json` with real credentials. Use User Secrets or Environment Variables in production.

### 5. Build & Run
```bash
# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the API
cd ApiTemplate
dotnet run
```

The API will be available at:
- **HTTP:** http://localhost:8080
- **HTTPS:** https://localhost:8081
- **Swagger UI:** http://localhost:8080/swagger

### 6. Test the API

**Using Swagger:**
1. Navigate to http://localhost:8080/swagger
2. Try the `/api/v1/auth/register` endpoint to create a user
3. Use `/api/v1/auth/login` to get a JWT token
4. Click "Authorize" button and enter: `Bearer YOUR_TOKEN`
5. Now you can test protected endpoints

**Using cURL:**
```bash
# Register a new user
curl -X POST http://localhost:8080/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test@123",
    "firstname": "Test",
    "lastname": "User"
  }'

# Login
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test@123"
  }'
```

---

## 📁 Project Structure

```
ApiTemplate/
│
├── ApiTemplate/                    # Main API Project
│   ├── Controllers/                # API Controllers
│   │   ├── AuthController.cs      # Authentication endpoints
│   │   ├── UserManagerController.cs # User management
│   │   └── ...
│   ├── Service/                    # Service configurations
│   │   └── GeneralDIContainer.cs  # Dependency injection setup
│   ├── Program.cs                  # Application entry point
│   ├── appsettings.json           # Configuration
│   └── Dockerfile                  # Docker configuration
│
├── DBLayer/                        # Database Layer
│   ├── Models/                     # Entity models
│   │   ├── TblUser.cs
│   │   ├── TblRole.cs
│   │   ├── TblPermission.cs
│   │   └── ...
│   └── DbContextDI.cs             # EF Core context
│
├── Repositories/                   # Data Access Layer
│   ├── Repository/
│   │   ├── GenericRepository.cs   # Base repository
│   │   ├── AuthRepository.cs      # Auth-specific operations
│   │   ├── UnitOfWork.cs         # Transaction management
│   │   └── ...
│   ├── Services/
│   │   ├── AuthorizationService.cs
│   │   └── TableOperationService.cs
│   └── Attributes/
│       └── RequirePermissionAttribute.cs
│
├── Shared/                         # Shared Components
│   ├── Dtos/                       # Data Transfer Objects
│   ├── Services/                   # Shared services
│   │   ├── LoggingConfiguration.cs
│   │   ├── AuditLoggingService.cs
│   │   ├── EmailService.cs
│   │   └── ...
│   ├── Middleware/                 # Custom middleware
│   │   ├── RequestResponseLoggingMiddleware.cs
│   │   ├── PerformanceMonitoringMiddleware.cs
│   │   ├── AuthMiddleware.cs
│   │   └── ...
│   ├── Pipline/
│   │   └── ApplicationPipelineExtensions.cs
│   └── SQL/                        # Database scripts
│       ├── table schema.sql
│       └── CreateLoggingTable.sql
│
├── UnitTest/                       # Unit tests
├── docker-compose.yml              # Docker Compose configuration
└── README.md                       # This file
```

---

## 🗄️ Database Setup

### Database Schema Overview

The application uses a comprehensive permission-based authorization system:

#### Core Tables
- **tblUsers** - User accounts
- **tblRole** - Roles (Admin, HR, Manager, etc.)
- **tblResource** - Protected resources (Employee, Invoice, etc.)
- **tblActionType** - Actions (View, Add, Edit, Delete)
- **tblPermission** - Resource + ActionType combinations
- **tblRolePermission** - Permissions assigned to roles
- **tblUserRole** - Roles assigned to users
- **tblResetToken** - Password reset and JWT refresh tokens
- **ApplicationLogs** - Application logging

### Setup Instructions

**Step 1: Create Database**
```sql
CREATE DATABASE ApiTemplateDB;
GO
USE ApiTemplateDB;
GO
```

**Step 2: Run Schema Script**
```bash
# Location: Shared/SQL/table schema.sql
# Execute this in SSMS or Azure Data Studio
```

**Step 3: Create Logging Table**
```bash
# Location: Shared/SQL/CreateLoggingTable.sql
# Execute this to enable database logging
```

**Step 4: Seed Initial Data**
```sql
-- Insert Action Types
INSERT INTO tblActionType (ActionTypeID, ActionTypeTitle) VALUES
(1, 'view'),
(2, 'add'),
(3, 'edit'),
(4, 'delete');

-- Insert a default Admin role
INSERT INTO tblRole (RoleID, RoleTitle) VALUES
(1, 'Admin');

-- Insert UserManagement resource
INSERT INTO tblResource (ResourceID, ResourceName) VALUES
(1, 'UserManagement');

-- Create permissions for UserManagement
INSERT INTO tblPermission (PermissionID, ResourceID, ActionTypeID) VALUES
(1, 1, 1), -- View UserManagement
(2, 1, 2), -- Add UserManagement
(3, 1, 3), -- Edit UserManagement
(4, 1, 4); -- Delete UserManagement

-- Assign all permissions to Admin role
INSERT INTO tblRolePermission (RoleID, PermissionID) VALUES
(1, 1), (1, 2), (1, 3), (1, 4);
```

### Using Docker

```bash
# Start SQL Server in Docker
docker-compose up -d

# The database will be available at:
# Server: localhost,1433
# Database: test2
# User: sa
# Password: YourStrongPassword!
```

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ApiTemplateDB;Trusted_Connection=True;"
  },

  "JWTSetting": {
    "ValidAudience": "User",
    "ValidIssuer": "Api",
    "securitykey": "your-secret-key-minimum-32-characters-long"
  },

  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },

  "Serilog": {
    "FilePath": "Logs/log-.json",
    "TextFilePath": "Logs/log-.txt"
  },

  "RateLimiting": {
    "PerIPPolicy": {
      "PermitLimit": 100,
      "WindowMinutes": 1
    }
  },

  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200"
    ]
  }
}
```

### Environment Variables (Production)

For production, use environment variables instead of `appsettings.json`:

```bash
# Connection String
export ConnectionStrings__DefaultConnection="Server=...;Database=...;User Id=...;Password=..."

# JWT Secret
export JWTSetting__securitykey="your-production-secret-key"

# Email Settings
export EmailSettings__Username="your-email@gmail.com"
export EmailSettings__Password="your-app-password"
```

### User Secrets (Development)

```bash
# Initialize user secrets
dotnet user-secrets init --project ApiTemplate

# Set secrets
dotnet user-secrets set "JWTSetting:securitykey" "your-secret-key" --project ApiTemplate
dotnet user-secrets set "EmailSettings:Password" "your-password" --project ApiTemplate
```

---

## 🔐 Authentication & Authorization

### How It Works

This API uses a **hierarchical permission system**:

```
User → UserRole → Role → RolePermission → Permission → (Resource + ActionType)
```

**Example Flow:**
1. User "John" has role "HR"
2. "HR" role has permission "View Employee"
3. "View Employee" = Resource("Employee") + Action("View")
4. John can view employees but not delete them

### API Endpoints

#### Authentication

**Register**
```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass@123",
  "firstname": "John",
  "lastname": "Doe",
  "roles": [
    { "roleId": 1 }
  ]
}
```

**Login**
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "username": "johndoe",
  "password": "SecurePass@123"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "loginDate": "2025-10-16T10:30:00Z",
  "roles": ["Admin"]
}
```

**Change Password**
```http
POST /api/v1/auth/change-password
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "currentPassword": "OldPass@123",
  "newPassword": "NewPass@456"
}
```

**Forgot Password**
```http
POST /api/v1/auth/forgot-password
Content-Type: application/json

{
  "email": "john@example.com"
}
```

**Reset Password**
```http
POST /api/v1/auth/reset-password
Content-Type: application/json

{
  "email": "john@example.com",
  "resetToken": "token-from-email",
  "newPassword": "NewPass@789"
}
```

#### User Management (Requires Permissions)

**Create Resource with Permissions**
```http
POST /api/v1/usermanager/resources
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "resourceName": "Salary",
  "actionTypeIds": [1, 2, 3, 4],
  "rolePermissions": {
    "1": [1, 2, 3, 4],  // Admin gets all
    "2": [1]             // User gets view only
  }
}
```

**Assign Role to User**
```http
POST /api/v1/usermanager/users/assign-role
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "userId": 123,
  "roleId": 2
}
```

### Using JWT Tokens

**In Swagger:**
1. Click the "Authorize" button (🔒)
2. Enter: `Bearer YOUR_JWT_TOKEN`
3. Click "Authorize"

**In Postman:**
- Add header: `Authorization: Bearer YOUR_JWT_TOKEN`

**In cURL:**
```bash
curl -H "Authorization: Bearer YOUR_JWT_TOKEN" http://localhost:8080/api/v1/protected-endpoint
```

### Creating Protected Endpoints

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class SalaryController : ControllerBase
{
    // Only users with "View Salary" permission can access
    [HttpGet]
    [RequirePermission("Salary", "view")]
    public IActionResult GetSalaries()
    {
        // Your code here
        return Ok(salaries);
    }

    // Only users with "Add Salary" permission can access
    [HttpPost]
    [RequirePermission("Salary", "add")]
    public IActionResult CreateSalary([FromBody] SalaryDto salary)
    {
        // Your code here
        return Ok();
    }
}
```

---

## 🏗️ Generic Repository Pattern

This is the **heart of the codebase** - a powerful, flexible data access layer that supports both Entity Framework Core and Dapper.

### Quick Overview

The Generic Repository pattern provides:
- ✅ **Dual ORM Support** - Switch between EF Core and Dapper at runtime
- ✅ **Automatic Caching** - Built-in performance optimization
- ✅ **Transaction Management** - Coordinated EF + Dapper transactions
- ✅ **Auto-Discovery** - Repositories automatically registered via attributes
- ✅ **Dependency Injection** - Services resolved only when needed

### Three Ways to Access Data

#### 1. Direct Generic Access (Simple CRUD)

Perfect for basic operations without custom logic:

```csharp
public class ProductController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Get generic repository for any entity
        var productRepo = _unitOfWork.Repository<Product>();

        // EF Core with automatic caching
        var products = await productRepo.GetAllAsync();

        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        var productRepo = _unitOfWork.Repository<Product>();

        await productRepo.AddAsync(product);
        await _unitOfWork.SaveAsync();  // EF: Save changes

        return Ok();
    }
}
```

#### 2. ORM Wrapper (Choose EF or Dapper)

Flexibility to choose the best ORM for each operation:

```csharp
[HttpGet("paged")]
public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
{
    var productRepo = _unitOfWork.RepositoryWrapper<Product>();

    // Option 1: Entity Framework (with navigation properties)
    var productsEF = await productRepo.GetAllAsync(OrmType.EntityFramework);

    // Option 2: Dapper (faster for large datasets)
    var productsDapper = await productRepo.GetPagedAsync(
        page,
        pageSize,
        OrmType.Dapper,
        new CrudOptions
        {
            TableName = "Products",
            OrderBy = "ProductId"
        }
    );

    return Ok(productsDapper);
}
```

#### 3. Custom Repositories (Complex Business Logic)

For operations requiring custom logic, multiple services, or complex queries:

```csharp
// Step 1: Create Interface
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<Product?> GetProductWithDetailsAsync(int productId);
}

// Step 2: Implement Repository
[AutoRegisterRepository(typeof(IProductRepository))]  // ← Auto-discovered!
public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    private readonly TestContext _context;
    private readonly IUnitOfWork _unitOfWork;

    // Lazy-load services only when needed
    private IEmailService? _emailService;
    private IEmailService EmailService => _emailService ??= GetService<IEmailService>();

    public ProductRepository(
        TestContext context,
        IDbConnection connection,
        IMemoryCache cache,
        IDbTransaction? transaction,
        IUnitOfWork unitOfWork,
        IServiceProvider serviceProvider)  // ← Enables GetService<T>()
        : base(context, connection, cache, transaction, serviceProvider)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        // Complex EF Core query with includes
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithDetailsAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.OrderDetails)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        // Use additional services
        if (product != null && product.Stock < 10)
        {
            await EmailService.SendEmailAsync(new EmailDto
            {
                To = "admin@example.com",
                Subject = "Low Stock Alert",
                Body = $"Product {product.Name} is running low on stock"
            });
        }

        return product;
    }
}

// Step 3: Use in Controller
[HttpGet("category/{categoryId}")]
public async Task<IActionResult> GetByCategory(int categoryId)
{
    var productRepo = _unitOfWork.GetRepository<IProductRepository>();
    var products = await productRepo.GetProductsByCategoryAsync(categoryId);
    return Ok(products);
}
```

### Understanding the Architecture

```
UnitOfWork (Single entry point)
    │
    ├── Repository<T>()           → GenericRepository → Direct CRUD
    ├── RepositoryWrapper<T>()    → GenericRepository → Choose ORM at runtime
    └── GetRepository<ICustom>()  → BaseRepository    → Custom logic + DI
            │
            └── GetService<T>()   → Resolve additional dependencies
                    │
                    ├── IEmailService
                    ├── ILogger<T>
                    ├── IAuditLoggingService
                    └── Any other service
```

### Transaction Management

```csharp
public async Task<IActionResult> CreateOrderWithItems([FromBody] OrderDto orderDto)
{
    try
    {
        // Start transaction
        await _unitOfWork.BeginTransactionAsync();

        var orderRepo = _unitOfWork.Repository<Order>();
        var itemRepo = _unitOfWork.Repository<OrderItem>();

        // Create order
        var order = new Order { /* ... */ };
        await orderRepo.AddAsync(order);
        await _unitOfWork.SaveAsync();

        // Create items
        foreach (var item in orderDto.Items)
        {
            await itemRepo.AddAsync(new OrderItem { /* ... */ });
        }
        await _unitOfWork.SaveAsync();

        // Commit both operations
        await _unitOfWork.CommitAsync();

        return Ok(order);
    }
    catch (Exception ex)
    {
        // Rollback on error
        await _unitOfWork.RollbackAsync();
        return StatusCode(500, ex.Message);
    }
}
```

### Key Benefits

| Feature | Benefit |
|---------|---------|
| **BaseRepository** | Provides `GetService<T>()` for dependency injection |
| **UnitOfWork** | Single entry point, manages all repositories |
| **Auto-Discovery** | `[AutoRegisterRepository]` attribute auto-registers repos |
| **Dual ORM** | Use EF Core for complex queries, Dapper for performance |
| **Caching** | Automatic 5-10 minute cache for reads |
| **Transactions** | Coordinate multiple operations atomically |

### 📖 Complete Guide

For detailed information including:
- How dependencies flow through the system
- Creating complex custom repositories
- Mixing EF Core and Dapper in single operations
- Best practices and common patterns
- Advanced examples and troubleshooting

**See:** [GENERIC_REPOSITORY_GUIDE.md](./GENERIC_REPOSITORY_GUIDE.md) - Complete 50+ page guide

---

## 📊 Logging System

The API includes a comprehensive logging system powered by **Serilog**.

### Log Destinations

1. **Console** - Real-time logs during development
2. **JSON Files** - `Logs/log-YYYYMMDD.json` (30 days retention)
3. **Text Files** - `Logs/log-YYYYMMDD.txt` (7 days retention)
4. **Database** - `ApplicationLogs` table in SQL Server

### What Gets Logged Automatically

✅ All HTTP requests and responses
✅ Performance metrics (slow request warnings)
✅ Exceptions and errors
✅ User authentication events
✅ Security events (failed logins, unauthorized access)
✅ Application startup/shutdown

### Viewing Logs

**Console (Development):**
```
[10:30:15 INF] HTTP Request abc123 | POST /api/auth/login | IP: 127.0.0.1
[10:30:15 INF] AUDIT: Successful login | User: john.doe | IP: 127.0.0.1
[10:30:15 INF] HTTP Response abc123 | Status: 200 | Duration: 123ms
```

**Database Query:**
```sql
-- View recent logs
SELECT TOP 50 * FROM ApplicationLogs
ORDER BY TimeStamp DESC;

-- View audit logs only
SELECT * FROM AuditLogs
WHERE TimeStamp > DATEADD(DAY, -7, GETDATE())
ORDER BY TimeStamp DESC;

-- View failed login attempts
SELECT TimeStamp, UserName, IPAddress, Message
FROM ApplicationLogs
WHERE Message LIKE '%Failed login%'
ORDER BY TimeStamp DESC;
```

**JSON File:**
```json
{
  "@t": "2025-10-16T10:30:15.1234567Z",
  "@mt": "HTTP Request {RequestId} | {Method} {Path}",
  "RequestId": "abc123",
  "Method": "POST",
  "Path": "/api/auth/login",
  "IPAddress": "127.0.0.1"
}
```

### Adding Logging to Your Code

```csharp
public class MyController : ControllerBase
{
    private readonly ILogger<MyController> _logger;
    private readonly IAuditLoggingService _auditLogger;

    public MyController(ILogger<MyController> logger, IAuditLoggingService auditLogger)
    {
        _logger = logger;
        _auditLogger = auditLogger;
    }

    [HttpPost]
    public async Task<IActionResult> MyAction()
    {
        _logger.LogInformation("Processing action for user {UserId}", userId);

        try
        {
            // Your code
            await _auditLogger.LogSecurityEvent("ActionPerformed", "User did something important");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process action for user {UserId}", userId);
            return StatusCode(500);
        }
    }
}
```

### Log Cleanup

```sql
-- Clean logs older than 90 days
EXEC sp_CleanOldLogs @RetentionDays = 90;
```

For more details, see [LOGGING_DOCUMENTATION.md](./LOGGING_DOCUMENTATION.md)

---

## 📚 API Documentation

### Swagger UI

Access interactive API documentation at: **http://localhost:8080/swagger**

Features:
- ✅ Try API endpoints directly from browser
- ✅ View request/response schemas
- ✅ Test authentication with JWT tokens
- ✅ See all available endpoints

### API Versioning

The API supports versioning via URL:

```
/api/v1/auth/login     # Version 1
/api/v2/auth/login     # Version 2 (future)
```

Current version: **v1.0**

### Response Format

All API responses follow this format:

**Success Response:**
```json
{
  "statusCode": 200,
  "message": "Success",
  "data": {
    // Your data here
  }
}
```

**Error Response:**
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "data": null
}
```

### Rate Limiting

API endpoints are rate-limited to prevent abuse:

- **Per IP:** 100 requests per minute
- **Login/Register:** 10 requests per minute
- **Authenticated:** 50 requests per minute

When rate limit is exceeded:
```json
{
  "statusCode": 429,
  "message": "Too many requests. Please try again later."
}
```

---

## 💻 Development Guide

### Adding a New Controller

**1. Create Controller:**
```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [RequirePermission("Product", "view")]
    public async Task<IActionResult> GetProducts()
    {
        _logger.LogInformation("Fetching all products");
        // Your code
        return Ok(products);
    }
}
```

### Adding a New Repository

**1. Create Interface:**
```csharp
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<int> AddAsync(Product product);
}
```

**2. Implement Repository:**
```csharp
[AutoRegisterRepository(typeof(IProductRepository))]
public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    private readonly TestContext _context;

    public ProductRepository(TestContext context, /* other dependencies */)
        : base(context, /* other dependencies */)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }
}
```

The `[AutoRegisterRepository]` attribute automatically registers it in DI.

### Adding a New Permission-Protected Resource

**1. Add Resource to Database:**
```sql
INSERT INTO tblResource (ResourceID, ResourceName) VALUES (10, 'Product');
```

**2. Create Permissions:**
```sql
INSERT INTO tblPermission (PermissionID, ResourceID, ActionTypeID) VALUES
(40, 10, 1),  -- View Product
(41, 10, 2),  -- Add Product
(42, 10, 3),  -- Edit Product
(43, 10, 4);  -- Delete Product
```

**3. Assign to Role:**
```sql
INSERT INTO tblRolePermission (RoleID, PermissionID) VALUES
(1, 40), (1, 41), (1, 42), (1, 43);  -- Admin gets all
```

**4. Use in Controller:**
```csharp
[HttpGet]
[RequirePermission("Product", "view")]
public IActionResult GetProducts() { }
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Building for Production

```bash
# Publish release build
dotnet publish -c Release -o ./publish

# The output will be in ./publish directory
```

---

## 🐳 Deployment

### Using Docker

**Build and Run:**
```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

The API will be available at http://localhost:8080

**Environment Variables in Docker:**
```yaml
# docker-compose.yml
services:
  api:
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=test2;...
      - JWTSetting__securitykey=your-production-key
      - ASPNETCORE_ENVIRONMENT=Production
```

### Deploying to Azure

**1. Create Azure SQL Database**
```bash
az sql server create --name myserver --resource-group myResourceGroup
az sql db create --name ApiTemplateDB --server myserver
```

**2. Create App Service**
```bash
az webapp create --name myapi --plan myPlan --resource-group myResourceGroup
```

**3. Configure App Settings**
```bash
az webapp config appsettings set --name myapi --settings \
  ConnectionStrings__DefaultConnection="..." \
  JWTSetting__securitykey="..."
```

**4. Deploy**
```bash
dotnet publish -c Release
cd bin/Release/net8.0/publish
zip -r deploy.zip .
az webapp deployment source config-zip --name myapi --src deploy.zip
```

### Deploying to IIS

**1. Install .NET Hosting Bundle**
- Download from: https://dotnet.microsoft.com/download/dotnet/8.0

**2. Publish Application**
```bash
dotnet publish -c Release -o C:\inetpub\wwwroot\myapi
```

**3. Configure IIS**
- Create Application Pool (.NET CLR Version: No Managed Code)
- Create Website pointing to publish folder
- Set Application Pool

**4. Configure web.config**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" />
    </handlers>
    <aspNetCore processPath="dotnet"
                arguments=".\ApiTemplate.dll"
                stdoutLogEnabled="true"
                stdoutLogFile=".\logs\stdout" />
  </system.webServer>
</configuration>
```

---

## 🔧 Troubleshooting

### Common Issues

#### "Unable to connect to database"
```bash
# Check SQL Server is running
docker ps  # or
Get-Service MSSQLSERVER

# Test connection
sqlcmd -S localhost -U sa -P YourStrongPassword!
```

#### "JWT token validation failed"
- Check that `securitykey` in `appsettings.json` is at least 32 characters
- Ensure you're using `Bearer TOKEN` format (note the space)
- Check token hasn't expired (10 hours default)

#### "Permission denied" on endpoints
- Ensure user has been assigned the correct role
- Check that role has the required permissions
- Use audit logs to see what permissions user has:
```sql
SELECT * FROM AuditLogs WHERE UserName = 'john.doe'
```

#### "Logs not appearing"
- Check `Logs/` directory exists and is writable
- Verify connection string for database logging
- Check log level in `appsettings.json`

#### "Rate limit exceeded"
- Wait for the time window to reset (1 minute)
- Adjust rate limits in `appsettings.json` > `RateLimiting`

#### Port already in use
```bash
# Change ports in launchSettings.json or
dotnet run --urls "http://localhost:5000;https://localhost:5001"
```

### Debug Mode

```bash
# Run with detailed logging
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run

# Enable SQL logging
```

### Getting Help

1. Check logs in `Logs/` directory
2. Check database `ApplicationLogs` table
3. Review Swagger documentation
4. Check existing issues in repository

---

## 📖 Additional Documentation

### Core Documentation
- **[GENERIC_REPOSITORY_GUIDE.md](./GENERIC_REPOSITORY_GUIDE.md)** - ⭐ Complete guide to the repository pattern (50+ pages)
  - Understanding the architecture
  - Creating custom repositories
  - Using UnitOfWork and dependency injection
  - Best practices and advanced examples

### Logging Documentation
- [LOGGING_DOCUMENTATION.md](./LOGGING_DOCUMENTATION.md) - Complete logging system guide
- [LOGGING_QUICK_START.md](./LOGGING_QUICK_START.md) - Quick start for logging
- [LOGGING_REFACTORING_SUMMARY.md](./LOGGING_REFACTORING_SUMMARY.md) - Logging architecture
- [FINAL_REFACTORING_SUMMARY.md](./FINAL_REFACTORING_SUMMARY.md) - Overall architecture details

---

## 🎯 Next Steps

After setting up the project:

1. ✅ **Create your first user** via `/api/v1/auth/register`
2. ✅ **Login and get JWT token** via `/api/v1/auth/login`
3. ✅ **Explore Swagger UI** at http://localhost:8080/swagger
4. ✅ **Check logs** in `Logs/` directory and database
5. ✅ **Create custom resources** via User Manager endpoints
6. ✅ **Build your first controller** following the development guide
7. ✅ **Setup CI/CD** for automated deployments

---

## 🤝 Contributing

Feel free to customize and extend this template for your needs:

- Add new controllers and repositories
- Extend the permission system
- Add new middleware
- Integrate with external services
- Improve documentation

---

## 📝 License

This is a template project. Use it as you wish for your own projects.

---

## 💡 Tips

- **Security:** Always use HTTPS in production
- **Passwords:** Use strong, random JWT secrets
- **Logging:** Review logs regularly for security issues
- **Backups:** Schedule regular database backups
- **Testing:** Write tests for critical business logic
- **Documentation:** Keep this README updated with your changes

---

## 🎉 You're All Set!

Your API template is ready to use. Start building amazing APIs! 🚀

For questions or issues, refer to the troubleshooting section or check the additional documentation files.

**Happy Coding!** 👨‍💻👩‍💻
