﻿using System;
using Microwave.Queries;

namespace Seasons.ReadHost.Seasons.Events
{
    public class TeamAddedToSeason : ISubscribedDomainEvent
    {
        public TeamAddedToSeason(Guid seasonId, Guid teamId)
        {
            SeasonId = seasonId;
            TeamId = teamId;
        }

        public string EntityId => SeasonId.ToString();
        public Guid SeasonId { get; }
        public Guid TeamId { get; }
    }
}