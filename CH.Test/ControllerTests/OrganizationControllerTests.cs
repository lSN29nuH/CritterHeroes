﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CH.Test.ControllerTests.TestHelpers;
using CritterHeroes.Web.Features.Admin.Organizations;
using CritterHeroes.Web.Features.Admin.Organizations.Models;
using CritterHeroes.Web.Features.Admin.Organizations.Queries;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CH.Test.ControllerTests
{
    [TestClass]
    public class OrganizationControllerTests
    {
        [TestMethod]
        public void EditProfileGetReturnsViewWithModel()
        {
            EditProfileModel model = new EditProfileModel();

            EditProfileQuery query = new EditProfileQuery();

            ControllerTester.UsingController<OrganizationController>()
                .SetupQueryDispatcherAsync(model, query)
                .WithCallTo(x => x.EditProfile(query))
                .VerifyQueryDispatcher()
                .ShouldReturnViewResult()
                .HavingModel(model);
        }

        [TestMethod]
        public void EditProfilePostReturnsViewWithModelIfModelStateIsInvalid()
        {
            EditProfileModel model = new EditProfileModel();

            ControllerTester.UsingController<OrganizationController>()
                .WithInvalidModelState("error")
                .WithCallTo(x => x.EditProfile(model))
                .ShouldReturnViewResult()
                .HavingModel(model);
        }

        [TestMethod]
        public void EditProfilePostReturnsViewWithModelIfModelStateIsValidAndCommandHandlerFailsAndNotAjaxRequest()
        {
            EditProfileModel model = new EditProfileModel();
            string error = "error";

            ControllerTester.UsingController<OrganizationController>()
                .SetupCommandDispatcherForFailureAsync(model, error)
                .ShouldHaveValidModelState()
                .ShouldNotBeAjaxRequest()
                .WithCallTo(x => x.EditProfile(model))
                .VerifyCommandDispatcher()
                .ShouldHaveSingleModelError(error)
                .ShouldReturnViewResult()
                .HavingModel(model);
        }

        [TestMethod]
        public void EditProfilePostReturnsStatusCodeIfModelStateIsValidAndCommandHandlerFailsAndIsAjaxRequest()
        {
            EditProfileModel model = new EditProfileModel();
            string error = "error";

            ControllerTester.UsingController<OrganizationController>()
                .SetupCommandDispatcherForFailureAsync(model, error)
                .ShouldHaveValidModelState()
                .ShouldBeAjaxRequest()
                .WithCallTo(x => x.EditProfile(model))
                .VerifyCommandDispatcher()
                .ShouldReturnStatusCode(HttpStatusCode.InternalServerError, error);
        }

        [TestMethod]
        public void EditProfilePostRedirectsIfModelStateIsValidAndCommandHandlerSucceeds()
        {
            EditProfileModel model = new EditProfileModel();

            ControllerTester.UsingController<OrganizationController>()
                .SetupCommandDispatcherForSuccessAsync(model)
                .ShouldHaveValidModelState()
                .WithCallTo(x => x.EditProfile(model))
                .ShouldRedirectToPrevious();
        }
    }
}
