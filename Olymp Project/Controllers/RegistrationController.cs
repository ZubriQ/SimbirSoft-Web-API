using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Services.Registration;

namespace Olymp_Project.Controllers
{
    [Route("registration")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _service;
        private readonly IMapper _mapper;

        public RegistrationController(IRegistrationService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountResponseDto>> CreateAccount(AccountRequestDto account)
        {
            if (!AccountValidator.IsValid(account))
            {
                return BadRequest();
            } 
            // TODO: 403 already authorized

            (HttpStatusCode status, Account? createdAccount) =
                await _service.RegisterAccountAsync(_mapper.Map<Account>(account));

            switch (status)
            {
                case HttpStatusCode.Forbidden:
                    return Forbid();
                case HttpStatusCode.Conflict:
                    return Conflict();
            }
            return CreatedAtAction(nameof(CreateAccount),
                _mapper.Map<AccountResponseDto>(createdAccount));
        }
    }
}
