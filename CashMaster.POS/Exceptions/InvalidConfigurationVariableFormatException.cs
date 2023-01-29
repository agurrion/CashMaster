using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMaster.POS.Exceptions
{
    /// <summary>
    /// Exception thrown when the format of the configuration variable 'Denominations' is invalid
    public class InvalidConfigurationVariableFormatException : Exception
    {
        public InvalidConfigurationVariableFormatException() : base("Invalid format for Denominations configuration use this CSV format denomination=quantity e.g. 0.01=1,0.05=2 ")
        { }

        public InvalidConfigurationVariableFormatException(FormatException innerException) : base("Invalid format for Denominations configuration", innerException)
        { }
    }
}
