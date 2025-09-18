# MasLazu.AspNet.Verification

A comprehensive ASP.NET Core verification system for handling user verification codes via email or other channels. This project provides a modular, extensible architecture for implementing verification workflows in .NET applications.

## 🏗️ Architecture

This solution follows Clean Architecture principles with the following layers:

- **Abstraction**: Interfaces, models, and enums
- **Domain**: Business entities and domain logic
- **Application**: Services, validators, and application logic
- **Infrastructure**: Data access and external integrations (EfCore)
- **Endpoint**: API endpoints and presentation layer

## 📦 Projects Overview

### Core Projects

- `MasLazu.AspNet.Verification.Abstraction` - Core interfaces, models, and enums
- `MasLazu.AspNet.Verification.Domain` - Domain entities and business rules
- `MasLazu.AspNet.Verification` - Application services and logic
- `MasLazu.AspNet.Verification.EfCore` - Entity Framework Core implementation
- `MasLazu.AspNet.Verification.Endpoint` - API endpoints and controllers

## 🚀 Features

### Verification Management

- **Multi-channel Support**: Email, SMS (extensible)
- **Purpose-based Verification**: Different verification types (registration, password reset, etc.)
- **Expiration Handling**: Configurable expiry times
- **Attempt Tracking**: Monitor verification attempts
- **Status Management**: Pending, Verified, Expired states

### Email Integration

- **Template-based Emails**: HTML email templates with theming
- **Rich Content**: Support for styled emails with company branding
- **Async Processing**: Non-blocking email sending
- **Extensible Renderer**: Pluggable HTML rendering

### Validation & Security

- **Request Validation**: Comprehensive input validation using FluentValidation
- **Secure Code Generation**: Cryptographically secure verification codes
- **Rate Limiting**: Built-in attempt tracking and limits

---

# � Project Details

## MasLazu.AspNet.Verification.Abstraction

The Abstraction layer defines the core contracts, data models, and domain types.

### Service Interfaces

#### IVerificationService

```csharp
Task<VerificationDto?> GetByCodeAsync(Guid userId, string code, CancellationToken ct = default);
Task<bool> IsCodeValidAsync(Guid userId, string code, CancellationToken ct = default);
Task<VerificationDto> VerifyAsync(string code, CancellationToken ct = default);
Task<VerificationDto> CreateVerificationAsync(Guid userId, CreateVerificationRequest request, CancellationToken ct = default);
Task<VerificationDto> SendVerificationAsync(Guid userId, SendVerificationRequest request, CancellationToken ct = default);
```

#### IVerificationPurposeService

```csharp
Task<VerificationPurposeDto> CreateIfNotExistsAsync(Guid id, CreateVerificationPurposeRequest createRequest, CancellationToken ct = default);
```

### Data Models

#### VerificationDto

```csharp
public record VerificationDto(
    Guid Id,
    Guid UserId,
    VerificationChannel Channel,
    string Destination,
    string VerificationCode,
    string VerificationPurposeCode,
    VerificationStatus Status,
    int AttemptCount,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? VerifiedAt,
    VerificationPurposeDto? VerificationPurpose,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
) : BaseDto(Id, CreatedAt, UpdatedAt);
```

#### Request Models

- `CreateVerificationRequest`
- `SendVerificationRequest`
- `UpdateVerificationRequest`
- `CreateVerificationPurposeRequest`

### Domain Enums

#### VerificationChannel

```csharp
public enum VerificationChannel
{
    Email
}
```

#### VerificationStatus

```csharp
public enum VerificationStatus
{
    Pending,
    Verified,
    Failed
}
```

### Domain Events

#### VerificationCompletedEvent

```csharp
public class VerificationCompletedEvent
{
    public Guid VerificationId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PurposeCode { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public bool IsSuccessful { get; set; }
}
```

---

## MasLazu.AspNet.Verification (Application Layer)

The Application layer contains the core business logic, services, validators, and utilities.

### Core Services

#### VerificationService

```csharp
public class VerificationService : CrudService<Domain.Entities.Verification, VerificationDto, CreateVerificationRequest, UpdateVerificationRequest>, IVerificationService
```

**Key Methods:**

- `CreateVerificationAsync()` - Creates new verification codes
- `SendVerificationAsync()` - Sends verification codes via email
- `VerifyAsync()` - Validates and marks codes as verified
- `IsCodeValidAsync()` - Checks code validity
- `GetByCodeAsync()` - Retrieves verification by code

