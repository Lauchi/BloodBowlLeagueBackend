using System;
using Microwave.Queries;

namespace Seasons.ReadHost.Matches.Events
{
    public class MatchCreated : ISubscribedDomainEvent
    {
        public string EntityId => MatchId.ToString();
        public Guid TeamAtHome { get; }
        public Guid TeamAsGuest { get; }
        public Guid MatchId { get; }

        public MatchCreated(Guid matchId, Guid teamAtHome, Guid teamAsGuest)
        {
            MatchId = matchId;
            TeamAtHome = teamAtHome;
            TeamAsGuest = teamAsGuest;
        }
    }
}