﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using CritterHeroes.Web.Data.Models;
using TOTD.EntityFramework;

namespace CritterHeroes.Web.Data.Configurations
{
    public class PersonConfiguration : EntityTypeConfiguration<Person>
    {
        public PersonConfiguration()
        {
            HasKey(x => x.ID);

            Property(x => x.ID).IsRequired().IsIdentity();
            Property(x => x.FirstName).HasMaxLength(100);
            Property(x => x.LastName).HasMaxLength(100);
            Property(x => x.City).HasMaxLength(100);
            Property(x => x.State).HasMaxLength(2).IsUnicode(false);
            Property(x => x.Email).HasMaxLength(256);
            Property(x => x.RescueGroupsID).HasMaxLength(6).IsUnicode(false).HasIndex();
        }
    }
}