#### VerificationPurposeService

```csharp
public class VerificationPurposeService : CrudService<VerificationPurpose, VerificationPurposeDto, CreateVerificationPurposeRequest, UpdateVerificationPurposeRequest>, IVerificationPurposeService
```

### Validation System

Comprehensive request validation using FluentValidation:

#### Available Validators

- `CreateVerificationRequestValidator`
- `UpdateVerificationRequestValidator`
- `SendVerificationRequestValidator`
- `VerifyCodeRequestValidator`
- `CreateVerificationPurposeRequestValidator`
- `UpdateVerificationPurposeRequestValidator`

#### Example Validation Rules

```csharp
public class CreateVerificationRequestValidator : AbstractValidator<CreateVerificationRequest>
{
    RuleFor(x => x.UserId).NotEmpty();
    RuleFor(x => x.Channel).IsInEnum();
    RuleFor(x => x.Destination).NotEmpty().EmailAddress()
        .When(x => x.Channel == VerificationChannel.Email);
    RuleFor(x => x.ExpiresAt).GreaterThan(DateTimeOffset.UtcNow)
        .When(x => x.ExpiresAt.HasValue);
}
```

### Property Mapping System

#### VerificationEntityPropertyMap

```csharp
public class VerificationEntityPropertyMap : IEntityPropertyMap<Domain.Entities.Verification>
{
    private readonly Dictionary<string, Expression<Func<Domain.Entities.Verification, object>>> _map = new()
    {
        { "id", v => v.Id },
        { "userId", v => v.UserId },
        { "channel", v => v.Channel },
        { "status", v => v.Status },
        // ... more mappings
    };
}
```

### Dependency Injection Extensions

#### VerificationApplicationUtilExtension

```csharp
public static class VerificationApplicationUtilExtension
{
    public static IServiceCollection AddVerificationApplicationUtils(this IServiceCollection services)
    {
        RegisterPropertyMapsAndExpressionBuilders(services);
        return services;
    }
}
```

---

## MasLazu.AspNet.Verification.EfCore (Infrastructure Layer)

The Infrastructure layer provides Entity Framework Core implementation for data persistence.

### Database Contexts

#### VerificationDbContext (Write Context)

```csharp
public class VerificationDbContext : BaseDbContext
{
    public DbSet<Domain.Entities.Verification> Verifications { get; set; }
    public DbSet<VerificationPurpose> VerificationPurposes { get; set; }
}
```

#### VerificationReadDbContext (Read Context)

```csharp
public class VerificationReadDbContext : BaseReadDbContext
{
    public DbSet<Domain.Entities.Verification> Verifications { get; set; }
    public DbSet<VerificationPurpose> VerificationPurposes { get; set; }
}
```

### Entity Configurations

#### VerificationConfiguration

```csharp
public class VerificationConfiguration : IEntityTypeConfiguration<Domain.Entities.Verification>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Verification> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.UserId).IsRequired();
        builder.Property(v => v.Channel).IsRequired().HasConversion<string>();
        builder.Property(v => v.Destination).IsRequired().HasMaxLength(255);
        builder.Property(v => v.VerificationCode).IsRequired().HasMaxLength(10);
        builder.Property(v => v.Status).IsRequired().HasConversion<string>()
            .HasDefaultValue(VerificationStatus.Pending);
        builder.Property(v => v.ExpiresAt).IsRequired();

        // Relationships and indexes...
    }
}
```

### Database Schema

#### Verifications Table

```sql
CREATE TABLE Verifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Channel NVARCHAR(50) NOT NULL,
    Destination NVARCHAR(255) NOT NULL,
    VerificationCode NVARCHAR(10) NOT NULL,
    VerificationPurposeCode NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    AttemptCount INT DEFAULT 0,
    ExpiresAt DATETIMEOFFSET NOT NULL,
    VerifiedAt DATETIMEOFFSET NULL,
    CreatedAt DATETIMEOFFSET NOT NULL,
    UpdatedAt DATETIMEOFFSET NULL
);
```

#### Indexes

- `IX_Verifications_UserId_VerificationPurposeCode_Status`
- `IX_Verifications_VerificationCode`
- `IX_Verifications_ExpiresAt`

---

## MasLazu.AspNet.Verification.Endpoint (Presentation Layer)

The Endpoint layer provides REST API endpoints using FastEndpoints framework.

### API Endpoints

#### POST /api/v1/verification/verify

Verifies a verification code.

**Request:**

