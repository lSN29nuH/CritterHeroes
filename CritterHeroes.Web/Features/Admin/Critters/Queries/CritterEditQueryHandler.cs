﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Data.Extensions;
using CritterHeroes.Web.Data.Models;
using CritterHeroes.Web.Domain.Contracts.Queries;
using CritterHeroes.Web.Domain.Contracts.Storage;
using CritterHeroes.Web.Features.Admin.Critters.Models;

namespace CritterHeroes.Web.Features.Admin.Critters.Queries
{
    public class CritterEditQuery : IAsyncQuery<CritterEditModel>
    {
        public int CritterID
        {
            get;
            set;
        }
    }

    public class CritterEditQueryHandler : IAsyncQueryHandler<CritterEditQuery, CritterEditModel>
    {
        private ISqlQueryStorageContext<Critter> _storageCritters;

        public CritterEditQueryHandler(ISqlQueryStorageContext<Critter> storageCritters)
        {
            this._storageCritters = storageCritters;
        }

        public async Task<CritterEditModel> ExecuteAsync(CritterEditQuery query)
        {
            Critter critter = await _storageCritters.Entities.FindByIDAsync(query.CritterID);

            CritterEditModel model = new CritterEditModel()
            {
                ID = critter.ID,
                Name = critter.Name
            };

            return model;
        }
    }
}
