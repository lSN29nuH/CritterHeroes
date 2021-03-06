﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Data.Extensions;
using CritterHeroes.Web.Data.Models;
using CritterHeroes.Web.Domain.Contracts.Configuration;
using CritterHeroes.Web.Domain.Contracts.Queries;
using CritterHeroes.Web.Domain.Contracts.Storage;
using CritterHeroes.Web.Features.Admin.Organizations.Models;

namespace CritterHeroes.Web.Features.Admin.Organizations.Queries
{
    public class EditProfileQuery : IAsyncQuery<EditProfileModel>
    {
    }

    public class EditProfileQueryHandler : IAsyncQueryHandler<EditProfileQuery, EditProfileModel>
    {
        private IAppConfiguration _appConfiguration;
        private ISqlQueryStorageContext<Organization> _storageContext;
        private IOrganizationLogoService _logoService;

        public EditProfileQueryHandler(IAppConfiguration appConfiguration, ISqlQueryStorageContext<Organization> storageContext, IOrganizationLogoService logoService)
        {
            this._appConfiguration = appConfiguration;
            this._storageContext = storageContext;
            this._logoService = logoService;
        }

        public async Task<EditProfileModel> ExecuteAsync(EditProfileQuery query)
        {
            Organization org = await _storageContext.Entities.FindByIDAsync(_appConfiguration.OrganizationID);

            return new EditProfileModel()
            {
                Name = org.FullName,
                ShortName = org.ShortName,
                Email = org.EmailAddress,
                LogoUrl = _logoService.GetLogoUrl()
            };
        }
    }
}
