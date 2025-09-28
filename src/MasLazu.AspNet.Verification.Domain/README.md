# MasLazu.AspNet.Verification.Domain

The Domain layer of the MasLazu ASP.NET Verification system. This project contains the core business entities, domain logic, and domain services that represent the heart of the verification system.

## 📋 Overview

This is the domain layer that encapsulates the business logic and domain entities. It contains the core business rules, entities, value objects, and domain services that define the verification system's behavior.

## 🏗️ Architecture

This project represents the **Domain Layer** in Clean Architecture:

- **Entities**: Core business entities with identity and state
- **Value Objects**: Immutable objects that describe characteristics
- **Domain Services**: Business logic that doesn't naturally fit in entities
- **Domain Events**: Events that represent business occurrences
- **Specifications**: Business rules and query specifications

## 📦 Dependencies

### Package References

- `MasLazu.AspNet.Framework.Domain` - Base domain framework with common domain patterns

### Project References

- `MasLazu.AspNet.Verification.Abstraction` - Core abstractions and contracts

## 🚀 Core Components

### Entities

#### Verification

The main verification entity representing a verification request:

```csharp
public class Verification : EntityBase
{
    public Guid UserId { get; private set; }
    public string VerificationCode { get; private set; }
    public VerificationPurpose Purpose { get; private set; }
    public VerificationStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public int AttemptCount { get; private set; }
    public int MaxAttempts { get; private set; }

    // Domain methods for business logic
    public bool IsExpired() => DateTimeOffset.UtcNow > ExpiresAt;
    public bool CanAttempt() => AttemptCount < MaxAttempts && !IsExpired();
    public void RecordAttempt() => AttemptCount++;
    public void MarkAsVerified() => Status = VerificationStatus.Verified;
}
```

#### VerificationPurpose

Entity representing different verification purposes:

```csharp
public class VerificationPurpose : EntityBase
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }
    public TimeSpan DefaultExpiration { get; private set; }
    public int DefaultMaxAttempts { get; private set; }
    public bool IsActive { get; private set; }
}
```

### Enums

#### VerificationStatus

```csharp
public enum VerificationStatus
{
    Pending = 0,
    Verified = 1,
    Expired = 2,
    Failed = 3
}
```

#### VerificationChannel

```csharp
public enum VerificationChannel
{
    Email = 0,
    Sms = 1
}
```

## 🔧 Usage

The domain layer is typically used by the application layer services. Here's an example of how the domain entities might be used:

```csharp
// Create a new verification
var verification = new Verification(
    userId: userId,
    code: generatedCode,
    purpose: purpose,
    expiresAt: DateTimeOffset.UtcNow.AddMinutes(15),
    maxAttempts: 3
);

// Check if verification is valid
if (verification.CanAttempt() && !verification.IsExpired())
{
    verification.RecordAttempt();

    if (verification.VerificationCode == providedCode)
    {
        verification.MarkAsVerified();
        // Save changes...
    }
}
```

## 📋 Business Rules

The domain layer enforces the following business rules:

- Verifications expire after a configurable time period
- Users have limited attempts to verify codes
- Once verified, a verification cannot be reused
- Verification codes must be unique and cryptographically secure
- Different purposes can have different expiration times and attempt limits

## 🔄 Domain Events

The domain layer publishes events for important business occurrences:

- `VerificationCreated` - When a new verification is created
- `VerificationVerified` - When a verification is successfully completed
- `VerificationExpired` - When a verification expires
- `VerificationFailed` - When verification attempts are exhausted

## 🧪 Testing

The domain layer is designed to be easily testable with pure unit tests, as it contains no external dependencies. Domain logic can be tested in isolation from infrastructure concerns.

## 📚 Related Documentation

- [Abstraction Layer](../MasLazu.AspNet.Verification.Abstraction/) - Core contracts and interfaces
- [Application Layer](../MasLazu.AspNet.Verification/) - Business logic and services
- [Infrastructure Layer](../MasLazu.AspNet.Verification.EfCore/) - Data access implementation
- [API Layer](../MasLazu.AspNet.Verification.Endpoint/) - REST API endpoints
