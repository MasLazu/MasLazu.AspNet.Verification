# MasLazu.AspNet.Verification.Abstraction

The Abstraction layer of the MasLazu ASP.NET Verification system. This project defines the core contracts, data models, and domain types that form the foundation of the verification system.

## 📋 Overview

This is the abstraction layer that defines the public API and data contracts for the verification system. It contains interfaces, data transfer objects (DTOs), enums, and events that other layers implement and use.

## 🏗️ Architecture

This project represents the **Abstraction Layer** in Clean Architecture:

- **Interfaces**: Service contracts and abstractions
- **Models**: Data transfer objects and request/response models
- **Enums**: Domain enumerations and constants
- **Events**: Domain events for inter-system communication

## 📦 Dependencies

### Package References

- `MasLazu.AspNet.Framework.Application` - Base framework with common abstractions

## 🚀 Core Components

### Service Interfaces

#### IVerificationService

The main interface for verification operations:

```csharp
public interface IVerificationService
{
    Task<VerificationDto?> GetByCodeAsync(Guid userId, string code, CancellationToken ct = default);
    Task<bool> IsCodeValidAsync(Guid userId, string code, CancellationToken ct = default);
    Task<VerificationDto> VerifyAsync(string code, CancellationToken ct = default);
    Task<VerificationDto> CreateVerificationAsync(Guid userId, CreateVerificationRequest request, CancellationToken ct = default);
    Task<VerificationDto> SendVerificationAsync(Guid userId, SendVerificationRequest request, CancellationToken ct = default);
}
```

#### IVerificationPurposeService

Interface for managing verification purposes:

```csharp
public interface IVerificationPurposeService
{
    Task<VerificationPurposeDto> CreateIfNotExistsAsync(Guid id, CreateVerificationPurposeRequest createRequest, CancellationToken ct = default);
}
```

### Data Models

#### VerificationDto

Core data transfer object for verification information:

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

##### CreateVerificationRequest

```csharp
public record CreateVerificationRequest(
    Guid UserId,
    VerificationChannel Channel,
    string Destination,
    string VerificationPurposeCode,
    DateTimeOffset? ExpiresAt = null
);
```

##### SendVerificationRequest

```csharp
public record SendVerificationRequest(
    Guid UserId,
    string Destination,
    string PurposeCode,
    DateTimeOffset? ExpiresAt = null
);
```

##### UpdateVerificationRequest

```csharp
public record UpdateVerificationRequest(
    Guid Id,
    Guid UserId,
    VerificationChannel? Channel,
    string? Destination,
    string? VerificationPurposeCode,
    VerificationStatus? Status,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? UpdatedAt
);
```

#### VerificationPurposeDto

Data transfer object for verification purposes:

```csharp
public record VerificationPurposeDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
) : BaseDto(Id, CreatedAt, UpdatedAt);
```

### Domain Enums

#### VerificationChannel

Defines the communication channels for verification:

```csharp
public enum VerificationChannel
{
    Email
}
```

**Extensibility Note**: Currently supports Email, but can be extended to include SMS, Push notifications, etc.

#### VerificationStatus

Represents the lifecycle states of a verification:

```csharp
public enum VerificationStatus
{
    Pending,    // Code created, awaiting verification
    Verified,   // Code successfully verified
    Failed      // Verification failed or expired
}
```

### Domain Events

#### VerificationCompletedEvent

Event published when verification is completed:

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

## 🔧 Usage Examples

### Implementing IVerificationService

```csharp
public class VerificationService : IVerificationService
{
    public async Task<VerificationDto> CreateVerificationAsync(Guid userId, CreateVerificationRequest request, CancellationToken ct = default)
    {
        // Implementation logic
        var verification = new VerificationDto(
            Id: Guid.NewGuid(),
            UserId: userId,
            Channel: request.Channel,
            Destination: request.Destination,
            VerificationCode: GenerateCode(),
            VerificationPurposeCode: request.VerificationPurposeCode,
            Status: VerificationStatus.Pending,
            AttemptCount: 0,
            ExpiresAt: request.ExpiresAt ?? DateTimeOffset.UtcNow.AddMinutes(15),
            VerifiedAt: null,
            VerificationPurpose: null,
            CreatedAt: DateTimeOffset.UtcNow,
            UpdatedAt: null
        );

        return verification;
    }

    // Other interface implementations...
}
```

### Using Request Models

