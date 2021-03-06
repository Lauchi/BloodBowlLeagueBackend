﻿using System;
using Microwave.Queries;

namespace Seasons.ReadHost.Seasons.Events
{
    public class SeasonCreated : ISubscribedDomainEvent
    {
        public string EntityId => SeasonId.ToString();
        public Guid SeasonId { get; }
        public string SeasonName { get; }
        public DateTimeOffset CreationDate { get; }


        public SeasonCreated(Guid seasonId, string seasonName, DateTimeOffset creationDate)
        {
            SeasonId = seasonId;
            SeasonName = seasonName;
            CreationDate = creationDate;
        }
    }
}