# MasLazu.AspNet.Verification.EfCore

The Infrastructure layer of the MasLazu ASP.NET Verification system. This project provides Entity Framework Core implementation for data persistence using Clean Architecture and CQRS patterns.

## 📋 Overview

This is the infrastructure layer that implements data access using Entity Framework Core. It provides database contexts, entity configurations, and repository implementations following CQRS (Command Query Responsibility Segregation) pattern.

## 🏗️ Architecture

This project represents the **Infrastructure Layer** in Clean Architecture:

- **Data Contexts**: EF Core DbContexts for write and read operations
- **Configurations**: Entity type configurations and relationships
- **Extensions**: Dependency injection utilities for EF Core setup
- **CQRS Support**: Separate contexts for commands and queries

## 📦 Dependencies

### Project References

- `MasLazu.AspNet.Verification.Abstraction` - Core interfaces
- `MasLazu.AspNet.Verification.Domain` - Domain entities

### Package References

- `MasLazu.AspNet.Framework.EfCore` - Base EF Core framework
- `Microsoft.EntityFrameworkCore` - EF Core ORM

## 🚀 Core Components

### Database Contexts

#### VerificationDbContext (Write Context)

Main context for write operations (commands):

```csharp
public class VerificationDbContext : BaseDbContext
{
    public VerificationDbContext(DbContextOptions<VerificationDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Verification> Verifications { get; set; }
    public DbSet<VerificationPurpose> VerificationPurposes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

#### VerificationReadDbContext (Read Context)

Optimized context for read operations (queries):

```csharp
public class VerificationReadDbContext : BaseReadDbContext
{
    public VerificationReadDbContext(DbContextOptions<VerificationReadDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Verification> Verifications { get; set; }
    public DbSet<VerificationPurpose> VerificationPurposes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

## 🔧 Entity Configurations

### VerificationConfiguration

Comprehensive configuration for the Verification entity:

```csharp
public class VerificationConfiguration : IEntityTypeConfiguration<Domain.Entities.Verification>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Verification> builder)
    {
        // Primary Key
        builder.HasKey(v => v.Id);

        // Required Properties
        builder.Property(v => v.UserId).IsRequired();
        builder.Property(v => v.Channel).IsRequired().HasConversion<string>();
        builder.Property(v => v.Destination).IsRequired().HasMaxLength(255);
        builder.Property(v => v.VerificationCode).IsRequired().HasMaxLength(10);
        builder.Property(v => v.Status).IsRequired().HasConversion<string>()
            .HasDefaultValue(VerificationStatus.Pending);
        builder.Property(v => v.ExpiresAt).IsRequired();

        // Optional Properties
        builder.Property(v => v.VerifiedAt).IsRequired(false);
        builder.Property(v => v.AttemptCount).HasDefaultValue(0);

        // Relationships
        builder.HasOne(v => v.VerificationPurpose)
            .WithMany(vp => vp.Verifications)
            .HasForeignKey(v => v.VerificationPurposeCode)
            .HasPrincipalKey(vp => vp.Code)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(v => new { v.UserId, v.VerificationPurposeCode, v.Status });
        builder.HasIndex(v => v.VerificationCode);
        builder.HasIndex(v => v.ExpiresAt);
    }
}
```

### VerificationPurposeConfiguration

Configuration for verification purposes:

```csharp
public class VerificationPurposeConfiguration : IEntityTypeConfiguration<VerificationPurpose>
{
    public void Configure(EntityTypeBuilder<VerificationPurpose> builder)
    {
        builder.HasKey(vp => vp.Id);
        builder.Property(vp => vp.Code).IsRequired().HasMaxLength(50);
        builder.Property(vp => vp.Name).IsRequired().HasMaxLength(100);
        builder.Property(vp => vp.Description).HasMaxLength(500);
        builder.Property(vp => vp.IsActive).HasDefaultValue(true);

        builder.HasIndex(vp => vp.Code).IsUnique();
        builder.HasIndex(vp => vp.IsActive);
    }
}
```

## 📊 Database Schema

### Tables

#### Verifications

```sql
CREATE TABLE Verifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Channel NVARCHAR(50) NOT NULL, -- Enum stored as string
    Destination NVARCHAR(255) NOT NULL,
    VerificationCode NVARCHAR(10) NOT NULL,
    VerificationPurposeCode NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    AttemptCount INT DEFAULT 0,
    ExpiresAt DATETIMEOFFSET NOT NULL,
    VerifiedAt DATETIMEOFFSET NULL,
    CreatedAt DATETIMEOFFSET NOT NULL,
    UpdatedAt DATETIMEOFFSET NULL,

    FOREIGN KEY (VerificationPurposeCode) REFERENCES VerificationPurposes(Code)
);
```

#### VerificationPurposes

```sql
CREATE TABLE VerificationPurposes (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIMEOFFSET NOT NULL,
    UpdatedAt DATETIMEOFFSET NULL
);
```

### Indexes

- `IX_Verifications_UserId_VerificationPurposeCode_Status`
- `IX_Verifications_VerificationCode`
- `IX_Verifications_ExpiresAt`
- `IX_VerificationPurposes_Code` (Unique)
- `IX_VerificationPurposes_IsActive`

## 🔧 Configuration

### Service Registration

```csharp
// Register EF Core contexts
services.AddDbContext<VerificationDbContext>(options =>
    options.UseSqlServer(connectionString));

services.AddDbContext<VerificationReadDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories
services.AddScoped<IRepository<Domain.Entities.Verification>, EfRepository<Domain.Entities.Verification, VerificationDbContext>>();
services.AddScoped<IReadRepository<Domain.Entities.Verification>, EfReadRepository<Domain.Entities.Verification, VerificationReadDbContext>>();

// Register unit of work
services.AddScoped<IUnitOfWork, EfUnitOfWork<VerificationDbContext>>();
```

### Connection String

```json
{
  "ConnectionStrings": {
    "VerificationDb": "Server=.;Database=VerificationDb;Trusted_Connection=True;MultipleActiveResultSets=true",
    "VerificationReadDb": "Server=.;Database=VerificationDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Migration Setup

```csharp
// In Program.cs or Startup.cs
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VerificationDbContext>();
    await dbContext.Database.MigrateAsync();
}
```

## 🚀 Usage Examples

### Repository Pattern Implementation

```csharp
public class VerificationService : IVerificationService
{
    private readonly IRepository<Domain.Entities.Verification> _repository;
    private readonly IReadRepository<Domain.Entities.Verification> _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerificationService(
        IRepository<Domain.Entities.Verification> repository,
        IReadRepository<Domain.Entities.Verification> readRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VerificationDto> CreateVerificationAsync(CreateVerificationRequest request)
    {
        var verification = new Domain.Entities.Verification
        {
            // ... set properties
        };

        await _repository.AddAsync(verification);
        await _unitOfWork.SaveChangesAsync();

        return verification.Adapt<VerificationDto>();
    }
}
```

### Query Optimization

```csharp
public async Task<VerificationDto?> GetByCodeAsync(string code)
{
    return await _readRepository.FirstOrDefaultAsync(
        v => v.VerificationCode == code &&
             v.Status == VerificationStatus.Pending &&
             v.ExpiresAt > DateTimeOffset.UtcNow);
}
```

### Bulk Operations

```csharp
public async Task ExpireOldVerificationsAsync()
{
    var expiredVerifications = await _repository.FindAsync(
        v => v.ExpiresAt < DateTimeOffset.UtcNow &&
             v.Status == VerificationStatus.Pending);

    foreach (var verification in expiredVerifications)
    {
        verification.Status = VerificationStatus.Failed;
        await _repository.UpdateAsync(verification);
    }

    await _unitOfWork.SaveChangesAsync();
}
```

## 🧪 Testing

### Unit Testing with In-Memory Database

```csharp
[Fact]
public async Task CreateVerificationAsync_ShouldCreateVerification()
{
    // Arrange
    var options = new DbContextOptionsBuilder<VerificationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;

    using var context = new VerificationDbContext(options);
    var repository = new EfRepository<Domain.Entities.Verification, VerificationDbContext>(context);

    // Act
    var verification = new Domain.Entities.Verification { /* ... */ };
    await repository.AddAsync(verification);
    await context.SaveChangesAsync();

    // Assert
    var saved = await context.Verifications.FindAsync(verification.Id);
    Assert.NotNull(saved);
}
```

### Integration Testing

```csharp
[Fact]
public async Task VerificationRepository_ShouldHandleConcurrency()
{
    // Arrange
    using var factory = new SqliteConnectionFactory();
    var connection = factory.CreateConnection();

    var options = new DbContextOptionsBuilder<VerificationDbContext>()
        .UseSqlite(connection)
        .Options;

    // Act & Assert
    // Test concurrent operations
}
```

## 📈 Performance Optimization

### Query Optimization

```csharp
// Use AsNoTracking for read-only queries
public async Task<List<VerificationDto>> GetPendingVerificationsAsync()
{
    return await _readRepository.FindAsync(
        v => v.Status == VerificationStatus.Pending,
        query => query.AsNoTracking()
                      .OrderBy(v => v.ExpiresAt));
}
```

### Indexing Strategy

- **Composite Indexes**: For common query patterns
- **Single Column**: For unique constraints and lookups
- **Filtered Indexes**: For active records only

### Connection Pooling

```csharp
// Configure connection pooling
services.AddDbContextPool<VerificationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MaxBatchSize(100);
        sqlOptions.CommandTimeout(30);
    }));
