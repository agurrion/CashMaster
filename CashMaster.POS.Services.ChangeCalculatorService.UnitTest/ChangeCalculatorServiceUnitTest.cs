using System;
using System.Collections.Generic;
using CashMaster.POS.Configuration;
using CashMaster.POS.Contracts;
using CashMaster.POS.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace CashMaster.POS.Services.ChangeCalculatorServices.UnitTest

{
    [TestFixture]
    public class ChangeCalculatorServiceTests
    {
        private IChangeCalculatorConfiguration _config;
        private ChangeCalculatorService _service;
        Dictionary<string, string> _initialData;

        [SetUp]
        public void SetUp()
        {
            _initialData = new Dictionary<string, string>
            {
                {"Denominations", "0.01=0,0.05=0,0.10=0,0.25=0,0.50=0,1.00=0,2.00=0,5.00=0,10.00=1,20.00=0"}
            };
            var _inMemoryConfig = new ConfigurationBuilder().AddInMemoryCollection(_initialData!).Build();
            _config = new ChangeCalculatorConfigurationService(_inMemoryConfig);
           
            _service = new ChangeCalculatorService(_config);
        }

        [Test]
        public void ChangeCalculatorService_WhenUserDepositExactImport_ShouldReturnEmptyChange()
        {
            decimal itemPrice = 5.5m;
            Dictionary<decimal, int> customerPayment = new()
            {
            { 5.00m, 1 },
            { 0.50m, 1 }
        };

            var result = _service.CalculateChange(itemPrice, customerPayment);
            Assert.That(result, Is.Empty);
            
        }

        [Test]
        public void ChangeCalculatorService_WhenUserDepositLessThanCost_ShouldThrowsArgumentException()
        {
            decimal itemPrice = 5.5m;
            Dictionary<decimal, int> customerPayment = new()
            {
            { 5.00m, 1 },
            { 0.25m, 1 }
        };

            var ex = Assert.Throws<ArgumentException>(() => _service.CalculateChange(itemPrice, customerPayment));
            Assert.That(ex.Message, Is.EqualTo("The total amount provided by the customer is not enough to cover the price of the item."));
            
        }

        [Test]
        public void ChangeCalculatorService_WhenDoestHaveProperDenominations_ShouldReturnsCustomerPayment()
        {
            decimal itemPrice = 1.00m;
            Dictionary<decimal, int> customerPayment = new() { { 5.00m, 1 }, { 0.50m, 1 } };
            var result = _service.CalculateChange(itemPrice, customerPayment);
            Assert.That(result, Is.EqualTo(customerPayment));
        }

        [Test]
        public void ChangeCalculatorService_WhenDoestHaveProperDenominations_ShouldNotUpdateItsDenominations()
        {
            decimal itemPrice = 9.00m;
            Dictionary<decimal, int> customerPayment = new() { { 20.00m, 1 } };
            var result = _service.CalculateChange(itemPrice, customerPayment);
            var expectedResult = string
                                .Join(",", _initialData.Select(kvp => string.Format("{0:0.00}={1}", kvp.Key, kvp.Value)))
                                .Replace("Denominations=", "");
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(customerPayment));
                Assert.That(string.Join(",", _service.Denominations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value).Select(kvp => string.Format("{0:0.00}={1}", kvp.Key, kvp.Value))), Is.EqualTo(expectedResult));
            });
        }

    }

}
