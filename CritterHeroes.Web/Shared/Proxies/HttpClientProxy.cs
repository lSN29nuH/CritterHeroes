﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using CritterHeroes.Web.Domain.Contracts;

namespace CritterHeroes.Web.Shared.Proxies
{
    public class HttpClientProxy : HttpClient, IHttpClient
    {
    }
}
