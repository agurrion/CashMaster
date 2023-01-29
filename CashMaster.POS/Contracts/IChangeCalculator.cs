namespace CashMaster.POS.Contracts
{
    /// <summary>
    /// The IChangeCalculator interface provides a contract for calculating the minimum number of bills and coins required to make change for a given price and customer payment.
    /// </summary>
    public interface IChangeCalculator
    {
        /// <summary>
        /// Calculates the change required for the given price and customer payment.
        /// </summary>
        /// <param name="itemPrice">The price of the item(s) being purchased.</param>
        /// <param name="customerPayment">The bills and coins provided by the customer to pay for the item(s) represented as a dictionary where the key is the denomination and the value is the number of that denomination provided by the customer.</param>
        /// <returns>A <see cref="Dictionary{decimal, int}"/> object that contains the breakdown of bills and coins required where the key is the denomination and the value is the number of that denomination required for change.</returns>
        Dictionary<decimal, int> CalculateChange(decimal itemPrice, Dictionary<decimal, int> customerPayment);

        Dictionary<decimal, int> Denominations { get; }


    }
}

