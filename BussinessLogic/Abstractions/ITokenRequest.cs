﻿using Domain;
using Domain.Response;
using Microsoft.Extensions.Primitives;

namespace BussinessLogic;


public interface ITokenRequest
{
    public Task<dynamic> GetAccessToken(Dictionary<string, StringValues> token);

}


