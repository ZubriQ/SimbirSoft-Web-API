﻿using Olymp_Project.Responses;

namespace Olymp_Project.Services.Registration
{
    public interface IRegistrationService
    {
        Task<IServiceResponse<Account>> RegisterAccountAsync(RegistrationRequestDto request);
    }
}
