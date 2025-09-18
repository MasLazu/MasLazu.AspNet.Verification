# MasLazu.AspNet.Verification

The Application layer of the MasLazu ASP.NET Verification system. This project contains the core business logic, services, validators, and utilities for handling verification workflows.

## 📋 Overview

This is the application layer that implements the verification business logic using Clean Architecture principles. It provides services for creating, managing, and validating verification codes across different channels (email, SMS, etc.).

## 🏗️ Architecture

This project follows the Application layer pattern in Clean Architecture:

- **Services**: Business logic implementation
- **Validators**: Request validation using FluentValidation
- **Extensions**: Dependency injection utilities
- **Utils**: Property mapping and expression building utilities

## 📦 Dependencies

### Project References

- `MasLazu.AspNet.Verification.Abstraction` - Core interfaces and models
- `MasLazu.AspNet.Verification.Domain` - Domain entities

### Package References

- `MasLazu.AspNet.Framework.Application` - Base application framework
- `MasLazu.AspNet.EmailSender.Abstraction` - Email sending abstraction
- `MassTransit` - Message bus for events
- `FluentValidation` - Validation framework
- `Mapster` - Object mapping

## 🚀 Features

### Core Services

#### VerificationService

The main service for verification operations:

```csharp
public class VerificationService : CrudService<Domain.Entities.Verification, VerificationDto, CreateVerificationRequest, UpdateVerificationRequest>, IVerificationService
```

**Key Methods:**

- `CreateVerificationAsync()` - Creates new verification codes
- `SendVerificationAsync()` - Sends verification codes via email
- `VerifyAsync()` - Validates and marks codes as verified
- `IsCodeValidAsync()` - Checks code validity
- `GetByCodeAsync()` - Retrieves verification by code

**Email Integration:**

- Uses `IEmailSender` for sending emails
- Supports HTML templates with `IHtmlRenderer`
- Publishes `VerificationCompletedEvent` via MassTransit

#### VerificationPurposeService

CRUD service for managing verification purposes:

```csharp
public class VerificationPurpose : CrudService<VerificationPurpose, VerificationPurposeDto, CreateVerificationPurposeRequest, UpdateVerificationPurposeRequest>, IVerificationPurposeService
```

**Key Methods:**

- `CreateIfNotExistsAsync()` - Creates purpose if it doesn't exist

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

Dynamic property mapping for sorting and filtering:

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

Utility for registering property maps and expression builders:

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

## 🔧 Configuration

### Service Registration

```csharp
// Register application utilities
services.AddVerificationApplicationUtils();

// Register email services
services.AddScoped<IEmailSender, YourEmailSenderImplementation>();
services.AddScoped<IHtmlRenderer, YourHtmlRendererImplementation>();

// Register MassTransit for events
services.AddMassTransit(config => {
    // Configure message bus
});
```

### Email Configuration

The service expects these dependencies to be registered:

- `IEmailSender` - For sending emails
- `IHtmlRenderer` - For rendering HTML templates
- `IPublishEndpoint` - For publishing events

## 📋 Usage Examples

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

### Email Template Usage

```csharp
private async Task SendEmailVerificationAsync(VerificationDto verification)
{
    EmailMessage email = new EmailMessageBuilder()
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
}
```

## 🔄 Event System

The service publishes events through MassTransit:

### VerificationCompletedEvent

Published when a verification is successfully completed:

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
    Assert.Contains(result.Errors, e => e.PropertyName == "Destination");
}
```

## 📈 Extensibility

### Adding Custom Validators

```csharp
public class CustomVerificationValidator : AbstractValidator<CreateVerificationRequest>
{
    public CustomVerificationValidator()
    {
        RuleFor(x => x.CustomField)
            .NotEmpty()
            .Must(BeValidCustomField);
    }

    private bool BeValidCustomField(string field)
    {
        // Custom validation logic
        return true;
    }
}
```

### Extending Property Maps

```csharp
public class ExtendedVerificationPropertyMap : VerificationEntityPropertyMap
{
    public ExtendedVerificationPropertyMap()
    {
        // Add additional property mappings
        _map.Add("customProperty", v => v.CustomProperty);
    }
}
```

### Custom Email Templates

Implement `IHtmlRenderer` for custom template rendering:

```csharp
public class CustomHtmlRenderer : IHtmlRenderer
{
    public Task<string> RenderAsync(string template, object model)
    {
        // Custom rendering logic
        return Task.FromResult("Rendered HTML");
    }
}
```

## 🛠️ Development

### Code Structure

```
src/MasLazu.AspNet.Verification/
├── Extensions/           # DI extensions
├── Services/            # Business logic services
├── Utils/               # Property maps and utilities
├── Validators/          # Request validators
└── MasLazu.AspNet.Verification.csproj
```

### Best Practices

- Use dependency injection for all services
- Implement comprehensive validation
- Follow async/await patterns
- Use meaningful exception messages
- Document public APIs with XML comments

## 🤝 Contributing

1. Maintain Clean Architecture principles
2. Add unit tests for new functionality
3. Update validators for new request types
4. Follow naming conventions
5. Keep services focused on single responsibilities

## 📄 License

Part of the MasLazu ASP.NET framework ecosystem.</content>
<parameter name="filePath">/home/mfaziz/projects/cs/MasLazu.AspNet.Verification/src/MasLazu.AspNet.Verification/README.md
