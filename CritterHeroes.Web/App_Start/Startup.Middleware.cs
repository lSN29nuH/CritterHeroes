﻿using System;
using System.Web.Mvc;
using CritterHeroes.Web.Middleware;
using Owin;

namespace CritterHeroes.Web
{
    public partial class Startup
    {
        public void ConfigureMiddleware(IAppBuilder app)
        {
            app.UseOrganizationContext(DependencyResolver.Current);
            app.UseUserContext(DependencyResolver.Current);
        }
    }
}