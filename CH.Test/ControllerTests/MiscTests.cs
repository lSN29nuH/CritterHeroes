﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CH.Website.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOTD.Mvc;

namespace CH.Test.ControllerTests
{
    [TestClass]
    public class MiscTests : BaseTest
    {
        [TestMethod]
        public void AllHttpPostControllerActionsShouldHaveValidateAntiForgeryTokenAttribute()
        {
            AssertMethodsListIsNullOrEmpty(UnitTestHelper.GetControllerActionsMissingValidateAntiForgeryTokenAttribute<BaseController>(), "all post actions need anti-forgery token");
        }
    }
}