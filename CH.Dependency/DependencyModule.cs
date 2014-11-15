﻿using System;
using CH.Azure;
using CH.Azure.Identity;
using CH.Domain.Contracts;
using CH.Domain.Contracts.Configuration;
using CH.Domain.Contracts.Email;
using CH.Domain.Contracts.Identity;
using CH.Domain.Handlers;
using CH.Domain.Identity;
using CH.Domain.Models;
using CH.Domain.Proxies;
using CH.Domain.Proxies.Configuration;
using CH.Domain.Proxies.Email;
using CH.Domain.StateManagement;
using CH.RescueGroups;
using CH.RescueGroups.Configuration;
using Microsoft.AspNet.Identity;
using Ninject;
using Ninject.Modules;

namespace CH.Dependency
{
    public class DependencyModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IHttpContext>().To<HttpContextProxy>();

            Bind<IAppConfiguration>().To<AppConfiguration>();
            Bind<IAzureConfiguration>().To<AzureConfiguration>();
            Bind<IRescueGroupsConfiguration>().To<RescueGroupsConfiguration>();

            Bind<IStorageContext<Organization>>().To<OrganizationAzureStorage>();
            Bind<IStateManager<OrganizationContext>>().To<OrganizationStateManager>();

            Bind<IEmailConfiguration>().To<EmailConfiguration>();
            Bind<IEmailClient>().To<EmailClientProxy>();

            Bind<IUserStore<IdentityUser>>().To<UserStore>();
            Bind<IApplicationUserManager>().To<ApplicationUserManager>();
        }
    }
}
