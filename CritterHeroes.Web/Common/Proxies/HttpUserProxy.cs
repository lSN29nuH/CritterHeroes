﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using CritterHeroes.Web.Common.Identity;
using CritterHeroes.Web.Contracts;
using Microsoft.Owin;

namespace CritterHeroes.Web.Common.Proxies
{
    public class HttpUserProxy:IHttpUser
    {
        private IOwinContext _owinContext;

        public HttpUserProxy(IOwinContext owinContext)
        {
            this._owinContext = owinContext;
        }

        private IPrincipal User
        {
            get
            {
                return _owinContext.Request.User;
            }
        }

        public string Username
        {
            get
            {
                return User.Identity.Name;
            }
        }

        public string UserID
        {
            get
            {
                ClaimsPrincipal claimsPrincipal = User as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                return claimsPrincipal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.UserID).Value;
            }
        }

        public bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }
    }
}