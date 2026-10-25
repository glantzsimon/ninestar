using K9.WebApplication.Helpers;
using System;
using System.IO;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class DeploymentTargetHelperTests
    {
        [Theory]
        [InlineData("prod", "prod")]
        [InlineData("int", "int")]
        [InlineData("INT", "int")]
        [InlineData(null, "prod")]
        public void RecognisesOnlyKnownTargets(string input, string expected)
        {
            Assert.Equal(expected, DeploymentTargetHelper.Environment(input));
        }

        [Theory]
        [InlineData("")]
        [InlineData("integration")]
        [InlineData("prod;int")]
        public void RejectsUnknownTargets(string input)
        {
            Assert.Throws<ArgumentException>(() => DeploymentTargetHelper.Environment(input));
        }

        [Fact]
        public void IntegrationRequiresItsOwnConfiguredVault()
        {
            var root = Path.Combine(Path.GetTempPath(), "ninestar-vault");
            Assert.Equal(Path.Combine(root, "int"), DeploymentTargetHelper.Vault("int", root, Path.Combine(root, "int")));
            Assert.Throws<ArgumentException>(() => DeploymentTargetHelper.Vault("int", root, root));
            Assert.Throws<ArgumentException>(() => DeploymentTargetHelper.Vault("int", root, null));
        }

        [Theory]
        [InlineData("../web.config", "views")]
        [InlineData("folder/../../web.config", "views")]
        [InlineData("folder\\..\\web.config", "views")]
        [InlineData("C:\\web.config", "root")]
        [InlineData("/web.config", "root")]
        [InlineData("file.dll", "images")]
        [InlineData("file.ps1", "root")]
        [InlineData("file.css", "unknown")]
        public void RejectsEscapesAndMismatchedCategories(string name, string category)
        {
            Assert.Throws<ArgumentException>(() => DeploymentTargetHelper.UploadPath(Path.GetTempPath(), name, category));
        }

        [Fact]
        public void PreservesNestedConfigurationAndLegacyViewConfigMapping()
        {
            var root = Path.Combine(Path.GetTempPath(), "ninestar-vault");
            Assert.Equal(Path.Combine(root, "config", "nested", "file.json"), DeploymentTargetHelper.UploadPath(root, "nested/file.json", "config"));
            Assert.Equal(Path.Combine(root, "views", "web.config"), DeploymentTargetHelper.UploadPath(root, "views.config", null));
        }
    }
}
