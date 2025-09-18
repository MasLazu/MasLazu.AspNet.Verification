# MasLazu.AspNet.Verification.Endpoint

The Endpoint layer of the MasLazu ASP.NET Verification system. This project provides REST API endpoints for verification operations using FastEndpoints framework.

## 📋 Overview

This is the presentation layer that exposes HTTP endpoints for the verification system. It implements RESTful APIs using FastEndpoints, providing a clean and efficient way to handle verification requests.

## 🏗️ Architecture

This project represents the **Presentation Layer** in Clean Architecture:

- **Endpoints**: FastEndpoints implementations for API operations
- **Endpoint Groups**: Organized endpoint grouping and configuration
- **Models**: Request/response models specific to the API layer
- **Extensions**: Dependency injection utilities for endpoint registration

## 📦 Dependencies

### Project References

- `MasLazu.AspNet.Verification.Abstraction` - Core interfaces and models

### Package References

- `FastEndpoints` - High-performance API framework
- `MasLazu.AspNet.Framework.Endpoint` - Base endpoint framework
- `Microsoft.AspNetCore.Http` - ASP.NET Core HTTP abstractions

## 🚀 API Endpoints

### Verification Endpoints

#### POST /api/v1/verification/verify

Verifies a verification code and marks it as verified.

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
    "verificationPurposeCode": "registration",
    "status": "Verified",
    "attemptCount": 1,
    "expiresAt": "2025-09-18T10:30:00Z",
    "verifiedAt": "2025-09-18T10:15:00Z",
    "createdAt": "2025-09-18T10:00:00Z",
    "updatedAt": "2025-09-18T10:15:00Z"
  },
  "message": "Verification successful",
  "success": true
}
```

## 🔧 Implementation Details

### Endpoint Structure

#### VerifyEndpoint

Main endpoint for code verification:

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

Organizes verification endpoints under a common group:

```csharp
public class VerificationEndpointGroup : SubGroup<V1EndpointGroup>
{
    public VerificationEndpointGroup()
    {
        Configure("verification", ep => ep.Description(x => x.WithTags("Verification")));
    }
}
```

This creates endpoints under `/api/v1/verification/` path.

### Request Models

#### VerifyRequest

Simple request model for verification:

```csharp
public record VerifyRequest(string Code);
```

### Base Endpoint Features

The endpoints inherit from `BaseEndpoint<TRequest, TResponse>` which provides:

- **Standardized Responses**: Consistent API response format
- **Error Handling**: Built-in error handling and logging
- **Validation**: Automatic request validation
- **Documentation**: OpenAPI/Swagger integration

## 📋 API Response Format

All endpoints return responses in a standardized format:

```json
{
  "data": T,           // Response data
  "message": string,   // Success/error message
  "success": boolean,  // Operation status
  "errors": string[]   // Validation errors (if any)
}
```

## 🔧 Configuration

### Service Registration

```csharp
// Register verification endpoints
services.AddVerificationEndpoints();

// Register FastEndpoints
services.AddFastEndpoints();

// Register Swagger/OpenAPI
services.AddSwaggerDoc();
```

### Endpoint Discovery

FastEndpoints automatically discovers and registers all endpoints that inherit from `BaseEndpoint<,>`.

### Middleware Configuration

```csharp
app.UseFastEndpoints();
app.UseSwaggerGen();
```

## 🚀 Usage Examples

### Basic Verification Flow

1. **Client sends verification request:**

```http
POST /api/v1/verification/verify
Content-Type: application/json

