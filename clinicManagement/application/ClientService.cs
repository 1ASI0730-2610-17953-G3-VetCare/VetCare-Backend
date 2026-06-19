using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.repositories;

namespace VetCare.clinicManagement.application;

public class ClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Client> CreateClientAsync(string fullName, string documentId, string phone, string email, string address)
    {
        var code = "CLI" + DateTime.UtcNow.Ticks.ToString().Substring(10);
        var client = new Client(code, fullName, documentId, phone, email, address);
        await _clientRepository.AddAsync(client);
        return client;
    }

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        foreach (var client in clients)
            client.ApplyInactivityRule();

        return clients;
    }
}
