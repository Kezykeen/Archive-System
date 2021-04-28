using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using Moq;
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
        private AccessCodeGenerator _accessCodeGenerator;
        private string _staffId;

        [SetUp]
        public void SetUp()
        {
            _accessCodeGenerator = new AccessCodeGenerator();
            _staffId = "Muna123";
        }

        [Test]
        public void NewCode_ContainsStaffId_ReturnsString()
        {
            // Act
            var result = _accessCodeGenerator.NewCode(_staffId);

            // Assert
            Assert.That(result, Does.Contain(_staffId));
            Assert.That(result, Does.EndWith(_staffId));
        }

        [Test]
        public void NewCode_ContainsGuid_ReturnsTrue()
        {
            // Act
            var result = _accessCodeGenerator.NewCode(_staffId);

            // Assert
            Assert.That(result.Length > _staffId.Length);

        }

        [Test]
        public void HashCode_WhenCalled_ReturnsHashedAccessCode()
        {
            // Arrange
            var code =_accessCodeGenerator.NewCode(_staffId);

            // Act
            var result = _accessCodeGenerator.HashCode(code);

            // Assert
            Assert.That(result, Is.TypeOf<String>());

        }

        [Test]
        public void VerifyCode_CodeMathchesHash_ReturnsTrue()
        {
            // Arrange
            var code = _accessCodeGenerator.NewCode(_staffId);
            var hash = _accessCodeGenerator.HashCode(code);

            // Act
            var result = _accessCodeGenerator.VerifyCode(code, hash);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyCode_CodeDoesNotMathchHash_ReturnsTrue()
        {
            // Arrange
            var code = _accessCodeGenerator.NewCode(_staffId);
            var hash = _accessCodeGenerator.HashCode(code);

            // Act
            var result = _accessCodeGenerator.VerifyCode("juhnik", hash);

            // Assert
            Assert.That(result, Is.False);
        }
    
        [Test]
        public void GenerateCode_ForAddMethod_ReturnsTrue()
        {
            // Arrange
            var code = _accessCodeGenerator.NewCode(_staffId);
            var hash = _accessCodeGenerator.HashCode(code);

            // Act
            var result = _accessCodeGenerator.VerifyCode("juhnik", hash);

            // Assert
            Assert.That(result, Is.False);
        }
        
       
    }
}
