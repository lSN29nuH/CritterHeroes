﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CH.Domain.Contracts;
using CH.Domain.Contracts.Identity;
using CH.Domain.Identity;
using CH.Domain.Services.Queries;
using CH.Domain.StateManagement;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test.QueryTests
{
    [TestClass]
    public class UserQueryHandlerTests : BaseTest
    {
        [TestMethod]
        public async Task ReturnsUserContextFromCookieIfItExists()
        {
            UserContext context = new UserContext()
            {
                DisplayName = "First Last"
            };

            Mock<IStateManager<UserContext>> mockStateManager = new Mock<IStateManager<UserContext>>();
            mockStateManager.Setup(x => x.GetContext()).Returns(context);

            Mock<IApplicationUserStore> mockUserStore = new Mock<IApplicationUserStore>();

            UserContextQueryHandler handler = new UserContextQueryHandler(mockUserStore.Object, mockStateManager.Object);
            UserContext resultContext = await handler.Retrieve(new UserQuery()
            {
                Username = "unit.test"
            });

            resultContext.Should().Equals(context);

            mockStateManager.Verify(x => x.GetContext(), Times.Once);
        }

        [TestMethod]
        public async Task CreatesAndSavesUserContextIfCookieDoesNotExist()
        {
            IdentityUser user = new IdentityUser("unit.test")
            {
                FirstName = "First",
                LastName = "Last"
            };

            Mock<IStateManager<UserContext>> mockStateManager = new Mock<IStateManager<UserContext>>();
            mockStateManager.Setup(x => x.GetContext()).Returns((UserContext)null);
            mockStateManager.Setup(x => x.SaveContext(It.IsAny<UserContext>()));

            Mock<IApplicationUserStore> mockUserStore = new Mock<IApplicationUserStore>();
            mockUserStore.Setup(x => x.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user));

            UserContextQueryHandler queryHandler = new UserContextQueryHandler(mockUserStore.Object, mockStateManager.Object);
            UserContext resultContext = await queryHandler.Retrieve(new UserQuery()
            {
                Username = user.UserName
            });

            resultContext.DisplayName.Should().Be("First Last");

            mockStateManager.Verify(x => x.GetContext(), Times.Once);
            mockStateManager.Verify(x => x.SaveContext(It.IsAny<UserContext>()), Times.Once);
            mockUserStore.Verify(x => x.FindByNameAsync(user.UserName), Times.Once);
        }
    }
}