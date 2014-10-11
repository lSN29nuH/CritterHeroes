﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AR.Website.Utility.FluentHtml.Elements;
using AR.Website.Utility.FluentHtml.Html;

namespace AR.Website.Utility.FluentHtml.Bootstrap
{
    public class BootstrapNav : BaseListElement<BootstrapNav>
    {
        public BootstrapNav(ViewContext viewContext)
            : base(HtmlTag.ListUnordered, viewContext)
        {
        }

        public BootstrapNav AsNavBar()
        {
            return Class("nav").Class("navbar-nav");

        }

        public BootstrapNav AsDropdown()
        {
            return Class("dropdown-menu");
        }
    }
}