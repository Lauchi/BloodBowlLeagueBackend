﻿using System.Collections.Generic;
using System.Linq;
using Domain.Matches.Errors;
using Domain.Matches.Events;
using Microwave.Domain;

namespace Domain.Matches
{
    public class Match : Entity, IApply<MatchFinished>, IApply<MatchStarted>, IApply<MatchCreated>
    {
        public GuidIdentity MatchId { get; private set; }
        public IEnumerable<GuidIdentity> GuestTeam { get; private set; }
        public IEnumerable<GuidIdentity> HomeTeam { get; private set; }
        public GuidIdentity TeamAtHome { get; private set; }
        public GuidIdentity TeamAsGuest { get; private set; }
        public IEnumerable<PlayerProgression> PlayerProgressions { get; private set; }
        public bool IsFinished { get; private set; }

        public static DomainResult Create(GuidIdentity trainerAtHome, GuidIdentity trainerAsGuest)
        {
            var domainEvents = new MatchCreated(GuidIdentity.Create(), trainerAtHome, trainerAsGuest);
            return DomainResult.Ok(domainEvents);
        }

        public DomainResult Start(TeamReadModel teamAtHome, TeamReadModel teamAsGuest)
        {
            if (TeamAtHome != teamAtHome.TeamId || TeamAsGuest != teamAsGuest.TeamId) return DomainResult.Error
            (new TrainerHaveToBeTheSameAsOnGameCreation(TeamAtHome, TeamAsGuest));
            HomeTeam = teamAtHome.Players;
            GuestTeam = teamAsGuest.Players;
            return DomainResult.Ok(new MatchStarted(MatchId, HomeTeam, GuestTeam));
        }

        public DomainResult Finish(IEnumerable<PlayerProgression> playerProgressions)
        {
            if (IsFinished) return DomainResult.Error(new MatchAllreadyFinished());
            var progressions = playerProgressions.ToList();

            var homeTeamProgression = progressions.Where(p => HomeTeam.Contains(p.PlayerId));
            var guestTeamProgression = progressions.Where(p => GuestTeam.Contains(p.PlayerId));
            var unrelatedPlayers = progressions.Where(p =>
                !HomeTeam.Contains(p.PlayerId) && !GuestTeam.Contains(p.PlayerId)).Select(p => p.PlayerId).ToList();
            if (unrelatedPlayers.Any()) return DomainResult.Error(new PlayerWasNotPartOfTheTeamOnMatchCreation(unrelatedPlayers));

            var homeTouchDowns = CountTouchDowns(homeTeamProgression);
            var guestTouchDowns = CountTouchDowns(guestTeamProgression);

            var gameResult = CreatGameResult(homeTouchDowns, guestTouchDowns);

            var matchResultUploaded = new MatchFinished(MatchId, progressions, gameResult);
            IsFinished = true;
            return DomainResult.Ok(matchResultUploaded);
        }

        private GameResult CreatGameResult(int homeTouchDowns, int guestTouchDowns)
        {
            GameResult gameResult;
            if (homeTouchDowns == guestTouchDowns) gameResult = GameResult.Draw();
            else
            {
                var homeResult = new TrainerGameResult(TeamAtHome, homeTouchDowns);
                var guestResult = new TrainerGameResult(TeamAsGuest, guestTouchDowns);

                gameResult = homeTouchDowns > guestTouchDowns
                    ? GameResult.WinResult(homeResult, guestResult)
                    : GameResult.WinResult(guestResult, homeResult);
            }

            return gameResult;
        }

        private static int CountTouchDowns(IEnumerable<PlayerProgression> trainerResults)
        {
            return trainerResults.Sum(playerProgression => playerProgression.ProgressionEvents.Count(ev => ev == ProgressionEvent.PlayerMadeTouchdown));
        }

        public void Apply(MatchFinished domainEvent)
        {
            IsFinished = true;
            PlayerProgressions = domainEvent.PlayerProgressions;
        }

        public void Apply(MatchStarted domainEvent)
        {
            MatchId = (GuidIdentity) domainEvent.EntityId;
            HomeTeam = domainEvent.HomeTeam;
            GuestTeam = domainEvent.GuestTeam;
        }

        public void Apply(MatchCreated domainEvent)
        {
            MatchId = (GuidIdentity) domainEvent.EntityId;
            TeamAtHome = domainEvent.TrainerAtHome;
            TeamAsGuest = domainEvent.TrainerAsGuest;
        }
    }
}