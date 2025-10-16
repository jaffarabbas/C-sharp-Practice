# 🏗️ Generic Repository Pattern - Complete Guide

## Table of Contents
- [Overview](#overview)
- [Architecture](#architecture)
- [Core Components](#core-components)
- [Using Generic Repository](#using-generic-repository)
- [Creating Custom Repositories](#creating-custom-repositories)
- [UnitOfWork Pattern](#unitofwork-pattern)
- [Dependency Injection](#dependency-injection)
- [Advanced Examples](#advanced-examples)
- [Best Practices](#best-practices)

---

## Overview

This codebase implements a **powerful Generic Repository pattern** that supports:

✅ **Both Entity Framework Core and Dapper**
✅ **Automatic caching** for performance
✅ **Transaction management** (EF and Dapper transactions)
✅ **Auto-discovery** of repositories via attributes
✅ **Dependency injection** through BaseRepository
✅ **ORM-agnostic CRUD operations**

### The Three-Layer Architecture

```
┌─────────────────────────────────────────────────────┐
│                  Your Controller                    │
│            (AuthController, ProductController)      │
└──────────────────────┬──────────────────────────────┘
                       │ uses
                       ▼
┌─────────────────────────────────────────────────────┐
│                    UnitOfWork                       │
│  - Manages all repositories                         │
│  - Handles transactions (EF + Dapper)               │
│  - Single entry point for data access               │
└──────────────────────┬──────────────────────────────┘
                       │ creates
                       ▼
┌─────────────────────────────────────────────────────┐
│         BaseRepository / GenericRepository          │
│  - Provides CRUD operations                         │
│  - Supports EF Core and Dapper                      │
│  - Built-in caching                                 │
│  - Dependency injection support                     │
└──────────────────────┬──────────────────────────────┘
                       │ accesses
                       ▼
┌─────────────────────────────────────────────────────┐
│                    Database                         │
│  - SQL Server via EF Core or Dapper                 │
└─────────────────────────────────────────────────────┘
```

---

## Architecture

### Component Hierarchy

```
GenericRepository<T>           (Base - Provides CRUD for any entity)
        │
        ├── Supports EF Core (TestContext)
        ├── Supports Dapper (IDbConnection)
        └── Built-in caching (IMemoryCache)

BaseRepository<T>              (Extends GenericRepository)
        │
        └── Adds IServiceProvider for DI

CustomRepository               (Your specific repository)
        │
        └── Inherits BaseRepository
        └── Marked with [AutoRegisterRepository]

UnitOfWork                     (Orchestrates everything)
        │
        ├── Creates repositories via Factory
        ├── Manages transactions
        └── Single entry point for data access
```

---

## Core Components

### 1. GenericRepository<T>

The **heart of the system**. Provides CRUD operations for any entity.

**Location:** `Repositories/Repository/GenericRepository.cs`

**Key Features:**
- ✅ Dual ORM support (EF Core + Dapper)
- ✅ Built-in caching (5-10 minute TTL)
- ✅ Pagination support
- ✅ Transaction-aware
- ✅ Auto-generates SQL for Dapper

**Constructor:**
```csharp
public GenericRepository(
    TestContext context,           // EF Core DbContext
    IDbConnection connection,      // Dapper connection
    IMemoryCache cache,            // Caching
    IDbTransaction? transaction)   // Transaction support
```

### 2. BaseRepository<T>

Extends `GenericRepository` with **dependency injection capabilities**.

**Location:** `Repositories/Repository/BaseRepository.cs`

**Why It Exists:**
- Your custom repositories need additional services (email, logging, etc.)
- BaseRepository provides `GetService<T>()` to resolve dependencies
- Keeps UnitOfWork clean - services resolved only when needed

**Constructor:**
```csharp
protected BaseRepository(
    TestContext context,
    IDbConnection connection,
    IMemoryCache cache,
    IDbTransaction? transaction,
    IServiceProvider serviceProvider)  // ← Provides DI
    : base(context, connection, cache, transaction)
```

### 3. UnitOfWork

The **orchestrator** that manages all repositories and transactions.

**Location:** `Repositories/Repository/UnitOfWork.cs`

**Key Features:**
- ✅ Single entry point for all data access
- ✅ Manages both EF and Dapper transactions
- ✅ Auto-discovers repositories via attributes
- ✅ Lazy loads repositories (created only when needed)
- ✅ Handles infrastructure dependencies internally

**Constructor:**
```csharp
public UnitOfWork(IServiceProvider serviceProvider)  // Only 1 parameter!
```

### 4. Infrastructure Context

Consolidates all infrastructure dependencies in one place.

**Location:** `Repositories/Infrastructure/InfrastructureContext.cs`

**What It Contains:**
```csharp
- TestContext (EF Core DbContext)
- IDbConnection (Dapper connection)
- IDbTransaction (Dapper transaction)
- IMemoryCache (Caching)
- IServiceProvider (Dependency injection)
- JWTSettings (Configuration)
- IUnitOfWork (Reference to UnitOfWork)
```

---

## Using Generic Repository

### Method 1: Direct Generic Access (Simple CRUD)

Use when you don't need custom logic - just basic CRUD operations.

```csharp
public class ProductController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        // Get generic repository for Product entity
        var productRepo = _unitOfWork.Repository<Product>();

        // EF Core - with caching
        var products = await productRepo.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var productRepo = _unitOfWork.Repository<Product>();

        // EF Core - cached for 10 minutes
        var product = await productRepo.GetByIdAsync(id);

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        var productRepo = _unitOfWork.Repository<Product>();

        // EF Core - invalidates cache
        await productRepo.AddAsync(product);
        await _unitOfWork.SaveAsync();  // Commit to database

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] Product product)
    {
        var productRepo = _unitOfWork.Repository<Product>();

        productRepo.Update(product);
        await _unitOfWork.SaveAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var productRepo = _unitOfWork.Repository<Product>();

        var product = await productRepo.GetByIdAsync(id);
        if (product != null)
        {
            productRepo.Delete(product);
            await _unitOfWork.SaveAsync();
        }

        return Ok();
    }
}
```

### Method 2: Using ORM Wrapper (Choose EF or Dapper)

The `RepositoryWrapper<T>` allows you to **choose between EF Core and Dapper at runtime**.

```csharp
public async Task<IActionResult> GetProductsFlexible()
{
    var productRepo = _unitOfWork.RepositoryWrapper<Product>();

    // Option 1: Use Entity Framework
    var productsEF = await productRepo.GetAllAsync(
        OrmType.EntityFramework
    );

    // Option 2: Use Dapper (faster for read-only)
    var productsDapper = await productRepo.GetAllAsync(
        OrmType.Dapper,
        new CrudOptions
        {
            TableName = "Products"
        }
    );

    return Ok(productsEF);
}

[HttpGet("paged")]
public async Task<IActionResult> GetPagedProducts(int page = 1, int pageSize = 10)
{
    var productRepo = _unitOfWork.RepositoryWrapper<Product>();

    // Dapper pagination (faster for large datasets)
    var pagedResult = await productRepo.GetPagedAsync(
        page,
        pageSize,
        OrmType.Dapper,
        new CrudOptions
        {
            TableName = "Products",
            OrderBy = "ProductId"  // Required for SQL Server
        }
    );

    return Ok(new
    {
        Items = pagedResult.Items,
        Total = pagedResult.TotalCount,
        Page = pagedResult.PageNumber,
        PageSize = pagedResult.PageSize
    });
}
```

### Method 3: Using Custom Repositories (Complex Logic)

When you need custom business logic, authentication, or additional services.

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
{
    // Get custom repository with auto-discovery
    var authRepo = _unitOfWork.GetRepository<IAuthRepository>();

    // Custom method with complex logic (hashing, JWT, email)
    var result = await authRepo.LoginAsync(loginDto);

    return Ok(result);
}
```

---

## Creating Custom Repositories

### Step 1: Define Interface

```csharp
// Repositories/Repository/IProductRepository.cs
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<Product?> GetProductWithDetailsAsync(int productId);
    Task<bool> IsProductInStockAsync(int productId);
}
```

### Step 2: Implement Repository (Inherit BaseRepository)

```csharp
// Repositories/Repository/ProductRepository.cs
using Repositories.Attributes;

[AutoRegisterRepository(typeof(IProductRepository))]  // ← Auto-discovery!
public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    private readonly TestContext _context;
    private readonly IUnitOfWork _unitOfWork;

    // Lazy-loaded services (only resolved when needed)
    private IEmailService? _emailService;
    private IEmailService EmailService => _emailService ??= GetService<IEmailService>();

    private ILogger<ProductRepository>? _logger;
    private ILogger<ProductRepository> Logger => _logger ??= GetService<ILogger<ProductRepository>>();

    public ProductRepository(
        TestContext context,
        IDbConnection connection,
        IMemoryCache cache,
        IDbTransaction? transaction,
        IUnitOfWork unitOfWork,
        IServiceProvider serviceProvider)  // ← For GetService<T>()
        : base(context, connection, cache, transaction, serviceProvider)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        Logger.LogInformation("Fetching products for category {CategoryId}", categoryId);

        // Use EF Core with navigation properties
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithDetailsAsync(int productId)
    {
        // Complex query with multiple includes
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.OrderDetails)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
            return null;

        // Use additional service
        if (product.Stock < 10)
        {
            await EmailService.SendEmailAsync(new EmailDto
            {
                To = "admin@example.com",
                Subject = "Low Stock Alert",
                Body = $"Product {product.Name} is low on stock"
            });
        }

        return product;
    }

    public async Task<bool> IsProductInStockAsync(int productId)
    {
        // Use Dapper for faster read
        var sql = "SELECT CASE WHEN Stock > 0 THEN 1 ELSE 0 END FROM Products WHERE ProductId = @ProductId";
        var result = await Connection.ExecuteScalarAsync<bool>(sql, new { ProductId = productId }, Transaction);
        return result;
    }
}
```

### Step 3: Use in Controller

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var productRepo = _unitOfWork.GetRepository<IProductRepository>();
        var products = await productRepo.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var productRepo = _unitOfWork.GetRepository<IProductRepository>();
        var product = await productRepo.GetProductWithDetailsAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }
}
```

---

## UnitOfWork Pattern

### Understanding UnitOfWork

**Purpose:** Single entry point for all data access operations.

**Benefits:**
- ✅ Centralized transaction management
- ✅ Coordinates multiple repositories
- ✅ Ensures data consistency
- ✅ Simplifies testing

### Transaction Management

#### EF Core Transactions

```csharp
public async Task<IActionResult> CreateOrderWithItems([FromBody] OrderDto orderDto)
{
    try
    {
        // Start EF transaction
        await _unitOfWork.BeginTransactionAsync();

        var orderRepo = _unitOfWork.Repository<Order>();
        var orderItemRepo = _unitOfWork.Repository<OrderItem>();

        // Create order
        var order = new Order { /* ... */ };
        await orderRepo.AddAsync(order);
        await _unitOfWork.SaveAsync();

        // Create order items
        foreach (var item in orderDto.Items)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                /* ... */
            };
            await orderItemRepo.AddAsync(orderItem);
        }
        await _unitOfWork.SaveAsync();

        // Commit transaction
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

#### Dapper Transactions

```csharp
public async Task<IActionResult> CreateProductDapper([FromBody] ProductDto productDto)
{
    try
    {
        var connection = _unitOfWork.Connection;
        var transaction = _unitOfWork.Transaction;

        // Insert product
        var sql = "INSERT INTO Products (Name, Price) VALUES (@Name, @Price)";
        await connection.ExecuteAsync(sql, productDto, transaction);

        // Insert audit log
        var auditSql = "INSERT INTO AuditLog (Action, Date) VALUES (@Action, @Date)";
        await connection.ExecuteAsync(auditSql,
            new { Action = "Product Created", Date = DateTime.UtcNow },
            transaction);

        // Commit
        _unitOfWork.Commit();

        return Ok();
    }
    catch
    {
        // Rollback
        _unitOfWork.Rollback();
        throw;
    }
}
```

### Mixing EF and Dapper in Single Operation

```csharp
public async Task<IActionResult> ComplexOperation()
{
    try
    {
        // Start both transactions
        await _unitOfWork.BeginTransactionAsync();

        // Use EF Core for complex entity
        var orderRepo = _unitOfWork.Repository<Order>();
        var order = new Order { /* ... */ };
        await orderRepo.AddAsync(order);
        await _unitOfWork.SaveAsync();

        // Use Dapper for high-performance bulk insert
        var connection = _unitOfWork.Connection;
        var transaction = _unitOfWork.Transaction;
        var sql = "INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)";
        await connection.ExecuteAsync(sql, orderItems, transaction);

        // Commit both
        await _unitOfWork.CommitAsync();
        _unitOfWork.Commit();

        return Ok();
    }
    catch
    {
        await _unitOfWork.RollbackAsync();
        _unitOfWork.Rollback();
        throw;
    }
}
```

---

## Dependency Injection

### How Dependencies Flow Through the System

```
IServiceProvider (Root DI Container)
        │
        ▼
UnitOfWork (Resolves infrastructure internally)
        │
        ├── TestContext (EF Core)
        ├── IDbConnection (Dapper)
        ├── IMemoryCache
        ├── JWTSettings
        └── Creates RepositoryContext
        │
        ▼
RepositoryFactory (Auto-discovers repositories)
        │
        ▼
BaseRepository (Your custom repository)
        │
        └── GetService<T>() resolves additional dependencies
                │
                ├── IEmailService
                ├── ILogger<T>
                ├── IAuditLoggingService
                └── Any other service you need
```

### Resolving Services in Your Repository

**Pattern 1: Lazy Loading (Recommended)**

```csharp
public class MyRepository : BaseRepository<MyEntity>, IMyRepository
{
    // Lazy-loaded - only resolved when first accessed
    private IEmailService? _emailService;
    private IEmailService EmailService => _emailService ??= GetService<IEmailService>();

    private ILogger<MyRepository>? _logger;
    private ILogger<MyRepository> Logger => _logger ??= GetService<ILogger<MyRepository>>();

    public async Task MyMethod()
    {
        // Service resolved here on first use
        await EmailService.SendEmailAsync(...);
        Logger.LogInformation("Action performed");
    }
}
```

**Pattern 2: Constructor Injection (When Always Needed)**

```csharp
public class MyRepository : BaseRepository<MyEntity>, IMyRepository
{
    private readonly IEmailService _emailService;
    private readonly ILogger<MyRepository> _logger;

    public MyRepository(
        TestContext context,
        IDbConnection connection,
        IMemoryCache cache,
        IDbTransaction? transaction,
        IUnitOfWork unitOfWork,
        IServiceProvider serviceProvider)
        : base(context, connection, cache, transaction, serviceProvider)
    {
        // Resolve services immediately
        _emailService = GetService<IEmailService>();
        _logger = GetService<ILogger<MyRepository>>();
    }
}
```

### Why Use GetService<T>() Instead of Constructor Parameters?

**Bad Approach (Don't Do This):**
```csharp
// ❌ This pollutes UnitOfWork and RepositoryFactory
public MyRepository(
    TestContext context,
    IDbConnection connection,
    IMemoryCache cache,
    IDbTransaction? transaction,
    IUnitOfWork unitOfWork,
    IServiceProvider serviceProvider,
    IEmailService emailService,          // ❌ Specific to this repo
    ILogger<MyRepository> logger,        // ❌ Specific to this repo
    ISmsService smsService)              // ❌ Specific to this repo
```

**Good Approach (Do This):**
```csharp
// ✅ Clean constructor, services resolved internally
public MyRepository(...) : base(...)
{
    // Resolve only what you need
    _emailService = GetService<IEmailService>();
}
```

**Benefits:**
- ✅ UnitOfWork stays clean
- ✅ Repositories get only what they need
- ✅ Easy to add new services
- ✅ No need to update RepositoryFactory

---

## Advanced Examples

### Example 1: Complex Query with Multiple Repositories

```csharp
public async Task<OrderSummaryDto> GetOrderSummary(int orderId)
{
    // Get multiple repositories
    var orderRepo = _unitOfWork.Repository<Order>();
    var customerRepo = _unitOfWork.Repository<Customer>();
    var productRepo = _unitOfWork.Repository<Product>();

    // Complex query
    var order = await _context.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .Include(o => o.Customer)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);

    if (order == null) return null;

    return new OrderSummaryDto
    {
        OrderId = order.OrderId,
        CustomerName = order.Customer.Name,
        Items = order.OrderItems.Select(oi => new OrderItemDto
        {
            ProductName = oi.Product.Name,
            Quantity = oi.Quantity,
            Price = oi.Price
        }).ToList(),
        Total = order.OrderItems.Sum(oi => oi.Quantity * oi.Price)
    };
}
```

### Example 2: Bulk Operations with Dapper

```csharp
public async Task<int> BulkInsertProducts(List<Product> products)
{
    var connection = _unitOfWork.Connection;
    var transaction = _unitOfWork.Transaction;

    var sql = @"INSERT INTO Products (Name, Price, Stock)
                VALUES (@Name, @Price, @Stock)";

    // Dapper executes in batch - much faster than EF for bulk inserts
    var result = await connection.ExecuteAsync(sql, products, transaction);

    _unitOfWork.Commit();

    return result;
}
```

### Example 3: Using GetMaxID for Auto-Incrementing IDs

```csharp
public async Task<Permission> CreatePermission(PermissionDto dto)
{
    var permissionRepo = _unitOfWork.Repository<Permission>();

    // Get next available ID (works with both EF and Dapper)
    var nextId = await permissionRepo.GetMaxID(
        "PermissionID",          // Column name
        OrmType.EntityFramework,
        new CrudOptions
        {
            TableName = "tblPermission"  // Only needed for Dapper
        }
    );

    var permission = new Permission
    {
        PermissionID = nextId,
        ResourceID = dto.ResourceID,
        ActionTypeID = dto.ActionTypeID
    };

    await permissionRepo.AddAsync(permission);
    await _unitOfWork.SaveAsync();

    return permission;
}
```

### Example 4: Caching Strategy

```csharp
public async Task<IEnumerable<Product>> GetProductsWithCaching()
{
    var productRepo = _unitOfWork.Repository<Product>();

    // First call: Fetches from database, caches for 5 minutes
    var products1 = await productRepo.GetAllAsync();

    // Second call within 5 minutes: Returns from cache (instant)
    var products2 = await productRepo.GetAllAsync();

    // After adding/updating: Cache is invalidated automatically
    await productRepo.AddAsync(new Product { /* ... */ });
    await _unitOfWork.SaveAsync();

    // Next call: Fetches fresh data from database
    var products3 = await productRepo.GetAllAsync();

    return products3;
}
```

---

## Best Practices

### ✅ DO

1. **Use Generic Repository for Simple CRUD**
   ```csharp
   var productRepo = _unitOfWork.Repository<Product>();
   var products = await productRepo.GetAllAsync();
   ```

2. **Create Custom Repositories for Complex Logic**
   ```csharp
   [AutoRegisterRepository(typeof(IAuthRepository))]
   public class AuthRepository : BaseRepository<User>, IAuthRepository
   ```

3. **Use GetService<T>() for Additional Dependencies**
   ```csharp
   private IEmailService EmailService => _emailService ??= GetService<IEmailService>();
   ```

4. **Always Use Transactions for Multi-Step Operations**
   ```csharp
   await _unitOfWork.BeginTransactionAsync();
   // ... operations ...
   await _unitOfWork.CommitAsync();
   ```

5. **Choose the Right ORM for the Job**
   - EF Core: Complex queries, navigation properties, change tracking
   - Dapper: High performance, bulk operations, simple queries

### ❌ DON'T

1. **Don't Add Repository-Specific Dependencies to UnitOfWork**
   ```csharp
   // ❌ Bad
   public UnitOfWork(IServiceProvider sp, IEmailService email, ISmsService sms) { }

   // ✅ Good
   public UnitOfWork(IServiceProvider sp) { }
   ```

2. **Don't Forget to Call SaveAsync() with EF Core**
   ```csharp
   // ❌ Changes not persisted
   await productRepo.AddAsync(product);

   // ✅ Changes persisted
   await productRepo.AddAsync(product);
   await _unitOfWork.SaveAsync();
   ```

3. **Don't Mix Transaction Types Without Committing Both**
   ```csharp
   // ❌ Only commits EF transaction
   await _unitOfWork.CommitAsync();

   // ✅ Commits both
   await _unitOfWork.CommitAsync();
   _unitOfWork.Commit();
   ```

4. **Don't Forget [AutoRegisterRepository] Attribute**
   ```csharp
   // ❌ Repository not discovered
   public class MyRepository : BaseRepository<MyEntity> { }

   // ✅ Auto-discovered
   [AutoRegisterRepository(typeof(IMyRepository))]
   public class MyRepository : BaseRepository<MyEntity>, IMyRepository { }
   ```

---

## Summary

The Generic Repository pattern in this codebase provides:

✅ **Flexibility** - Use EF Core or Dapper as needed
✅ **Performance** - Built-in caching, optimized queries
✅ **Simplicity** - Auto-discovery, minimal boilerplate
✅ **Testability** - Easy to mock repositories
✅ **Maintainability** - Clean separation of concerns
✅ **Scalability** - Supports complex enterprise scenarios

**Key Takeaway:**
- Use `Repository<T>()` for simple CRUD
- Use `GetRepository<ICustomRepository>()` for complex logic
- Extend `BaseRepository<T>` and use `GetService<T>()` for dependencies

Now you're ready to build scalable, maintainable data access layers! 🚀
