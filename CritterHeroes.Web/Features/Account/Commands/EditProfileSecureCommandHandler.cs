﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Data.Models.Identity;
using CritterHeroes.Web.Domain.Contracts;
using CritterHeroes.Web.Domain.Contracts.Commands;
using CritterHeroes.Web.Domain.Contracts.Email;
using CritterHeroes.Web.Domain.Contracts.Events;
using CritterHeroes.Web.Domain.Contracts.Identity;
using CritterHeroes.Web.Domain.Contracts.StateManagement;
using CritterHeroes.Web.Features.Account.Models;
using CritterHeroes.Web.Features.Shared.ActionExtensions;
using CritterHeroes.Web.Models.LogEvents;
using CritterHeroes.Web.Shared.Commands;
using CritterHeroes.Web.Shared.StateManagement;
using Microsoft.AspNet.Identity;

namespace CritterHeroes.Web.Features.Account.Commands
{
    public class EditProfileSecureCommandHandler : IAsyncCommandHandler<EditProfileSecureModel>
    {
        private IAppUserManager _userManager;
        private IAppEventPublisher _publisher;
        private IHttpUser _httpUser;
        private IStateManager<UserContext> _userContextManager;
        private IUrlGenerator _urlGenerator;
        private IEmailService<ConfirmEmailEmailCommand> _emailService;

        public EditProfileSecureCommandHandler(IAppUserManager userManager, IAppEventPublisher publisher, IHttpUser httpUser, IStateManager<UserContext> userContextManager, IUrlGenerator urlGenerator, IEmailService<ConfirmEmailEmailCommand> emailService)
        {
            this._userManager = userManager;
            this._publisher = publisher;
            this._httpUser = httpUser;
            this._userContextManager = userContextManager;
            this._urlGenerator = urlGenerator;
            this._emailService = emailService;
        }

        public async Task<CommandResult> ExecuteAsync(EditProfileSecureModel command)
        {
            AppUser user = await _userManager.FindByNameAsync(_httpUser.Username);
            user.Person.NewEmail = command.NewEmail;
            user.EmailConfirmed = false;

            IdentityResult identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                return CommandResult.Failed(identityResult.Errors);
            }

            UserContext userContext = UserContext.FromUser(user);
            _userContextManager.SaveContext(userContext);

            _publisher.Publish(UserLogEvent.Action("Email changed from {OldEmail} to {NewEmail}", user.Email, command.NewEmail));

            ConfirmEmailEmailCommand emailCommand = new ConfirmEmailEmailCommand(command.NewEmail);
            emailCommand.TokenLifespan = _userManager.TokenLifespan;
            emailCommand.Token = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            emailCommand.UrlConfirm = _urlGenerator.GenerateConfirmEmailAbsoluteUrl(command.NewEmail, emailCommand.Token);
            await _emailService.SendEmailAsync(emailCommand);

            return CommandResult.Success();
        }
    }
}
