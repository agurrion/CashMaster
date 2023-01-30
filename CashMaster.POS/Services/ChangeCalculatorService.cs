using CashMaster.POS.Contracts;
using Microsoft.Extensions.Configuration;

namespace CashMaster.POS.Services
{
    /// <summary>
    /// The ChangeCalculatorService class calculates the minimum number of bills and coins required to make change for a given price and customer payment.
    /// </summary>
    public class ChangeCalculatorService : IChangeCalculator
    {
        private Dictionary<decimal, int> _denominations;
        private readonly IChangeCalculatorConfiguration _config;

        public Dictionary<decimal, int> Denominations => _denominations;





        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeCalculator"/> class.
        /// </summary>
        /// <param name="config">The configuration object containing the 
        public ChangeCalculatorService(IChangeCalculatorConfiguration config)
        {
            _config = config;
            _denominations = _config.GetDenominations();
        }
        


        private void UpdateInventory(Dictionary<decimal, int> customerPayment)
        {
            foreach (var item in customerPayment)
            {
                if (_denominations.ContainsKey(item.Key))
                    _denominations[item.Key] += item.Value;
                else
                    _denominations.Add(item.Key, item.Value);
            }
        }

        private void RollbackInventory(Dictionary<decimal, int> customerPayment)
        {
            foreach (var item in customerPayment)
            {
                if (_denominations.ContainsKey(item.Key))
                    _denominations[item.Key] -= item.Value;
            }
        }

        private void RollbackChange(Dictionary<decimal, int> change)
        {
            foreach (var item in change)
            {
                if (_denominations.ContainsKey(item.Key))
                    _denominations[item.Key] += item.Value;
            }
        }

        /// <summary>
        /// Calculates the change required for the given price and customer payment.
        /// </summary>
        /// <param name="itemPrice">The price of the item(s) being purchased.</param>
        /// <param name="customerPayment">The bills and coins provided by the customer to pay for the item(s) represented as a dictionary where the key is the denomination and the value is the number of that denomination provided by the customer.</param>
        /// <returns>A <see cref="Dictionary{decimal, int}"/> object that contains the breakdown of bills and coins required where the key is the denomination and the value is the number of that denomination required for change.</returns>
        /// /// <exception cref="ArgumentException">When the total amount provided by the customer is less than the price of the item.</exception>
        public Dictionary<decimal, int> CalculateChange(decimal itemPrice, Dictionary<decimal, int> customerPayment)
        {

            // Verify that the total of what the customer provided is greater than or equal to the price of the item they're purchasing

            decimal customerTotal = customerPayment.Sum(x => x.Key * x.Value);
            if (customerTotal < itemPrice)
            {
                throw new ArgumentException("The total amount provided by the customer is not enough to cover the price of the item.");
            }

            // Calculate the total change due
            decimal changeDue = customerTotal - itemPrice;
            Dictionary<decimal, int> change = new Dictionary<decimal, int>();

            // first thing is to update the inventory _denominations dictionary
            UpdateInventory(customerPayment);

            // Iterate through the available denominations in descending order
            foreach (decimal theDenomination in _denominations.Keys.OrderByDescending(x => x))
            {
                if (theDenomination <= changeDue && _denominations[theDenomination] > 0)
                {
                    // Calculate the number of that denomination required to make change
                    int numDenom = (int)(changeDue / theDenomination);
                    if (_denominations[theDenomination] >= numDenom)
                    {
                        change.Add(theDenomination, numDenom);
                        changeDue -= (theDenomination * numDenom);
                        _denominations[theDenomination] -= numDenom;

                    }
                }
            }
            //update the denominations' inventory if it was not able to complete the change due
            if (changeDue > 0)
            {
                //rollback what i've added
                RollbackInventory(customerPayment);
                RollbackChange(change);
                return customerPayment;
            }
                
            else
                return change;
        }

       
    }





}





