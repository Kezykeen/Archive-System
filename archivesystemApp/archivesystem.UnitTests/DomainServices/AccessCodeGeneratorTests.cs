using archivesystemDomain.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1.DomainServices
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
        public void HashCode_WhenCalled_ReturnsString()
        {
            // Act
            var result = AccessCodeGenerator.HashCode(_staffId);

            // Assert
           // Assert.That(result, Is.TypeOf(String));

        }
    }
}
