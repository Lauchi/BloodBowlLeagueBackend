using Microwave.Queries;

namespace Teams.ReadHost.Races
{
    public class SkillReadModel : ReadModel<SkillCreated>, IHandle<SkillCreated>
    {
        public string SkillId { get; set; }
        public SkillType SkillType { get; set; }

        public void Handle(SkillCreated domainEvent)
        {
            SkillId = domainEvent.SkillId;
            SkillType = domainEvent.SkillType;
        }
    }
}