```csharp
// Creating a verification
var createRequest = new CreateVerificationRequest(
    UserId: Guid.NewGuid(),
    Channel: VerificationChannel.Email,
    Destination: "user@example.com",
    VerificationPurposeCode: "registration",
    ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15)
);

// Sending verification
var sendRequest = new SendVerificationRequest(
    UserId: userId,
    Destination: "user@example.com",
    PurposeCode: "password-reset"
);
```

### Handling Domain Events

```csharp
// Publishing verification completed event
var verificationEvent = new VerificationCompletedEvent
{
    VerificationId = verification.Id,
    UserId = verification.UserId,
    Email = verification.Destination,
    PurposeCode = verification.VerificationPurposeCode,
    CompletedAt = DateTime.UtcNow,
    IsSuccessful = true
};

// Publish to message bus
await _publishEndpoint.Publish(verificationEvent);
```

## 📋 Design Principles

### Clean Architecture Compliance

- **No Implementation Details**: Contains only abstractions and contracts
- **Dependency Inversion**: Higher-level modules depend on these abstractions
- **Stable Interfaces**: Changes to implementations don't affect consumers

### Data Transfer Objects (DTOs)

- **Immutability**: Records ensure data integrity
- **Inheritance**: Extends `BaseDto` for common audit fields
- **Nullability**: Proper nullable annotations for optional fields

### Interface Segregation

- **Focused Interfaces**: Each interface has a single responsibility
- **Async by Default**: All operations are asynchronous with cancellation support
- **Optional Parameters**: Sensible defaults for common scenarios

## 🔄 Event-Driven Design

The abstraction layer supports event-driven architecture through:

- **Domain Events**: `VerificationCompletedEvent` for system integration
- **Message Contracts**: Well-defined event structures
- **Async Communication**: Events can be published to message buses

## 📈 Extensibility

### Adding New Channels

```csharp
public enum VerificationChannel
{
    Email,
    Sms,        // New channel
    Push        // Future channel
}
```

### Extending Request Models

```csharp
public record CreateVerificationRequest(
    Guid UserId,
    VerificationChannel Channel,
    string Destination,
    string VerificationPurposeCode,
    DateTimeOffset? ExpiresAt = null,
    string? CustomField = null  // New optional field
);
```

### Adding New Events

```csharp
public class VerificationFailedEvent
{
    public Guid VerificationId { get; set; }
    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
}
```

## 🧪 Testing

### Interface Testing

```csharp
public class VerificationServiceTests
{
    [Fact]
    public async Task CreateVerificationAsync_ShouldReturnValidDto()
    {
        // Arrange
        var mockService = new Mock<IVerificationService>();
        var request = new CreateVerificationRequest(/* params */);

        // Act
        var result = await mockService.Object.CreateVerificationAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(VerificationStatus.Pending, result.Status);
    }
}
```

### Model Validation

```csharp
[Fact]
public void VerificationDto_ShouldHaveRequiredFields()
{
    var dto = new VerificationDto(/* params */);

    Assert.NotEqual(Guid.Empty, dto.Id);
    Assert.NotEqual(Guid.Empty, dto.UserId);
    Assert.NotNull(dto.Destination);
}
```

## 🛠️ Development Guidelines

### Naming Conventions

- **Interfaces**: Prefix with `I` (e.g., `IVerificationService`)
- **DTOs**: Suffix with `Dto` (e.g., `VerificationDto`)
- **Requests**: Suffix with `Request` (e.g., `CreateVerificationRequest`)
- **Events**: Suffix with `Event` (e.g., `VerificationCompletedEvent`)

### Code Structure

```
src/MasLazu.AspNet.Verification.Abstraction/
├── Enums/              # Domain enumerations
├── Events/             # Domain events
├── Interfaces/         # Service contracts
├── Models/             # DTOs and request models
└── MasLazu.AspNet.Verification.Abstraction.csproj
```

### Best Practices

- Keep interfaces focused and minimal
- Use records for immutable DTOs
- Include XML documentation for public APIs
- Follow async/await patterns consistently
- Use meaningful property names and types

## 🤝 Contributing

1. **Interface Changes**: Consider backward compatibility
2. **New Models**: Follow existing naming and structure patterns
3. **Documentation**: Update XML comments for new members
4. **Testing**: Add tests for new abstractions
5. **Versioning**: Consider semantic versioning for breaking changes

## 📄 License

Part of the MasLazu ASP.NET framework ecosystem.</content>
<parameter name="filePath">/home/mfaziz/projects/cs/MasLazu.AspNet.Verification/src/MasLazu.AspNet.Verification.Abstraction/README.md
