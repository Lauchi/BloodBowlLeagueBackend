using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Players.Events.Players;
using Microwave.Domain.Identities;

namespace Domain.Players.UnitTests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void LevelUp_NoSkillAvailable()
        {
            var player = new Player();
            var skillLevelUp = Block();

            var domainResult = player.ChooseSkill(skillLevelUp);

            Assert.IsFalse(domainResult.IsOk);
        }

        [TestMethod]
        public void PlayerLevelsUp()
        {
            var player = new Player();
            var nominateForMostValuablePlayer = player.NominateForMostValuablePlayer();
            player.Apply(nominateForMostValuablePlayer.DomainEvents);
            var result = player.Block();
            player.Apply(result.DomainEvents);

            Assert.AreEqual(2, ((PlayerLeveledUp) result.DomainEvents.Last()).NewLevel);
            Assert.AreEqual(2, player.Level);
        }

        private static SkillReadModel Block()
        {
            return new SkillReadModel
            {
                SkillId = StringIdentity.Create("Block"),
                SkillType = SkillType.General
            };
        }

        private static SkillReadModel PlusOneStrength()
        {
            return new SkillReadModel
            {
                SkillId = StringIdentity.Create("PlusOneStrength"),
                SkillType = SkillType.PlusOneStrength
            };
        }

        private static SkillReadModel Pass()
        {
            return new SkillReadModel
            {
                SkillId = StringIdentity.Create("Pass"),
                SkillType = SkillType.Passing
            };
        }

        [TestMethod]
        public void LevelUp_StrengthSkillAvailable()
        {
            var player = new Player();
            player.Apply(PlayerLeveledUp(new [] { FreeSkillPoint.PlusOneStrength }));
            var skillLevelUp = PlusOneStrength();

            var domainResult = player.ChooseSkill(skillLevelUp);

            Assert.IsTrue(domainResult.IsOk);
            Assert.AreEqual(1, domainResult.DomainEvents.Count());
        }

        [TestMethod]
        public void LevelUp_StrengtSkillAvailable_SkillOfLowerPower()
        {
            var player = new Player();
            player.Apply(PlayerLeveledUp(new []{ FreeSkillPoint.PlusOneStrength }));
            var skillLevelUp = Pass();

            var domainResult = player.ChooseSkill(skillLevelUp);

            Assert.IsTrue(domainResult.IsOk);
            Assert.AreEqual(1, domainResult.DomainEvents.Count());
        }

        [TestMethod]
        public void LevelUp_StrengthWanted_ButOnlyArmorPossible()
        {
            var player = new Player();
            player.Apply(PlayerCreated());
            player.Apply(PlayerLeveledUp(new []{ FreeSkillPoint.PlusOneArmorOrMovement }));
            var skillLevelUp = PlusOneStrength();

            var domainResult = player.ChooseSkill(skillLevelUp);

            Assert.IsFalse(domainResult.IsOk);
        }

        [TestMethod]
        public void LevelUp_DoubleReplacedWithGeneral()
        {
            var player = new Player();
            player.Apply(PlayerCreated());
            player.Apply(PlayerLeveledUp(new []{ FreeSkillPoint.Double }));
            var skillLevelUp = Block();

            var domainResult = player.ChooseSkill(skillLevelUp);

            Assert.IsTrue(domainResult.IsOk);
        }

        [TestMethod]
        public void LevelUp_DoubleUSedForDouble()
        {
            var player = new Player();
            player.Apply(PlayerCreated());
            player.Apply(PlayerLeveledUp(new []{ FreeSkillPoint.Double }));
            var skillLevelUp = Pass();

            var domainResult = player.ChooseSkill(skillLevelUp);

            Assert.IsTrue(domainResult.IsOk);
        }

        [TestMethod]
        public void LevelUp_DoubleTooHigh()
        {
            var player = new Player();
            player.Apply(PlayerCreated());
            player.Apply(PlayerLeveledUp(new []{ FreeSkillPoint.Normal }));
            var skillLevelUp = Pass();

            var domainResult = player.ChooseSkill(skillLevelUp);

            Assert.IsFalse(domainResult.IsOk);
        }

        [TestMethod]
        public void LevelUp_PickSkillTwice()
        {
            var player = new Player();
            player.Apply(PlayerCreated());
            player.Apply(PlayerLeveledUp(new []{ FreeSkillPoint.Normal }));
            player.Apply(PlayerLeveledUp(new []{ FreeSkillPoint.Normal }));
            player.Apply(SkillPicked(Block()));

            var skillLevelUp = Block();

            var domainResult2 = player.ChooseSkill(skillLevelUp);

            Assert.IsFalse(domainResult2.IsOk);
        }

        private static PlayerLeveledUp PlayerLeveledUp(IEnumerable<FreeSkillPoint> freeSkillPoints = null)
        {
            return new PlayerLeveledUp(GuidIdentity.Create(), freeSkillPoints ?? new []{ FreeSkillPoint.PlusOneStrength }, 2);
        }

        private static SkillChosen SkillPicked(SkillReadModel skill)
        {
            return new SkillChosen(GuidIdentity.Create(), skill, new List<FreeSkillPoint>());
        }

        private static PlayerConfig PlayerConfig(
            IEnumerable<SkillType> defaultSkills = null,
            IEnumerable<SkillType> doubleSkills = null)
        {
            var playerConfig = new PlayerConfig(
                StringIdentity.Create("whatever"),
                new List<SkillReadModel>(),
                defaultSkills ?? new[] {SkillType.General},
                doubleSkills ?? new[] {SkillType.Passing});
            return playerConfig;
        }

        private static PlayerCreated PlayerCreated(
            IEnumerable<SkillType> defaultSkills = null,
            IEnumerable<SkillType> doubleSkills = null)
        {
            var playerConfig = PlayerConfig(defaultSkills, doubleSkills);
            return new PlayerCreated(GuidIdentity.Create(), GuidIdentity.Create(), playerConfig, "Peter Wolf");
        }
    }
}