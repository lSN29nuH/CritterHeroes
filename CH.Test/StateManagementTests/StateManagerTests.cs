﻿using System;
using System.Collections.Generic;
using System.Linq;
using CritterHeroes.Web.Domain.Contracts.StateManagement;
using CritterHeroes.Web.Shared.StateManagement;
using FluentAssertions;
using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TOTD.Utility.StringHelpers;

namespace CH.Test.StateManagementTests
{
    [TestClass]
    public class StateManagerTests : BaseContextTest
    {
        [TestMethod]
        public void ThrowsExceptionForNullOwinContext()
        {
            Action action = () => new FakeStateManager(null, new StateSerializer(), "test");
            action
                .ShouldThrow<ArgumentException>()
                .And
                .ParamName.Should().Be("owinContext");
        }

        [TestMethod]
        public void ThrowsExceptionForNullKey()
        {
            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            Action action = () => new FakeStateManager(mockOwinContext.Object, new StateSerializer(), null);
            action
                .ShouldThrow<ArgumentException>()
                .And
                .ParamName.Should().Be("key");
        }

        [TestMethod]
        public void ThrowsExceptionForEmptyKey()
        {
            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            Action action = () => new FakeStateManager(mockOwinContext.Object, new StateSerializer(), string.Empty);
            action
                .ShouldThrow<ArgumentException>()
                .And
                .ParamName.Should().Be("key");
        }

        [TestMethod]
        public void CombinesGivenKeyWithBaseKey()
        {
            Dictionary<string, string[]> cookies = new Dictionary<string, string[]>();

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Response.Cookies).Returns(new ResponseCookieCollection(new HeaderDictionary(cookies)));

            FakeStateManager stateManager = new FakeStateManager(mockOwinContext.Object, new StateSerializer(), "key");
            stateManager.SaveContext("test");

            cookies.Should().HaveCount(1);
            cookies.First().Value[0].Should().Contain("CritterHeroes_key");
        }

        [TestMethod]
        public void ReturnsDefaultValueWhenCookieIsInvalid()
        {
            Dictionary<string, string> cookies = new Dictionary<string, string>();
            cookies["CritterHeroes_key"] = "";

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Request.Cookies).Returns(new RequestCookieCollection(cookies));

            FakeStateManager stateManager = new FakeStateManager(mockOwinContext.Object, new StateSerializer(), "key");
            stateManager.GetContext().Should().BeNull();
        }

        [TestMethod]
        public void ReturnsCachedValueWhenAskedForContextSecondTime()
        {
            string value = "test";

            Dictionary<string, string> cookies = new Dictionary<string, string>();
            StateSerializer serializer = new StateSerializer();
            cookies["CritterHeroes_key"] = serializer.Serialize(value);

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Request.Cookies).Returns(new RequestCookieCollection(cookies));

            FakeStateManager stateManager = new FakeStateManager(mockOwinContext.Object, new StateSerializer(), "key");

            stateManager.GetContext().Should().Be(value);
            stateManager.GetContext().Should().Be(value);

            mockOwinContext.Verify(x => x.Request.Cookies, Times.Once);
        }
    }

    public class FakeStateManager : StateManager<string>
    {
        public FakeStateManager(IOwinContext httpContext, IStateSerializer serializer, string key)
            : base(httpContext, serializer, key)
        {
        }

        protected override bool IsValid(string context)
        {
            if (context.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }
    }
}
