﻿using System.Collections.Generic;
using Domain.Teams.DomainEvents;
using Microwave.Domain;

namespace Domain.Teams
{
    public class RaceConfig : Entity, IApply<RaceCreated>
    {
        public IEnumerable<AllowedPlayer> AllowedPlayers { get; private set; } = new List<AllowedPlayer>();
        public StringIdentity Id { get; private set; }

        public void Apply(RaceCreated raceCreated)
        {
            Id = (StringIdentity) raceCreated.EntityId;
            AllowedPlayers = raceCreated.AllowedPlayers;
        }
    }
}