﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CritterHeroes.Web.Features.Home;

namespace CritterHeroes.Web.Features.Shared.ActionExtensions
{
    public static class HomeActionExtensions
    {
        public static void RenderHomeMenuAction(this HtmlHelper htmlHelper)
        {
            htmlHelper.RenderAction(nameof(HomeController.Menu), HomeController.Route, AreaName.NoAreaRouteValue);
        }

        public static void RenderHomeHeaderAction(this HtmlHelper htmlHelper)
        {
            htmlHelper.RenderAction(nameof(HomeController.Header), HomeController.Route, AreaName.NoAreaRouteValue);
        }
    }
}
