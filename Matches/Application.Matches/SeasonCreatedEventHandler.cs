using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Matches;
using Microwave.Domain;
using Microwave.EventStores.Ports;
using Microwave.Queries;

namespace Application.Matches
{
    public class SeasonCreatedEventHandler : IHandleAsync<SeasonStarted>
    {
        private readonly IEventStore _eventStore;

        public SeasonCreatedEventHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        public async Task HandleAsync(SeasonStarted domainEvent)
        {
            foreach (var gameDay in domainEvent.GameDays)
            {
                foreach (var matchup in gameDay.Matchups)
                {
                    var domainEvents = Matchup.Create(matchup.TeamAtHome, matchup.TeamAsGuest).DomainEvents;
                    var result = await _eventStore.AppendAsync(domainEvents, 0);
                    result.Check();
                }
            }
        }
    }

    public class SeasonStarted
    {
        public SeasonStarted(IEnumerable<GameDay> gameDays)
        {
            GameDays = gameDays;
        }

        public IEnumerable<GameDay> GameDays { get; }
    }

    public class GameDay
    {
        public IEnumerable<MatchupReadModel> Matchups { get; set; }
    }

    public class MatchupReadModel
    {
        public GuidIdentity TeamAtHome { get; set; }
        public GuidIdentity TeamAsGuest { get; set; }
    }
}