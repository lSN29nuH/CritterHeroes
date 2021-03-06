﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CritterHeroes.Web.Domain.Contracts.Configuration;

namespace CritterHeroes.Web.DataProviders.RescueGroups.Configuration
{
    public class RescueGroupsConfiguration : IRescueGroupsConfiguration
    {
        private RescueGroupsConfigurationSection configSection;

        public RescueGroupsConfiguration()
        {
            configSection = ConfigurationManager.GetSection("rescueGroups") as RescueGroupsConfigurationSection;
            if (configSection == null)
            {
                throw new RescueGroupsException("RescueGroups configuration does not exist");
            }
        }

        public string Url
        {
            get
            {
                return configSection.Url;
            }
        }

        public string APIKey
        {
            get
            {
                return configSection.APIKey;
            }
        }

        public string Username
        {
            get
            {
                return configSection.Username;
            }
        }

        public string Password
        {
            get
            {
                return configSection.Password;
            }
        }

        public string AccountNumber
        {
            get
            {
                return configSection.AccountNumber;
            }
        }
    }
}
