﻿using Maxi.BackOffice.Agent.Infrastructure.OrbographWebService;

namespace Maxi.BackOffice.Agent.Infrastructure.Contracts
{
    public interface IOrbographRepository
    {
        ValidationResponse ValidateCheck(byte[] pic, string picName);
    }
}
