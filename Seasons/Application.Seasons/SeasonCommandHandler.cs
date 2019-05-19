using System.Linq;
using System.Threading.Tasks;
using Domain.Seasons;
using Microwave.Domain;
using Microwave.EventStores.Ports;
using Microwave.Queries;

namespace Application.Matches
{
    public class SeasonCommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IReadModelRepository _readModelRepository;

        public SeasonCommandHandler(IEventStore eventStore, IReadModelRepository readModelRepository)
        {
            _eventStore = eventStore;
            _readModelRepository = readModelRepository;
        }

        public async Task StartSeason(StartSeasonCommand command)
        {
            var eventStoreResult = await _eventStore.LoadAsync<Season>(command.SeasonId);
            var season = eventStoreResult.Value;
            var domainResult = season.StartSeason();
            var domainEvents = domainResult.DomainEvents.ToList();
            var result = await _eventStore.AppendAsync(domainEvents, 0);
            result.Check();
        }

        public async Task AddTeamToSeason(AddTeamToSeasonCommand command)
        {
            var seasonResult = await _eventStore.LoadAsync<Season>(command.SeasonId);
            var season = seasonResult.Value;
            var team = (await _readModelRepository.Load<TeamReadModel>(command.TeamId)).Value;
            var domainResult = season.AddTeam(team.TeamId);
            (await _eventStore.AppendAsync(domainResult.DomainEvents, seasonResult.Version)).Check();
        }

        public async Task CreateSeason()
        {
            var domainResult = Season.Create();
            (await _eventStore.AppendAsync(domainResult.DomainEvents, 0)).Check();
        }
    }

    public class AddTeamToSeasonCommand
    {
        public GuidIdentity SeasonId { get; set; }
        public Identity TeamId { get; set; }
    }

    public class StartSeasonCommand
    {
        public StartSeasonCommand(GuidIdentity seasonId)
        {
            SeasonId = seasonId;
        }

        public GuidIdentity SeasonId { get; }
    }
}