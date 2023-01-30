// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using System;
using System.Text;
using CashMaster.POS.Configuration;
using CashMaster.POS.Contracts;
using CashMaster.POS.Services;
using Microsoft.Extensions.Configuration;


internal class Program
{
    private static void DisplayDenomination(Dictionary<decimal, int> denominations,string message)
    {
         
        Console.WriteLine(message);
        foreach (KeyValuePair<decimal, int> item in denominations)
        {
            Console.WriteLine($"{item.Key} x {item.Value}");
        }
    }
    private static void MakeTransaction(string message, decimal itemPrice, Dictionary<decimal, int> customerPayment, IChangeCalculator _changeService)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException($"'{nameof(message)}' cannot be null or empty.", nameof(message));
        

        if (customerPayment is null)
            throw new ArgumentNullException(nameof(customerPayment));

        if (_changeService is null)
            throw new ArgumentNullException(nameof(_changeService));
        

        var change = _changeService.CalculateChange(itemPrice, customerPayment);
        DisplayDenomination(customerPayment, "Customer's deposit");
        decimal customerTotal = customerPayment.Sum(x => x.Key * x.Value);
        Console.WriteLine($"The cost is {itemPrice} but  You deposit: {customerTotal} so CASH machine will return you: ");
        if (change.Count > 0)
        {
            foreach (var item in change)
            {
                Console.WriteLine($"{item.Key} x {item.Value}");
            }
        }
        else Console.WriteLine("NOTHING");

        DisplayDenomination(_changeService.Denominations, "Denominations in CASH machine");
        Console.WriteLine();

    }
    private static void Main()
    {
        StringBuilder theInfo = new StringBuilder("")
                                    .AppendLine("This process shows a way to consume ChangeCalculatorService")
                                    .AppendLine("Step 1: You need to specify a string that contains the target country denomination inventory of bills and coins")
                                    .AppendLine("For an Hypothetical country setx initial available inventory of denominations with the initial quantity Denominations \"0.01=100,0.05=100,0.10=100,0.25=100,0.50=100,1.00=100,2.00=100,5.00=100,10.00=100,20.00=100,50.00=100,100.00=100\"")
                                    .AppendLine("It can be read as I have 100 items of 0.01 value  100 items of 0.05 value and so on , you should specify all the valid denominations of course you can have 0 items of an specific value ")
                                    .AppendLine("e.g. For windows OS : setx Denominations \"0.01=100,0.05=100,0.10=100,0.25=100,0.50=100,1.00=100,2.00=100,5.00=100,10.00=100,20.00=100,50.00=100,100.00=100\" /M ")
                                    .AppendLine("or export Denominations \"0.01=100,0.05=100,0.10=100,0.25=100,0.50=100,1.00=100,2.00=100,5.00=100,10.00=100,20.00=100,50.00=100,100.00=100\" /M in Linux and MacOS.")
                                    .AppendLine("Step 2: Setup the configuration and inyect it to ChangeCalculatorService")
                                    .AppendLine("Configuration is flexible enough so you can trust in environment, json file, redis etc ")
                                    .AppendLine("I will recommend using enviroment configuration , of course for scalability we should use distributed db like redis")
                                    .AppendLine("For future it can become an api, it can be dockerized and deployed everywhere like Azure App Service, ACS,AKS, k8s etc")
                                    .AppendLine();
        
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(theInfo);
        Console.ForegroundColor = foregroundColor;
        try
        {



            // Our ChangeCalculatorConfigurationService is flexible enough to get configuration from env, json file...
            // I will use in memory config this time BUT as a guide I have commented another ways to setup the configuration

            /* var _envConfiguration = new ConfigurationBuilder()
                                .AddEnvironmentVariables()
                                .Build(); */
            /* var _jsonConfiguration = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                             .Build(); */

            /* var _redisConfiguration = new ConfigurationBuilder()
                                .AddRedis("redis_connection_string", options => {
                                                 options.KeyPrefix = "Denominations:";
                                        })
                                .Build(); */
            // configure this ONLY once

            Dictionary<string, string> initialData = new()
            {
                    {"Denominations", "0.01=0,0.05=0,0.10=0,0.25=0,0.50=0,1.00=0,2.00=0,5.00=0,10.00=0"}
                };

            var _inMemoryConfig = new ConfigurationBuilder().AddInMemoryCollection(initialData!).Build();
            //IChangeCalculatorConfiguration _config = new ChangeCalculatorConfigurationService(_envConfiguration);
            IChangeCalculatorConfiguration _config = new ChangeCalculatorConfigurationService(_inMemoryConfig);
            IChangeCalculator _changeService = new ChangeCalculatorService(_config);

            Console.WriteLine("");
            DisplayDenomination(_changeService.Denominations, "Initial CASH machine denominations");
            //service invocation
            //user1
            decimal itemPrice = 5.5m;
            Dictionary<decimal, int> customerPayment = new()
            {
                { 5.00m, 1 },{ 0.5m, 1 }

            };
            var message = $"Transaction for user1 itemPrice {itemPrice}";
            MakeTransaction(message,itemPrice, customerPayment, _changeService);
           
            // user2
            itemPrice = 5.25m;
            customerPayment = new Dictionary<decimal, int>
            {
                { 5.00m, 1 },{ 0.25m, 3 }

            };
            message = $"Transaction for user2 itemPrice {itemPrice}";
            MakeTransaction(message,itemPrice, customerPayment, _changeService);
            

            //user3
            itemPrice = 1.00m;
            customerPayment = new Dictionary<decimal, int>
            {
                { 2.00m, 1 }

            };
            message = $"Transaction for user3 itemPrice {itemPrice}";
            MakeTransaction(message, itemPrice, customerPayment, _changeService);
            //user4
            itemPrice = 10.25m;
            customerPayment = new Dictionary<decimal, int>
            {
                { 5.00m, 2 },
                { 0.50m, 1}

            };
            message = $"Transaction for user4 itemPrice {itemPrice}";
            MakeTransaction(message, itemPrice, customerPayment, _changeService);

            //user5
            itemPrice = 10.50m;
            customerPayment = new Dictionary<decimal, int>
            {
                { 20.00m, 1 },
                { 0.01m, 1}

            };
            message = $"Transaction for user5 itemPrice {itemPrice}";
            MakeTransaction(message, itemPrice, customerPayment, _changeService);

        }
        catch(Exception ex )
        {
            Console.WriteLine($"Exception happened -> {ex}");
        }



    }
}