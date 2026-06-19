using VetCare.communication.domain.model.aggregates;
using VetCare.communication.domain.repositories;

namespace VetCare.communication.application;

public class DirectMessageCommandService
{
    private readonly IDirectMessageRepository _repository;

    public DirectMessageCommandService(IDirectMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<DirectMessage> SendMessageAsync(int clientId, int veterinarianId, string channel, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content cannot be empty.");
        }

        // Simulación de envío a proveedor externo (WhatsApp/Email)
        // Aquí iría el código HTTP para Twilio, SendGrid, etc.
        // await externalProvider.SendAsync(...)
        
        var message = new DirectMessage(clientId, veterinarianId, channel, content);
        
        // Guardar el registro en la base de datos para historial
        await _repository.AddAsync(message);
        
        return message;
    }
}
