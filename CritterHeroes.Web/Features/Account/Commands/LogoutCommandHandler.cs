﻿using System;
using System.Collections.Generic;
using System.Linq;
using CritterHeroes.Web.Domain.Contracts.Commands;
using CritterHeroes.Web.Domain.Contracts.StateManagement;
using CritterHeroes.Web.Features.Account.Models;
using CritterHeroes.Web.Shared.Commands;
using CritterHeroes.Web.Shared.StateManagement;
using Microsoft.Owin.Security;

namespace CritterHeroes.Web.Features.Account.Commands
{
    public class LogoutCommandHandler : ICommandHandler<LogoutModel>
    {
        private IAuthenticationManager _authenticationManager;
        private IStateManager<UserContext> _stateManager;

        public LogoutCommandHandler(IAuthenticationManager authenticationManager, IStateManager<UserContext> stateManager)
        {
            this._authenticationManager = authenticationManager;
            this._stateManager = stateManager;
        }

        public CommandResult Execute(LogoutModel command)
        {
            _authenticationManager.SignOut();
            _stateManager.ClearContext();

            return CommandResult.Success();
        }
    }
}
