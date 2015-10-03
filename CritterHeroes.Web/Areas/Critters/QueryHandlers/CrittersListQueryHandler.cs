﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Areas.Critters.Models;
using CritterHeroes.Web.Areas.Critters.Queries;
using CritterHeroes.Web.Contracts.Queries;
using CritterHeroes.Web.Contracts.Storage;
using CritterHeroes.Web.Data.Models;

namespace CritterHeroes.Web.Areas.Critters.QueryHandlers
{
    public class CrittersListQueryHandler : IAsyncQueryHandler<CrittersListQuery, CrittersListModel>
    {
        private ISqlStorageContext<Critter> _critterStorage;

        public CrittersListQueryHandler(ISqlStorageContext<Critter> critterStorage)
        {
            this._critterStorage = critterStorage;
        }

        public async Task<CrittersListModel> RetrieveAsync(CrittersListQuery query)
        {
            CrittersListModel model = new CrittersListModel();

            model.Critters = await
            (
                from x in _critterStorage.Entities
                select new CritterModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    Status = x.Status.Name,
                    Breed = x.Breed.BreedName,
                    PictureFilename = x.Pictures.FirstOrDefault(p => p.Picture.DisplayOrder == 1).Picture.Filename
                }
            ).ToListAsync();

            return model;
        }
    }
}