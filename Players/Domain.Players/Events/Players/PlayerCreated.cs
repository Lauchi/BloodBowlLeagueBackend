using Microwave.Domain.EventSourcing;
using Microwave.Domain.Identities;

namespace Domain.Players.Events.Players
{
    public class PlayerCreated : IDomainEvent
    {
        public PlayerCreated(
            GuidIdentity playerId,
            GuidIdentity teamId,
            PlayerConfig playerConfig,
            string name)
        {
            PlayerId = playerId;
            TeamId = teamId;
            PlayerConfig = playerConfig;
            Name = name;
        }

        public string Name { get; }
        public Identity EntityId => PlayerId;
        public GuidIdentity TeamId { get; }
        public PlayerConfig PlayerConfig { get; }
        public GuidIdentity PlayerId { get; }
    }
}