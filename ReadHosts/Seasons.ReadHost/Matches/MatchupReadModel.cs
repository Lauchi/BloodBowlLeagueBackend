﻿using System;
using System.Collections.Generic;
using Microwave.Domain.Identities;
using Microwave.Queries;
using Seasons.ReadHost.Matches.Events;

namespace Seasons.ReadHost.Matches
{
    public class MatchupReadModel : ReadModel,
        IHandle<MatchFinished>,
        IHandle<MatchStarted>,
        IHandle<MatchCreated>
    {
        public GuidIdentity MatchId { get; private set; }
        public IEnumerable<GuidIdentity> HomeTeamPlayers { get; private set; }
        public IEnumerable<GuidIdentity> GuestTeamPlayers { get; private set; }
        public GuidIdentity TeamAsGuest { get; private set; }
        public GuidIdentity TeamAtHome { get; private set; }
        public bool IsFinished { get; private set; }

        public void Handle(MatchFinished domainEvent)
        {
            IsFinished = true;
        }

        public void Handle(MatchStarted domainEvent)
        {
            HomeTeamPlayers = domainEvent.HomeTeam;
            GuestTeamPlayers = domainEvent.GuestTeam;
        }

        public void Handle(MatchCreated domainEvent)
        {
            MatchId = domainEvent.MatchId;
            TeamAtHome = domainEvent.TeamAtHome;
            TeamAsGuest = domainEvent.TeamAsGuest;
        }

        public override Type GetsCreatedOn => typeof(MatchCreated);
    }
}