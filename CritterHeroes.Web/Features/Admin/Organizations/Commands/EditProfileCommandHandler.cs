﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Data.Extensions;
using CritterHeroes.Web.Data.Models;
using CritterHeroes.Web.Domain.Contracts.Commands;
using CritterHeroes.Web.Domain.Contracts.Configuration;
using CritterHeroes.Web.Domain.Contracts.StateManagement;
using CritterHeroes.Web.Domain.Contracts.Storage;
using CritterHeroes.Web.Features.Admin.Organizations.Models;
using CritterHeroes.Web.Shared.Commands;
using CritterHeroes.Web.Shared.StateManagement;

namespace CritterHeroes.Web.Features.Admin.Organizations.Commands
{
    public class EditProfileCommandHandler : IAsyncCommandHandler<EditProfileModel>
    {
        private IAppConfiguration _appConfiguration;
        private ISqlCommandStorageContext<Organization> _storageContext;
        private IOrganizationLogoService _logoService;
        private IStateManager<OrganizationContext> _stateManager;

        public EditProfileCommandHandler(IAppConfiguration appConfiguration, ISqlCommandStorageContext<Organization> storageContext, IOrganizationLogoService logoService, IStateManager<OrganizationContext> stateManager)
        {
            this._appConfiguration = appConfiguration;
            this._storageContext = storageContext;
            this._logoService = logoService;
            this._stateManager = stateManager;
        }

        public async Task<CommandResult> ExecuteAsync(EditProfileModel command)
        {
            Organization org = await _storageContext.Entities.FindByIDAsync(_appConfiguration.OrganizationID);
            org.FullName = command.Name;
            org.ShortName = command.ShortName;
            org.EmailAddress = command.Email;

            await _storageContext.SaveChangesAsync();

            if (command.LogoFile != null)
            {
                await _logoService.SaveLogo(command.LogoFile.InputStream, command.LogoFile.FileName, command.LogoFile.ContentType);

                OrganizationContext orgContext = _stateManager.GetContext();
                orgContext.LogoFilename = command.LogoFile.FileName;
                _stateManager.SaveContext(orgContext);
            }

            return CommandResult.Success();
        }
    }
}