```

## 🔄 Migrations

### Creating Migrations

```bash
# Add migration
dotnet ef migrations add InitialCreate --project src/MasLazu.AspNet.Verification.EfCore --startup-project src/MasLazu.AspNet.Verification.Endpoint

# Update database
dotnet ef database update --project src/MasLazu.AspNet.Verification.EfCore --startup-project src/MasLazu.AspNet.Verification.Endpoint
```

### Migration Files

```csharp
[Migration("20230918000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create tables and indexes
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Rollback logic
    }
}
```

## 📊 Monitoring & Health Checks

### Health Check Configuration

```csharp
services.AddHealthChecks()
    .AddDbContextCheck<VerificationDbContext>("VerificationDb")
    .AddDbContextCheck<VerificationReadDbContext>("VerificationReadDb");
```

### Metrics Collection

```csharp
// Configure EF Core metrics
services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder.AddMeter("Microsoft.EntityFrameworkCore");
    });
```

## 🛠️ Development Guidelines

### Code Structure

```
src/MasLazu.AspNet.Verification.EfCore/
├── Configurations/     # Entity type configurations
├── Data/              # DbContext classes
├── Extensions/        # DI utilities
└── Migrations/        # EF Core migrations (generated)
```

### Best Practices

- Use separate contexts for read/write operations
- Configure indexes for common query patterns
- Use appropriate data types and constraints
- Implement proper foreign key relationships
- Use migrations for schema changes
- Test with realistic data volumes

### Naming Conventions

- **Configurations**: Entity name + "Configuration"
- **Contexts**: Domain name + "DbContext" / "ReadDbContext"
- **Migrations**: Descriptive names with timestamps

## 🤝 Contributing

1. **Schema Changes**: Use EF Core migrations for database changes
2. **Performance**: Consider query optimization and indexing
3. **Testing**: Add integration tests for data operations
4. **Documentation**: Update entity configurations documentation
5. **Code Review**: Ensure proper separation of read/write contexts

## 📄 License

Part of the MasLazu ASP.NET framework ecosystem.</content>
<parameter name="filePath">/home/mfaziz/projects/cs/MasLazu.AspNet.Verification/src/MasLazu.AspNet.Verification.EfCore/README.md
