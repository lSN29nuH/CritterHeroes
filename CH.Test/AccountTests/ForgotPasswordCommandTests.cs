﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CH.Test.Mocks;
using CritterHeroes.Web.Areas.Account;
using CritterHeroes.Web.Areas.Account.CommandHandlers;
using CritterHeroes.Web.Areas.Account.Models;
using CritterHeroes.Web.Areas.Common.ActionExtensions;
using CritterHeroes.Web.Common.Commands;
using CritterHeroes.Web.Common.StateManagement;
using CritterHeroes.Web.Contracts;
using CritterHeroes.Web.Contracts.Email;
using CritterHeroes.Web.Contracts.Identity;
using CritterHeroes.Web.Contracts.Logging;
using CritterHeroes.Web.Contracts.StateManagement;
using CritterHeroes.Web.Contracts.Storage;
using CritterHeroes.Web.Data.Models.Identity;
using CritterHeroes.Web.Models.Logging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test.AccountTests
{
    [TestClass]
    public class ForgotPasswordCommandTests
    {
        [TestMethod]
        public async Task ReturnsSuccessAndSendsAttemptEmailIfUserNotFound()
        {
            string email = "email@email.com";
            string urlLogo = "logo";

            OrganizationContext organizationContext = new OrganizationContext()
            {
                FullName = "FullName"
            };

            Mock<IStateManager<OrganizationContext>> mockStateManager = new Mock<IStateManager<OrganizationContext>>();
            mockStateManager.Setup(x => x.GetContext()).Returns(organizationContext);

            ForgotPasswordModel command = new ForgotPasswordModel()
            {
                ResetPasswordEmail = email
            };

            Mock<IUserLogger> mockUserLogger = new Mock<IUserLogger>();
            Mock<IAppUserManager> mockUserManager = new Mock<IAppUserManager>();
            Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
            Mock<IUrlGenerator> mockUrlGenerator = new Mock<IUrlGenerator>();

            Mock<IOrganizationLogoService> mockLogoService = new Mock<IOrganizationLogoService>();
            mockLogoService.Setup(x => x.GetLogoUrl()).Returns(urlLogo);

            ForgotPasswordCommandHandler handler = new ForgotPasswordCommandHandler(mockUserLogger.Object, mockUserManager.Object, mockEmailService.Object, mockUrlGenerator.Object, mockStateManager.Object, mockLogoService.Object);
            CommandResult result = await handler.ExecuteAsync(command);
            result.Succeeded.Should().BeTrue();

            mockUserLogger.Verify(x => x.LogActionAsync(UserActions.ForgotPasswordFailure, command.ResetPasswordEmail), Times.Once);
            mockUserManager.Verify(x => x.FindByEmailAsync(email), Times.Once);
            mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<ResetPasswordEmailCommand>()), Times.Never);
            mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<ResetPasswordAttemptEmailCommand>()), Times.Once);
        }

        [TestMethod]
        public async Task ResetsPasswordForValidUser()
        {
            ForgotPasswordModel command = new ForgotPasswordModel()
            {
                ResetPasswordEmail = "email@email.com",
            };

            AppUser user = new AppUser("unit.test")
            {
                Email = command.ResetPasswordEmail
            };

            string code = "code";
            string urlLogo = "logo";

            OrganizationContext organizationContext = new OrganizationContext()
            {
                FullName = "FullName"
            };

            Mock<IStateManager<OrganizationContext>> mockStateManager = new Mock<IStateManager<OrganizationContext>>();
            mockStateManager.Setup(x => x.GetContext()).Returns(organizationContext);

            Mock<IUserLogger> mockUserLogger = new Mock<IUserLogger>();

            Mock<IAppUserManager> mockUserManager = new Mock<IAppUserManager>();
            mockUserManager.Setup(x => x.FindByEmailAsync(command.ResetPasswordEmail)).Returns(Task.FromResult(user));
            mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user.Id)).Returns(Task.FromResult(code));

            MockUrlGenerator mockUrlGenerator = new MockUrlGenerator();

            Mock<IOrganizationLogoService> mockLogoService = new Mock<IOrganizationLogoService>();
            mockLogoService.Setup(x => x.GetLogoUrl()).Returns(urlLogo);

            Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
            mockEmailService.Setup(x => x.SendEmailAsync(It.IsAny<ResetPasswordEmailCommand>())).Returns((ResetPasswordEmailCommand emailCommand) =>
            {
                emailCommand.Token.Should().Be(code);

                emailCommand.Url.Should().Be(mockUrlGenerator.UrlHelper.AbsoluteAction(nameof(AccountController.ResetPassword), AccountActionExtensions.ControllerRouteName, new
                {
                    code = code
                }));

                return Task.FromResult(CommandResult.Success());
            });

            ForgotPasswordCommandHandler handler = new ForgotPasswordCommandHandler(mockUserLogger.Object, mockUserManager.Object, mockEmailService.Object, mockUrlGenerator.Object, mockStateManager.Object, mockLogoService.Object);
            CommandResult result = await handler.ExecuteAsync(command);
            result.Succeeded.Should().BeTrue();

            mockUserLogger.Verify(x => x.LogActionAsync(UserActions.ForgotPasswordSuccess, user.Email), Times.Once);
            mockUserManager.Verify(x => x.FindByEmailAsync(command.ResetPasswordEmail), Times.Once);
            mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(user.Id), Times.Once);
            mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<ResetPasswordEmailCommand>()), Times.Once);
        }
    }
}
