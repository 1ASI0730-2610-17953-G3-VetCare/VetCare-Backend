using VetCare.backoffice.domain.model.aggregates;
using VetCare.backoffice.domain.repositories;

namespace VetCare.backoffice.application;

public class EconomicsService
{
    private readonly IEntryRepository _entryRepository;

    public EconomicsService(IEntryRepository entryRepository)
    {
        _entryRepository = entryRepository;
    }

    public async Task<IEnumerable<Entry>> GetAllEntriesAsync() => await _entryRepository.GetAllAsync();

    public async Task<Entry> CreateEntryAsync(string type, string category, decimal amount, DateTime date, string description)
    {
        var entry = new Entry(type, category, amount, date, description);
        await _entryRepository.AddAsync(entry);
        return entry;
    }
}
