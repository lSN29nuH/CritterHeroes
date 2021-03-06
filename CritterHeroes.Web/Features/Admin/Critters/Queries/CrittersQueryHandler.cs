﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CritterHeroes.Web.Data.Models;
using CritterHeroes.Web.Data.Models.Identity;
using CritterHeroes.Web.Domain.Contracts;
using CritterHeroes.Web.Domain.Contracts.Queries;
using CritterHeroes.Web.Domain.Contracts.Storage;
using CritterHeroes.Web.Features.Admin.Critters.Models;
using TOTD.EntityFramework;

namespace CritterHeroes.Web.Features.Admin.Critters.Queries
{
    public class CrittersQuery : BaseCrittersQuery, IAsyncQuery<CrittersModel>
    {
    }

    public class CrittersQueryHandler : IAsyncQueryHandler<CrittersQuery, CrittersModel>
    {
        private ISqlQueryStorageContext<CritterStatus> _statusStorage;
        private IHttpUser _user;

        public CrittersQueryHandler(ISqlQueryStorageContext<CritterStatus> statusStorage, IHttpUser user)
        {
            this._statusStorage = statusStorage;
            this._user = user;
        }

        public async Task<CrittersModel> ExecuteAsync(CrittersQuery query)
        {
            CrittersModel model = new CrittersModel();

            model.Query = query;
            model.ShowImport = (_user.IsInRole(UserRole.MasterAdmin));

            model.StatusItems = await _statusStorage.Entities
                .OrderBy(x => x.Name)
                .SelectToListAsync(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.Name,
                    Selected = (x.ID == query.StatusID)
                });

            return model;
        }
    }
}
