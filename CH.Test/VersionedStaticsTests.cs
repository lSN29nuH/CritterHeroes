﻿using System;
using System.Collections.Generic;
using System.Linq;
using CH.Test.Mocks;
using CritterHeroes.Web.Common.VersionedStatics;
using CritterHeroes.Web.Contracts;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test
{
    [TestClass]
    public class VersionedStaticsTests
    {
        [TestMethod]
        public void ReturnsUrlForDebugFileIfDebuging()
        {
            MockFileSystem mockFileSystem = new MockFileSystem();
            mockFileSystem.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns(LibManifest);
            mockFileSystem.Setup(x => x.MapServerPath(It.IsAny<string>())).Returns((string path) => path);

            Mock<IHttpContext> mockHttpContext = new Mock<IHttpContext>();
            mockHttpContext.Setup(x => x.IsDebuggingEnabled).Returns(true);
            mockHttpContext.Setup(x => x.ConvertToAbsoluteUrl(It.IsAny<string>())).Returns((string virtualPath) => virtualPath);

            VersionedStatics.Configure(mockFileSystem.Object, mockHttpContext.Object);
            VersionedStatics.IsDebug = true;
            VersionedStatics.UrlFor("file1.js").Should().Be("~/dist/js/file1-12345.js");
        }

        [TestMethod]
        public void ReturnsUrlForProductionFileIfNotDebuging()
        {
            MockFileSystem mockFileSystem = new MockFileSystem();
            mockFileSystem.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns(LibManifest);
            mockFileSystem.Setup(x => x.MapServerPath(It.IsAny<string>())).Returns((string path) => path);

            Mock<IHttpContext> mockHttpContext = new Mock<IHttpContext>();
            mockHttpContext.Setup(x => x.IsDebuggingEnabled).Returns(true);
            mockHttpContext.Setup(x => x.ConvertToAbsoluteUrl(It.IsAny<string>())).Returns((string virtualPath) => virtualPath);

            VersionedStatics.Configure(mockFileSystem.Object, mockHttpContext.Object);
            VersionedStatics.IsDebug = false;
            VersionedStatics.UrlFor("file1.js").Should().Be("~/dist/js/file1-12345.min.js");
        }

        public const string LibManifest = "{\"file1.js\": \"file1-12345.js\"}";
    }
}