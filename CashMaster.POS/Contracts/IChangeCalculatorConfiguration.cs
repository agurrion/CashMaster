using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMaster.POS.Contracts
{
    /// <summary>
    /// The IChangeCalculatorConfiguration interface defines a single method, 
    /// GetDenominations(), that is used to retrieve the available denominations that can be used to make change. 
    /// The method returns a Dictionary<decimal, int> object where the key represents the denomination and the value represents the number of that denomination available. 
    /// This interface is typically implemented by a configuration class that reads the denominations from a configuration source such as an environment variable
    /// or a configuration file. 
    /// The implementation of the interface is responsible for parsing the configuration data and returning it in the expected format, 
    /// which is a dictionary of decimal keys and integer values.
    /// </summary>
    public interface IChangeCalculatorConfiguration
    {
        Dictionary<decimal, int> GetDenominations();
    }
}
