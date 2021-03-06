﻿using Microwave.Domain.Validation;

namespace Domain.Teams.DomainErrors
{
    public class FewMoneyInTeamChestError : DomainError
    {
        public FewMoneyInTeamChestError(long playerCost, long teamMoney) : base($"Can not buy Player. Player costs {playerCost}, your chest only contains {teamMoney}")
        {
        }
    }
}