using System.Collections.Generic;
using Microwave.Domain.Identities;
using Microwave.Domain.Validation;

namespace Domain.Matches.Errors
{
    public class PlayerWasNotPartOfTheTeamWhenStartingTheMatch : DomainError
    {
        public PlayerWasNotPartOfTheTeamWhenStartingTheMatch(IEnumerable<GuidIdentity> playerIdentity) : base($"The progression for players {string.Join(", ", playerIdentity)} are not possible, as they where not part of one team on the creation of this match.")
        {
        }
    }
}