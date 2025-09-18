namespace MasLazu.AspNet.Verification.Abstraction.Events;

public class VerificationCompletedEvent
{
    public Guid VerificationId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PurposeCode { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public bool IsSuccessful { get; set; }
}