```json
{
  "code": "123456"
}
```

**Response:**

```json
{
  "data": {
    "id": "guid",
    "userId": "guid",
    "channel": "Email",
    "destination": "user@example.com",
    "verificationCode": "123456",
    "status": "Verified",
    "attemptCount": 1,
    "expiresAt": "2025-09-18T10:30:00Z",
    "verifiedAt": "2025-09-18T10:15:00Z"
  },
  "message": "Verification successful",
  "success": true
}
```

### Endpoint Implementation

#### VerifyEndpoint

```csharp
public class VerifyEndpoint : BaseEndpoint<VerifyRequest, VerificationDto>
{
    public IVerificationService VerificationService { get; set; }

    public override void ConfigureEndpoint()
    {
        Post("/verify");
        Group<VerificationEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(VerifyRequest req, CancellationToken ct)
    {
        VerificationDto result = await VerificationService.VerifyAsync(req.Code, ct);
        await SendOkResponseAsync(result, "Verification successful", ct);
    }
}
```

### Endpoint Groups

#### VerificationEndpointGroup

```csharp
public class VerificationEndpointGroup : SubGroup<V1EndpointGroup>
{
    public VerificationEndpointGroup()
    {
        Configure("verification", ep => ep.Description(x => x.WithTags("Verification")));
    }
}
```

### Request Models

#### VerifyRequest

```csharp
public record VerifyRequest(string Code);
```

---

## 🔧 Configuration

### Package Dependencies

- `MasLazu.AspNet.Framework.Application` - Base application framework
- `MasLazu.AspNet.EmailSender.Abstraction` - Email sending abstraction
- `MasLazu.AspNet.Framework.EfCore` - EF Core base framework
- `Microsoft.EntityFrameworkCore` - EF Core ORM
- `MassTransit` - Message bus for events
- `FluentValidation` - Validation framework
- `Mapster` - Object mapping
- `FastEndpoints` - API framework

### Service Registration

```csharp
// Application services
services.AddVerificationApplicationUtils();

// EF Core contexts
services.AddDbContext<VerificationDbContext>(options =>
    options.UseSqlServer(connectionString));
services.AddDbContext<VerificationReadDbContext>(options =>
    options.UseSqlServer(connectionString));

// Email services
services.AddScoped<IEmailSender, YourEmailSenderImplementation>();
services.AddScoped<IHtmlRenderer, YourHtmlRendererImplementation>();

// FastEndpoints
services.AddFastEndpoints();
```

### Email Configuration

```csharp
var email = new EmailMessageBuilder()
    .From("security@yourapp.com", "Your App Security")
    .To(verification.Destination)
    .Subject("🔐 Verify Your Account")
    .RenderOptions(new EmailRenderOptions
    {
        Theme = "VerificationCode",
        CompanyName = "Your Company",
        PrimaryColor = "#28a745"
    })
    .Model(new
    {
        VerificationCode = verification.VerificationCode,
        UserName = "User",
        ExpiryMinutes = 15
    })
    .Build();

await _emailSender.SendEmailAsync(email, _htmlRenderer);
```

---

## 📋 API Usage Examples

### Creating a Verification

```csharp
var verificationService = serviceProvider.GetRequiredService<IVerificationService>();

var request = new CreateVerificationRequest(
    UserId: Guid.NewGuid(),
    Channel: VerificationChannel.Email,
    Destination: "user@example.com",
    VerificationPurposeCode: "registration",
    ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15)
);

var verification = await verificationService.CreateVerificationAsync(userId, request);
```

### Sending Verification Email

```csharp
var sendRequest = new SendVerificationRequest(
    UserId: userId,
    Destination: "user@example.com",
    PurposeCode: "registration"
);

var verification = await verificationService.SendVerificationAsync(userId, sendRequest);
```

### Verifying a Code

```csharp
var isValid = await verificationService.IsCodeValidAsync(userId, code);
if (isValid)
{
    var result = await verificationService.VerifyAsync(code);
    // Handle successful verification
}
```

### API Endpoint Usage

```bash
# Verify a code
curl -X POST "https://api.yourapp.com/api/v1/verification/verify" \
     -H "Content-Type: application/json" \
     -d '{"code": "123456"}'
```

---

## 🧪 Testing

### Unit Testing Services

```csharp
[Fact]
public async Task CreateVerificationAsync_ShouldCreateVerification()
{
    // Arrange
    var mockRepo = new Mock<IRepository<Domain.Entities.Verification>>();
    var service = new VerificationService(/* dependencies */);

    // Act
    var result = await service.CreateVerificationAsync(userId, request);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(VerificationStatus.Pending, result.Status);
}
```

