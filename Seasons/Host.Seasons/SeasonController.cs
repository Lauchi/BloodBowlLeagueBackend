﻿using System;
using System.Threading.Tasks;
using Application.Matches;
using Microsoft.AspNetCore.Mvc;

namespace Host.Matches
{
    [Route("api/seasons")]
    public class SeasonController : Controller
    {
        private readonly SeasonCommandHandler _commandHandler;

        public SeasonController(SeasonCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        [HttpPost("{seasonId}/start")]
        public async Task<ActionResult> StartSeason(Guid seasonId)
        {
            var command = new StartSeasonCommand(seasonId);
            await _commandHandler.StartSeason(command);
            return Ok();
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateSeason([FromBody] CreateSeasonCommand command)
        {
            var result = await _commandHandler.CreateSeason(command);
            return Created($"{Request.Scheme}{Request.Path}/seasons/{result}", null);
        }

        [HttpPost("{seasonId}/add-team")]
        public async Task<ActionResult> AddTeam(Guid seasonId, [FromBody] AddTeamToSeasonCommand command)
        {
            command.SeasonId = seasonId;
            await _commandHandler.AddTeamToSeason(command);
            return Ok();
        }
    }
}