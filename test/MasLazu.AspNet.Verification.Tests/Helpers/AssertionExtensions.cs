using FluentAssertions;
using FluentAssertions.Execution;
using MasLazu.AspNet.Verification.Abstraction.Models;

namespace MasLazu.AspNet.Verification.Tests.Helpers;

/// <summary>
/// Extension methods for common test assertions
/// </summary>
public static class AssertionExtensions
{
    public static void ShouldBeValidVerificationDto(this VerificationDto dto)
    {
        using (new AssertionScope())
        {
            dto.Should().NotBeNull();
            dto.Id.Should().NotBeEmpty();
            dto.UserId.Should().NotBeEmpty();
            dto.Destination.Should().NotBeNullOrEmpty();
            dto.VerificationCode.Should().NotBeNullOrEmpty();
            dto.VerificationPurposeCode.Should().NotBeNullOrEmpty();
            dto.ExpiresAt.Should().BeAfter(DateTimeOffset.MinValue);
            dto.CreatedAt.Should().BeAfter(DateTimeOffset.MinValue);
        }
    }

    public static void ShouldBe6DigitCode(this string code)
    {
        using (new AssertionScope())
        {
            code.Should().NotBeNullOrEmpty();
            code.Length.Should().Be(6);
            code.Should().MatchRegex(@"^\d{6}$");
            int.Parse(code).Should().BeInRange(100000, 999999);
        }
    }

    public static void ShouldBeCloseTo(this DateTimeOffset actual, DateTimeOffset expected, TimeSpan precision)
    {
        double difference = Math.Abs((actual - expected).TotalMilliseconds);
        difference.Should().BeLessThanOrEqualTo(precision.TotalMilliseconds);
    }
}