### Testing Validators

```csharp
[Fact]
public void CreateVerificationRequestValidator_ShouldValidateEmail()
{
    var validator = new CreateVerificationRequestValidator();
    var request = new CreateVerificationRequest
    {
        Channel = VerificationChannel.Email,
        Destination = "invalid-email"
    };

    var result = validator.Validate(request);
    Assert.False(result.IsValid);
}
```

### Endpoint Testing

```csharp
[Fact]
public async Task VerifyEndpoint_ShouldReturnSuccess_WhenCodeIsValid()
{
    var client = CreateClient();
    var request = new { code = "123456" };

    var response = await client.PostAsJsonAsync("/api/v1/verification/verify", request);
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

### Database Testing

```csharp
[Fact]
public async Task VerificationRepository_ShouldCreateVerification()
{
    var options = new DbContextOptionsBuilder<VerificationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;

    using var context = new VerificationDbContext(options);
    var repository = new EfRepository<Domain.Entities.Verification, VerificationDbContext>(context);

    var verification = new Domain.Entities.Verification { /* ... */ };
    await repository.AddAsync(verification);
    await context.SaveChangesAsync();

    var saved = await context.Verifications.FindAsync(verification.Id);
    Assert.NotNull(saved);
}
```

---

## 📈 Extensibility

### Adding New Channels

1. Extend `VerificationChannel` enum
2. Implement channel-specific sending logic
3. Add channel validators

### Custom Email Templates

1. Implement `IHtmlRenderer`
2. Define template models
3. Configure rendering options

### Adding New Endpoints

1. Create endpoint class inheriting from `BaseEndpoint<TRequest, TResponse>`
2. Implement `ConfigureEndpoint()` and `HandleAsync()`
3. Add to appropriate endpoint group

### Database Extensions

1. Create new entity configurations
2. Add to DbContext
3. Create EF Core migrations

---

## 🔄 Event System

### VerificationCompletedEvent

Published when verification is completed:

```csharp
var verificationEvent = new VerificationCompletedEvent
{
    VerificationId = verification.Id,
    UserId = verification.UserId,
    Email = verification.Destination,
    PurposeCode = verification.VerificationPurposeCode,
    CompletedAt = DateTime.UtcNow,
    IsSuccessful = true
};

await _publishEndpoint.Publish(verificationEvent);
```

### Integration with MassTransit

```csharp
services.AddMassTransit(config => {
    config.AddConsumer<VerificationCompletedConsumer>();
    config.UsingRabbitMq((context, cfg) => {
        // Configure RabbitMQ
    });
});
```

---

## 🛠️ Development

### Project Structure

```
src/
├── MasLazu.AspNet.Verification.Abstraction/
│   ├── Enums/
│   ├── Events/
│   ├── Interfaces/
│   └── Models/
├── MasLazu.AspNet.Verification.Domain/
│   └── Entities/
├── MasLazu.AspNet.Verification/
│   ├── Extensions/
│   ├── Services/
│   ├── Utils/
│   └── Validators/
├── MasLazu.AspNet.Verification.EfCore/
│   ├── Configurations/
│   ├── Data/
│   └── Extensions/
└── MasLazu.AspNet.Verification.Endpoint/
    ├── Endpoints/
    ├── EndpointGroups/
    ├── Extensions/
    └── Models/
```

### Code Standards

- Follow Clean Architecture principles
- Use dependency injection throughout
- Implement comprehensive validation
- Write unit tests for all components
- Use meaningful naming conventions
- Include XML documentation

### Best Practices

- Keep interfaces focused and minimal
- Use records for immutable DTOs
- Implement proper error handling
- Use async/await consistently
- Follow CQRS for data operations
- Validate all inputs

---

## 🤝 Contributing

1. **Architecture**: Follow Clean Architecture principles
2. **Testing**: Add comprehensive unit and integration tests
3. **Documentation**: Update READMEs and XML comments
4. **Code Review**: Ensure separation of concerns
5. **Migrations**: Use EF Core migrations for schema changes
6. **API Design**: Follow RESTful conventions

### Development Workflow

1. Create feature branch
2. Implement changes with tests
3. Update documentation
4. Submit pull request
5. Code review and merge

---

## 📄 License

This project is part of the MasLazu ASP.NET framework ecosystem.
