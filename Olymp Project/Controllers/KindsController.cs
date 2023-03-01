﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Kinds;

namespace Olymp_Project.Controllers
{
    [Authorize]
    [Route("animals/types")]
    [ApiController]
    public class KindsController : ControllerBase
    {
        private readonly IKindService _service;
        private readonly IMapper _mapper;

        public KindsController(IKindService kindService, IMapper mapper)
        {
            _service = kindService;
            _mapper = mapper;
        }

        [HttpGet("{kindId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KindResponseDto>> GetKind([FromRoute] long? kindId)
        {
            var kind = await _service.GetAnimalKindAsync(kindId!.Value);

            return Ok(_mapper.Map<KindResponseDto>(kind));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<KindResponseDto>> CreateKind([FromBody] string? name)
        {
            var response = await _service.InsertAnimalKindAsync(name!);

            var kindDto = _mapper.Map<KindResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, kindDto, nameof(CreateKind));
        }

        [HttpPut("{kindId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<KindResponseDto>> UpdateKind(
            [FromRoute] long? kindId,
            [FromBody] string? name)
        {
            var response = await _service.UpdateAnimalKindAsync(kindId!.Value, name!);

            var kindDto = _mapper.Map<KindResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, kindDto);
        }

        [HttpDelete("{kindId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteKind(long? kindId)
        {
            var status = await _service.RemoveAnimalKindAsync(kindId.Value);
            switch(status)
            {
                case HttpStatusCode.BadRequest:
                    return BadRequest();
                case HttpStatusCode.NotFound:
                    return NotFound();
            }
            return Ok();
        }
    }
}
