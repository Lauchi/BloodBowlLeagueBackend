﻿using Microwave.Domain;

namespace Domain.Teams.DomainEvents
{
    public class PlayerBought : IDomainEvent
    {
        public PlayerBought(Identity entityId, StringIdentity playerTypeId, GoldCoins newTeamChestBalance)
        {
            EntityId = entityId;
            NewTeamChestBalance = newTeamChestBalance;
            PlayerTypeId = playerTypeId;
        }

        public Identity EntityId { get; }
        public GoldCoins NewTeamChestBalance { get; }
        public StringIdentity PlayerTypeId { get; }
    }
}