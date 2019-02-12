using Microwave.Domain;

namespace Teams.ReadHost.Players.Events
{
    public class PlayerLeveledUp : IDomainEvent
    {
        public PlayerLeveledUp(GuidIdentity entityId, int newLevel)
        {
            EntityId = entityId;
            NewLevel = newLevel;
        }

        public Identity EntityId { get; }
        public int NewLevel { get; }
    }
}