{
  "code": "123456"
}
```

2. **Server processes the request:**

   - Validates the request
   - Calls `IVerificationService.VerifyAsync()`
   - Returns verification result

3. **Client receives response:**

```json
{
  "data": {
    /* verification details */
  },
  "message": "Verification successful",
  "success": true
}
```

### Error Handling

**Invalid Code:**

```json
{
  "data": null,
  "message": "Invalid or expired verification code",
  "success": false,
  "errors": ["Verification code not found or expired"]
}
```

**Validation Error:**

```json
{
  "data": null,
  "message": "Validation failed",
  "success": false,
  "errors": ["Code is required"]
}
```

## 🧪 Testing

### Endpoint Testing

```csharp
[Fact]
public async Task VerifyEndpoint_ShouldReturnSuccess_WhenCodeIsValid()
{
    // Arrange
    var client = CreateClient();
    var request = new { code = "123456" };

    // Act
    var response = await client.PostAsJsonAsync("/api/v1/verification/verify", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<VerificationDto>>();
    result.Success.Should().BeTrue();
    result.Message.Should().Be("Verification successful");
}
```

### Integration Testing

```csharp
[Fact]
public async Task VerifyEndpoint_ShouldUpdateVerificationStatus()
{
    // Arrange
    var verification = await CreateTestVerificationAsync();
    var client = CreateClient();

    // Act
    var response = await client.PostAsJsonAsync("/api/v1/verification/verify",
        new { code = verification.VerificationCode });

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    // Verify database state changed
}
```

## 📈 Extensibility

### Adding New Endpoints

1. **Create Endpoint Class:**

```csharp
public class CreateVerificationEndpoint : BaseEndpoint<CreateVerificationRequest, VerificationDto>
{
    public IVerificationService VerificationService { get; set; }

    public override void ConfigureEndpoint()
    {
        Post("/create");
        Group<VerificationEndpointGroup>();
    }

    public override async Task HandleAsync(CreateVerificationRequest req, CancellationToken ct)
    {
        var result = await VerificationService.CreateVerificationAsync(req.UserId, req, ct);
        await SendCreatedResponseAsync(result, "Verification created", ct);
    }
}
```

2. **Add to Endpoint Group:**

```csharp
public class VerificationEndpointGroup : SubGroup<V1EndpointGroup>
{
    public VerificationEndpointGroup()
    {
        Configure("verification", ep => ep.Description(x => x.WithTags("Verification")));
    }
}
```

3. **Create Request Model:**

```csharp
public record CreateVerificationRequest(
    Guid UserId,
    string Destination,
    string PurposeCode
);
```

### Custom Response Format

Override response handling in endpoint:

```csharp
public override async Task HandleAsync(VerifyRequest req, CancellationToken ct)
{
    var result = await VerificationService.VerifyAsync(req.Code, ct);

    var customResponse = new
    {
        verificationId = result.Id,
        status = result.Status.ToString(),
        verifiedAt = result.VerifiedAt
    };

    await SendAsync(customResponse, StatusCodes.Status200OK, ct);
}
```

### Adding Authentication/Authorization

```csharp
public override void ConfigureEndpoint()
{
    Post("/verify");
    Group<VerificationEndpointGroup>();
    // Require authentication
    // Policies, Roles, etc.
}
```

## 🔒 Security Considerations

### Input Validation

- All requests are automatically validated using FastEndpoints validation
- Custom validators can be added for complex business rules

### Rate Limiting

Consider implementing rate limiting for verification endpoints:

```csharp
public override void ConfigureEndpoint()
{
    Post("/verify");
    Group<VerificationEndpointGroup>();
    AllowAnonymous();
    // Add rate limiting attributes
}
```

### CORS Configuration

Configure CORS policies for cross-origin requests:

```csharp
services.AddCors(options =>
{
    options.AddPolicy("VerificationPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## 📊 Monitoring & Logging

### Request Logging

FastEndpoints automatically logs requests and responses. Additional logging:

```csharp
public override async Task HandleAsync(VerifyRequest req, CancellationToken ct)
{
    _logger.LogInformation("Processing verification request for code: {Code}", req.Code);

    var result = await VerificationService.VerifyAsync(req.Code, ct);

    _logger.LogInformation("Verification completed for user: {UserId}", result.UserId);

    await SendOkResponseAsync(result, "Verification successful", ct);
}
```

### Performance Monitoring

- Response times are automatically tracked
- Custom metrics can be added using middleware

## 🛠️ Development Guidelines

### Code Structure

```
src/MasLazu.AspNet.Verification.Endpoint/
├── Endpoints/          # FastEndpoints implementations
├── EndpointGroups/     # Endpoint organization
├── Extensions/         # DI utilities
├── Models/             # API-specific models
└── MasLazu.AspNet.Verification.Endpoint.csproj
```

### Naming Conventions

- **Endpoints**: Suffix with `Endpoint` (e.g., `VerifyEndpoint`)
- **Groups**: Suffix with `EndpointGroup` (e.g., `VerificationEndpointGroup`)
- **Requests**: Suffix with `Request` (e.g., `VerifyRequest`)

### Best Practices

- Keep endpoints focused on single operations
- Use dependency injection for services
- Return consistent response formats
- Include comprehensive error handling
- Document endpoints with XML comments

## 🤝 Contributing

1. **New Endpoints**: Follow existing patterns and naming conventions
2. **Testing**: Add unit and integration tests for new endpoints
3. **Documentation**: Update API documentation for new endpoints
4. **Security**: Consider authentication and authorization requirements
5. **Performance**: Monitor and optimize endpoint performance

## 📄 License

Part of the MasLazu ASP.NET framework ecosystem.</content>
<parameter name="filePath">/home/mfaziz/projects/cs/MasLazu.AspNet.Verification/src/MasLazu.AspNet.Verification.Endpoint/README.md
