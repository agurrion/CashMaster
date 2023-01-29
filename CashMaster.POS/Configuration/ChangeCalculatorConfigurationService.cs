using CashMaster.POS.Contracts;
using CashMaster.POS.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMaster.POS.Configuration
{
    /// <summary>
    /// Service class that reads the denominations configuration
    /// Expected format is a list of all valid denomination=quantity e.g. 0.01=100,0.05=0...
    /// Include the denomination key even you have a zero initial quantity of this .
    /// </summary>
    public class ChangeCalculatorConfigurationService : IChangeCalculatorConfiguration
    {
        private readonly IConfiguration _config;
        const string _denominationsConfigName = "Denominations";

        /// <summary>
        /// Constructor that accepts an instance of <see cref="IConfiguration"/>
        /// </summary>
        /// <param name="config"></param>
        public ChangeCalculatorConfigurationService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Reads the denomination from a configuration provider
        /// </summary>
        /// <returns>Dictionary<decimal, int></returns>
        public Dictionary<decimal, int> GetDenominations()
        {
            // make it more flexible and testable trusting in abstract IConfiguration
           //var denominationString = Environment.GetEnvironmentVariable("ChangeInventory");

            var denominationString = _config.GetValue<string>($"{_denominationsConfigName}");
            if (string.IsNullOrEmpty(denominationString))
                throw new MissingConfigurationException();
            try
            {
                return  denominationString!
                        .Split(',')
                        .Select(x => x.Split('='))
                        .ToDictionary(x => decimal.Parse(x[0]), x => int.Parse(x[1]));
            }
            catch (FormatException e)
            {
                throw new InvalidConfigurationVariableFormatException(e);
            }
        }
    }

}
