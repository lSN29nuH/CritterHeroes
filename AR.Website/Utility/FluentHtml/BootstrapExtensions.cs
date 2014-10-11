﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AR.Website.Utility.FluentHtml.Bootstrap;

namespace AR.Website.Utility.FluentHtml
{
    public static class BootstrapExtensions
    {
        public static BootstrapNav BootstrapNav(this ElementFactory elementFactory)
        {
            return elementFactory.CreateElement<BootstrapNav>();
        }

        public static BootstrapNavItem BootstrapNavItem(this ElementFactory elementFactory)
        {
            return elementFactory.CreateElement<BootstrapNavItem>();
        }
    }
}