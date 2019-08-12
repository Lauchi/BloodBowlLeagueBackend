using System.Collections.Generic;
using Microwave.Domain.Identities;

namespace Domain.Players
{
    public class PlayerConfig
    {
        public PlayerConfig(
            StringIdentity playerTypeId,
            IEnumerable<Skill> startingSkills,
            IEnumerable<SkillType> skillsOnDefault,
            IEnumerable<SkillType> skillsOnDouble)
        {
            StartingSkills = startingSkills;
            SkillsOnDefault = skillsOnDefault;
            SkillsOnDouble = skillsOnDouble;
            PlayerTypeId = playerTypeId;
        }

        public StringIdentity PlayerTypeId { get; }
        public IEnumerable<Skill> StartingSkills { get; }
        public IEnumerable<SkillType> SkillsOnDefault { get; }
        public IEnumerable<SkillType> SkillsOnDouble { get; }
    }
}