﻿using System;
using CritterHeroes.Web.Common;

namespace CritterHeroes.Web.DataProviders.RescueGroups
{
    public class RescueGroupsException : BaseException
    {
        public RescueGroupsException(string message)
            : base(message)
        {
        }

        public RescueGroupsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RescueGroupsException(string format, params object[] args)
            : base(format, args)
        {
        }
    }
}