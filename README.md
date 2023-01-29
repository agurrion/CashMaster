# ChangeCalculatorService

## Requirements
----
Today’s young cashiers are increasingly unable to return the correct amount of change.  As a result, we would like our POS system to calculate the correct change and display the optimum (i.e. minimum) number of bills and coins to return to the customer. 
  
Your task is to write a routine that takes two inputs:
-	Price of the item(s) being purchased
-	All bills and coins provided by the customer to pay for the item(s)

The routine should return the smallest number of bills and coins equal to the change due.

Note: the customer may not submit an optimal number of bills and coins. For example, if the price is $5.25, they might provide one $5 bill and one $1 bill, or, they could provide one $5 bill and ten dimes ($0.10).  The only expectation is that the total of what they provide is greater that or equal to the price of the item they’re purchasing.  Your function should verify this assumption.

Since other engineers will be using your new function, recommend an appropriate data structure for the bills and coins. This structure should be used for the input parameter and for the returned value.  Additionally, this system will be sold around the world.  Each country will have its own denomination of bills and coins. For example, here are denomination lists for two countries where our POS might be sold:

•	US: 0.01, 0.05, 0.10, 0.25, 0.50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00
•	Mexico: 0.05, 0.10, 0.20, 0.50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00

You are not required to physically distinguish whether the values are bills or coins, only their numeric values.

When a POS terminal is sold and installed in a given country, its currency setting will be set only once as a global configuration. 
## Solution
----
The `ChangeCalculatorService` class calculates the minimum number of bills and coins required to make change for a given price and customer payment.

## Usage
You need to specify a string that contains the INITIAL target country denomination inventory of bills and coins.
<br/>The solution supports you to specify the configuration from different sources, for example in memory, environment variables etc. Let's consider the case that you want to specify the configuration in an environment variable
<br/> For an Hypothetical country set an initial available inventory of denominations with its initial quantity.
e.g. For windows OS :  
<br/> setx Denominations "0.01=0,0.05=0,0.10=0,0.25=0,0.50=0,1.00=0,2.00=0,5.00=0,10.00=0" /M 
<br/> or export Denominations 0.01=0,0.05=0,0.10=0,0.25=0,0.50=0,1.00=0,2.00=0,5.00=0,10.00=0 /M in Linux and MacOS.
<br/> For example the expression 0.01=0 tells the system that initially you have the quantity of 0 times 0.01, in this way if you wanted to express that you have an initial inventory with 5 denominations of 0.01 you should substitute 0.01=5

1.Define the initial inventory **Denominations** and from which configuration scope it will be read: Configuration reading supports different providers eg environment variable, in-memory, json config file etc. (In Program Console I have commented some examples to define the configuration)
e.g. 
```csharp
Dictionary<string, string> initialData = new Dictionary<string, string>
                {
                    {"Denominations", "0.01=0,0.05=0,0.10=0,0.25=0,0.50=0,1.00=0,2.00=0,5.00=0,10.00=0"}
                };
var _inMemoryConfig = new ConfigurationBuilder().AddInMemoryCollection(initialData!).Build();
IChangeCalculatorConfiguration _config = new ChangeCalculatorConfigurationService(_inMemoryConfig);               
```
### IMPORTANT: 

We are delegating the responsibility of the configuration read to the Microsoft.Extensions.Configuration.IConfiguration implementation, so please notice that these providers usually only support reads, for writes additional work is required, the scope of this DEMO is to focus in the optimal change return algorithm.
A more realistic scenario should contemplate preserving the CASH machine ' Denominations state  , I have put an initial idea in Transaction Coordinator.txt

2.Create an instance of the ChangeCalculatorService class and pass in an implementation of the IChangeCalculatorConfiguration interface.
e.g.
```csharp
IChangeCalculator _changeService = new ChangeCalculatorService(_config);
```
3.The CalculateChange method returns a dictionary representing the change required, where the key is the denomination and the value is the number of that denomination required for change.
e.g.
```csharp
decimal itemPrice = 5.5m;
Dictionary<decimal, int> customerPayment = new Dictionary<decimal, int>
            {
                { 5.00m, 1 },{ 0.5m, 1 }

            };
var change = _changeService.CalculateChange(itemPrice, customerPayment);
```


## Exceptions
**ChangeCalculatorService**
<br/>
An `ArgumentException` will be thrown if the total amount provided by the customer is less than the price of the item.
<br/>
**ChangeCalculatorConfigurationService**
<br/>
`InvalidConfigurationVariableFormatException` Exception thrown when the format of the configuration variable 'Denominations' is invalid. 
<br/> CSV format is expected denomination=quantity e.g. 0.01=1,0.05=2
<br/>
`MissingConfigurationException` Exception thrown when the configuration variable 'Denominations' is missing



