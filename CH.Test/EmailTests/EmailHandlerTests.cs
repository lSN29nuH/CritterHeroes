﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Common.Commands;
using CritterHeroes.Web.Common.Email;
using CritterHeroes.Web.Common.StateManagement;
using CritterHeroes.Web.Contracts.Email;
using CritterHeroes.Web.Contracts.StateManagement;
using CritterHeroes.Web.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test.EmailTests
{
    [TestClass]
    public class EmailHandlerTests
    {
        [TestMethod]
        public async Task ResetPasswordEmailHandlerSendsResetPasswordEmail()
        {
            OrganizationContext organizationContext = new OrganizationContext()
            {
                OrganizationID = Guid.NewGuid(),
                FullName = "Full Name",
                EmailAddress = "org@org.com"
            };

            string userEmail = "email@email.com";

            ResetPasswordEmailCommand command = new ResetPasswordEmailCommand(userEmail)
            {
                Token = "code",
                Url = "http://www.password.reset.com/code"
            };

            Mock<IEmailClient> mockEmailClient = new Mock<IEmailClient>();
            mockEmailClient.Setup(x => x.SendAsync(It.IsAny<EmailMessage>())).Returns((EmailMessage message) =>
            {
                message.To.Should().HaveCount(1);
                message.To.First().Should().Be(userEmail);

                message.From.Should().Be(organizationContext.EmailAddress);
                message.Subject.Should().Be("Password Reset - " + organizationContext.FullName);
                message.HtmlBody.Should().Contain(command.Url);
                message.TextBody.Should().Contain(command.Url);

                return Task.FromResult(0);
            });

            Mock<IStateManager<OrganizationContext>> mockOrgStateManager = new Mock<IStateManager<OrganizationContext>>();
            mockOrgStateManager.Setup(x => x.GetContext()).Returns(organizationContext);

            ResetPasswordEmailCommandHandler handler = new ResetPasswordEmailCommandHandler(mockEmailClient.Object, mockOrgStateManager.Object);
            CommandResult commandResult = await handler.ExecuteAsync(command);
            commandResult.Succeeded.Should().BeTrue();

            mockEmailClient.Verify(x => x.SendAsync(It.IsAny<EmailMessage>()), Times.Once);
        }
    }
}