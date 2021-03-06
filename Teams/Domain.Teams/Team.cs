﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Teams.DomainErrors;
using Domain.Teams.DomainEvents;
using Microwave.Domain.EventSourcing;
using Microwave.Domain.Validation;

namespace Domain.Teams
{
    public class Team : Entity,
        IApply<TeamCreated>,
        IApply<TeamDraftCreated>,
        IApply<PlayerBought>,
        IApply<PlayerRemoved>,
        IApply<PlayerAddedToDraft>
    {
        public Guid TeamId { get; private set; }
        public GoldCoins TeamMoney { get; private set; }
        public IEnumerable<PlayerReadModel> Players { get; private set; } = new List<PlayerReadModel>();
        public IEnumerable<AllowedPlayer> AllowedPlayers { get; private set; } = new List<AllowedPlayer>();
        public string TrainerName { get; private set; }
        public string TeamName { get; private set; }
        public string RaceId { get; private set; }
        private TeamState _teamState = new TeamDraftState();

        public static DomainResult Draft(
            string raceId,
            string teamName,
            string trainerName,
            IEnumerable<AllowedPlayer> allowedPlayers)
        {
            return DomainResult.Ok(new TeamDraftCreated(
                Guid.NewGuid(),
                raceId, 
                teamName,
                trainerName, 
                allowedPlayers, 
                new GoldCoins(1000000)));
        }

        public DomainResult BuyPlayer(string playerTypeId)
        {
            var playerBuyConfig = AllowedPlayers.FirstOrDefault(ap => ap.PlayerTypeId.Equals(playerTypeId));
            if (playerBuyConfig == null) return DomainResult.Error(new CanNotUsePlayerInThisRaceError(playerTypeId));

            var amountOfPlayerTypeToBuy = Players.Count(p => p.PlayerTypeId.Equals(playerTypeId));
            if (amountOfPlayerTypeToBuy >= playerBuyConfig.MaximumPlayers) return DomainResult.Error(new TeamFullError(playerBuyConfig.MaximumPlayers));

            if (playerBuyConfig.Cost.MoreThan(TeamMoney))
                return DomainResult.Error(new FewMoneyInTeamChestError(playerBuyConfig.Cost.Value, TeamMoney.Value));

            var newTeamMoney = TeamMoney.Minus(playerBuyConfig.Cost);
            var orderedPositions = Players.Select(p => p.PlayerPositionNumber).OrderBy(p => p).ToList();
            var travers = FindFirstFreeNumber(orderedPositions, 1);

            var playerBought = _teamState.BoughtEvent(TeamId, playerTypeId, travers, Guid.NewGuid(),
            newTeamMoney);
            Apply(new List<IDomainEvent> {playerBought});
            return DomainResult.Ok(playerBought);
        }

        public int FindFirstFreeNumber(List<int> numbers, int current)
        {
            if (!numbers.Any()) return current;
            var first = numbers.First();
            if (first > current) return current;

            return FindFirstFreeNumber(numbers.Skip(1).ToList(), current + 1);
        }

        public DomainResult CommitDraft()
        {
            if (Players.Count() < 11) return DomainResult.Error(new TeamDoesNeedMorePlayersToFinish(Players.Count()));
            
            var domainEvents = new List<IDomainEvent>();
            domainEvents.Add(new TeamCreated(TeamId, RaceId, TeamName, TrainerName, AllowedPlayers, TeamMoney));
            domainEvents.AddRange(Players.Select(player => new PlayerBought(
                    TeamId,
                    player.PlayerTypeId,
                    player.PlayerPositionNumber,
                    player.PlayerId,
                    TeamMoney)));

            Apply(domainEvents);

            return DomainResult.Ok(domainEvents);
        }

        public DomainResult RemovePlayer(Guid playerId)
        {
            var playerReadModel = Players.Single(p => p.PlayerId == playerId);
            var playerBuyConfig = AllowedPlayers.Single(ap => ap.PlayerTypeId.Equals(playerReadModel.PlayerTypeId));
            var newTeamMoney = TeamMoney.Plus(playerBuyConfig.Cost);

            var playerRemovedFromDraft = new PlayerRemoved(TeamId, playerId, newTeamMoney);
            Apply(playerRemovedFromDraft);
            return DomainResult.Ok(playerRemovedFromDraft);
        }

        public void Apply(PlayerRemoved domainEvent)
        {
            Players = Players.Where(p => p.PlayerId != domainEvent.PlayerId).ToList();
            TeamMoney = domainEvent.NewTeamChestBalance;
        }
        public void Apply(TeamCreated domainEvent)
        {
            TeamMoney = domainEvent.StartingMoney;
            Players = new List<PlayerReadModel>();
            _teamState = new FinalTeamState();
        }

        public void Apply(PlayerBought domainEvent)
        {
            TeamMoney = domainEvent.NewTeamChestBalance;
            var playerReadModel = new PlayerReadModel(domainEvent.PlayerId, domainEvent.PlayerTypeId, domainEvent.PlayerPositionNumber);
            var playerReadModels = Players.ToList();
            playerReadModels.Add(playerReadModel);
            Players = playerReadModels;
        }

        public void Apply(TeamDraftCreated domainEvent)
        {
            TeamMoney = domainEvent.StartingMoney;
            TeamName = domainEvent.TeamName;
            TrainerName = domainEvent.TrainerName;
            RaceId = domainEvent.RaceId;
            AllowedPlayers = domainEvent.AllowedPlayers;
            TeamId = domainEvent.TeamId;
        }

        public void Apply(PlayerAddedToDraft domainEvent)
        {
            TeamMoney = domainEvent.NewTeamChestBalance;
            var playerReadModel = new PlayerReadModel(domainEvent.PlayerId, domainEvent.PlayerTypeId, domainEvent.PlayerPositionNumber);
            var playerReadModels = Players.ToList();
            playerReadModels.Add(playerReadModel);
            Players = playerReadModels;
        }
    }

    internal class FinalTeamState : TeamState
    {
        public override IDomainEvent BoughtEvent(
            Guid teamId,
            string playerTypeId,
            int playerPositionNumber,
            Guid playerId,
            GoldCoins newTeamMoney)
        {
            return new PlayerBought(teamId, playerTypeId, playerPositionNumber, playerId, newTeamMoney);
        }
    }

    internal class TeamDraftState : TeamState
    {
        public override IDomainEvent BoughtEvent(
            Guid teamId,
            string playerTypeId,
            int playerPositionNumber,
            Guid playerId,
            GoldCoins newTeamMoney)
        {
            return new PlayerAddedToDraft(teamId, playerTypeId, playerPositionNumber, playerId, newTeamMoney);
        }
    }

    internal abstract class TeamState
    {
        abstract public IDomainEvent BoughtEvent(
            Guid teamId,
            string playerTypeId,
            int playerPositionNumber,
            Guid playerId,
            GoldCoins newTeamMoney);
    }
}