﻿using System.Threading.Tasks;
using Application.Players;
using Microsoft.AspNetCore.Mvc;
using Microwave.Domain;

namespace WebApi.Players
{
    [Route("api/players")]
    public class PlayerController : Controller
    {
        private readonly PlayerCommandHandler _commandHandler;

        public PlayerController(PlayerCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        [HttpPost("{playerId}/level-up")]
        public async Task<ActionResult> CreateTeam(GuidIdentity playerId, [FromBody] LevelUpPlayerComand createTeamCommand)
        {
            await _commandHandler.LevelUp(playerId, createTeamCommand);
            return Ok();
        }
    }
}