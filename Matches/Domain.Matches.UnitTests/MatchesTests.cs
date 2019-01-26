using System.Linq;
using Domain.Matches.Errors;
using Domain.Matches.Events;
using Domain.Matches.ForeignEvents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microwave.Domain;

namespace Domain.Matches.UnitTests
{
    [TestClass]
    public class MatchesTests
    {
        [TestMethod]
        public void FinishMatchHappyPath()
        {
            var match = new Match();

            var player1Id = GuidIdentity.Create();
            var player2Id = GuidIdentity.Create();
            var player3Id = GuidIdentity.Create();
            var player4Id = GuidIdentity.Create();
            var player5Id = GuidIdentity.Create();
            var teamReadModel = TeamReadModel(player1Id, player2Id);
            var teamReadModel2 = TeamReadModel(player3Id, player4Id, player5Id);

            match.Apply(new MatchCreated(GuidIdentity.Create(), teamReadModel, teamReadModel2));

            var playerProgression1 = PlayerProgressionTouchdown(player2Id);
            var playerProgression2 = PlayerProgressionNormal(player4Id);
            var playerProgressions = new []{ playerProgression1, playerProgression2 };

            var domainResult = match.Finish(playerProgressions);

            Assert.IsTrue(match.IsFinished);
            var domainEvent = domainResult.DomainEvents.First() as MatchFinished;
            Assert.AreEqual(teamReadModel.TeamId, domainEvent.GameResult.Winner.TrainerId);
            Assert.AreEqual(teamReadModel2.TeamId, domainEvent.GameResult.Looser.TrainerId);
        }

        [TestMethod]
        public void FinishMatch_PlayersNotInTeam()
        {
            var match = new Match();

            var player1Id = GuidIdentity.Create();
            var player2Id = GuidIdentity.Create();
            var player3Id = GuidIdentity.Create();
            var teamReadModel = TeamReadModel(player1Id);
            var teamReadModel2 = TeamReadModel(player3Id);

            match.Apply(new MatchCreated(GuidIdentity.Create(), teamReadModel, teamReadModel2));

            var playerProgression1 = PlayerProgressionTouchdown(player2Id);
            var playerProgressions = new []{ playerProgression1 };

            var domainResult = match.Finish(playerProgressions);

            Assert.IsFalse(match.IsFinished);
            Assert.IsTrue(domainResult.DomainErrors.Single().GetType() == typeof(PlayerWasNotPartOfTheTeamOnMatchCreation));
        }

        private static PlayerProgression PlayerProgressionTouchdown(GuidIdentity playerId)
        {
            playerId = playerId ?? GuidIdentity.Create();
            var playerProgression = new PlayerProgression(playerId,
                new[] {ProgressionEvent.PlayerPassed, ProgressionEvent.PlayerMadeTouchdown});
            return playerProgression;
        }

        private static PlayerProgression PlayerProgressionNormal(GuidIdentity playerId)
        {
            playerId = playerId ?? GuidIdentity.Create();
            var playerProgression = new PlayerProgression(playerId,
                new[] {ProgressionEvent.PlayerPassed, ProgressionEvent.PlayerMadeCasualty});
            return playerProgression;
        }

        private static TeamReadModel TeamReadModel(params GuidIdentity[] playerIds)
        {
            var teamReadModel = new TeamReadModel();
            var trainerAsGuest = GuidIdentity.Create();
            teamReadModel.Apply(new TeamCreated(trainerAsGuest));
            foreach (var playerId in playerIds)
            {
                teamReadModel.Apply(new PlayerBought(trainerAsGuest, playerId));
            }
            return teamReadModel;
        }
    }
}