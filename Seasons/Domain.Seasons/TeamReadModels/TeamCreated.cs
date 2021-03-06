﻿using System;
using Microwave.Queries;

namespace Domain.Seasons.TeamReadModels
{
    public class TeamCreated : ISubscribedDomainEvent
    {
        public TeamCreated(Guid teamId)
        {
            TeamId = teamId;
        }
        public Guid TeamId { get; }
        public string EntityId => TeamId.ToString();
    }
}