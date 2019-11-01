using System;
using Microwave.Queries;

namespace Teams.ReadHost.Teams.Events
{
    public class PlayerAddedToDraft : ISubscribedDomainEvent
    {
        public PlayerAddedToDraft(
            Guid teamId,
            string playerTypeId,
            Guid playerId,
            GoldCoins newTeamChestBalance)
        {
            TeamId = teamId;
            NewTeamChestBalance = newTeamChestBalance;
            PlayerTypeId = playerTypeId;
            PlayerId = playerId;
        }

        public string EntityId => TeamId.ToString();
        public Guid TeamId { get; }
        public GoldCoins NewTeamChestBalance { get; }
        public string PlayerTypeId { get; }
        public Guid PlayerId { get; }
    }
}