using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMaster.POS.Exceptions
{
    /// <summary>
    /// Exception thrown when the configuration variable 'Denominations' is missing
    /// </summary>
    public class MissingConfigurationException : Exception
    {
        public MissingConfigurationException() : base("Missing Denominations environment variable")
        { }
    }
}
