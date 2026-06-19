using VetCare.shared.domain;

namespace VetCare.communication.domain.model.aggregates;

public class DirectMessage : AggregateRoot
{
    public int Id { get; private set; }
    public int ClientId { get; private set; }
    public int VeterinarianId { get; private set; }
    public string Channel { get; private set; } // "WhatsApp", "Email"
    public string Content { get; private set; }
    public DateTime SentAt { get; private set; }
    public string Status { get; private set; } // "Sent", "Failed"

    protected DirectMessage() {}

    public DirectMessage(int clientId, int veterinarianId, string channel, string content)
    {
        ClientId = clientId;
        VeterinarianId = veterinarianId;
        Channel = channel;
        Content = content;
        SentAt = DateTime.UtcNow;
        Status = "Sent"; // Simulated status
    }
}
