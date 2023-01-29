


using CashMaster.POS.Configuration;
using CashMaster.POS.Contracts;
using CashMaster.POS.Exceptions;
using CashMaster.POS.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CashMaster.POS.ChangeCalculatorConfigurationServices.Tests
{
    [TestFixture]
    public class ChangeCalculatorConfigurationServiceTests
    {
        private IConfiguration _config;
        private ChangeCalculatorConfigurationService _service;

        [SetUp]
        public void Setup()
        {
            Dictionary<string, string> initialData = new Dictionary<string, string>
                {
                    {"Denominations", "0.01=0,0.05=0,0.10=0,0.25=0,0.50=0,1.00=0,2.00=0,5.00=0,10.00=0"}
                };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData!)
                .Build();
            _service = new ChangeCalculatorConfigurationService(_config);
        }

        [Test]
        public void GetDenominations_Should_ReturnsExpectedDenominationsInventory()
        {
            // Act
            var result = _service.GetDenominations();

            // Assert
            
            Assert.That(result.Count, Is.EqualTo(9));
            Assert.That(result[0.01m], Is.EqualTo(0));
            Assert.That(result[0.05m], Is.EqualTo(0));
            Assert.That(result[0.10m], Is.EqualTo(0));
            Assert.That(result[0.25m], Is.EqualTo(0));
            Assert.That(result[0.50m], Is.EqualTo(0));
            Assert.That(result[1.00m], Is.EqualTo(0));
            Assert.That(result[2.00m], Is.EqualTo(0));
            Assert.That(result[5.00m], Is.EqualTo(0));
            Assert.That(result[10.00m], Is.EqualTo(0));
        }

        [Test]
        public void ConfigurationService_WhenEmptyConfig_Should_ThrowsMissingConfigurationException()
        {
            // Arrange
            var emptyConfig = new ConfigurationBuilder().Build();
            var service = new ChangeCalculatorConfigurationService(emptyConfig);

            // Act & Assert
            var ex = Assert.Throws<MissingConfigurationException>(() => service.GetDenominations());
            Assert.That(ex, !Is.Null);
        }

        [Test]
        public void ConfigurationService_WhenDenominationsConfigVariableIsInvalid_ShouldThrowInvalidConfigurationVariableFormatException()
        {
            //Moq is designed to mock only virtual or abstract members but GetValue is an extension method
            // Arrange
            Dictionary<string, string> initialData = new Dictionary<string, string>
                {
                    {"Denominations", "0.01=a"}
                };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData!)
                .Build();

            _service = new ChangeCalculatorConfigurationService(_config);
           
            // Assert
            Assert.Throws<InvalidConfigurationVariableFormatException>(() => _service.GetDenominations());
        }
    }
}
