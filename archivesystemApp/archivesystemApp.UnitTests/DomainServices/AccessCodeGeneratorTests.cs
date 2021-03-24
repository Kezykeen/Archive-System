using archivesystemDomain.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemApp.UnitTests.DomainServices
{
    [TestFixture]
    class AccessCodeGeneratorTests
    {
        private string _staffId;

        [SetUp]
        public void SetUp()
        {
            _staffId = "Muna123";
        }

        [Test]
        public void NewCode_ContainsStaffId_ReturnsString()
        {
            // Act
            var result = AccessCodeGenerator.NewCode(_staffId);

            // Assert
            Assert.That(result, Does.Contain(_staffId));
            Assert.That(result, Does.EndWith(_staffId));
        }

        [Test]
        public void NewCode_ContainsGuid_ReturnsTrue()
        {
            // Act
            var result = AccessCodeGenerator.NewCode(_staffId);

            // Assert
            Assert.That(result.Length > _staffId.Length);

        }

        [Test]
        public void HashCode_WhenCalled_ReturnsHashedAccessCode()
        {
            // Arrange
            var code = AccessCodeGenerator.NewCode(_staffId);

            // Act
            var result = AccessCodeGenerator.HashCode(code);

            // Assert
            Assert.That(result, Is.TypeOf<String>());

        }

        [Test]
        public void VerifyCode_CodeMathchesHash_ReturnsTrue()
        {
            // Arrange
            var code = AccessCodeGenerator.NewCode(_staffId);
            var hash = AccessCodeGenerator.HashCode(code);

            // Act
            var result = AccessCodeGenerator.VerifyCode(code, hash);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyCode_CodeDoesNotMathchHash_ReturnsTrue()
        {
            // Arrange
            var code = AccessCodeGenerator.NewCode(_staffId);
            var hash = AccessCodeGenerator.HashCode(code);

            // Act
            var result = AccessCodeGenerator.VerifyCode("juhnik", hash);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
