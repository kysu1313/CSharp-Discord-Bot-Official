// using System;
// using System.IO;
// using System.Net;
// using System.Net.Http;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json.Linq;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using ClassLibrary.Data;
// using RedditSharp;
// using Coinbase;
// using Coinbase.Models;
// using NBitcoin;
// using System.Collections.Generic;
// using QBitNinja.Client;
// using QBitNinja.Client.Models;
// using Money = NBitcoin.Money;
// using Network = NBitcoin.Network;
// using System.Linq;
//
// namespace ClassLibrary.Helpers.Crypto
// {
//     public class Blockchain
//     {
//
//         private readonly ApplicationDbContext _context;
//         private readonly IConfiguration _config;
//         private static HttpClient _client; 
//         private readonly RefreshTokenWebAgentPool _redditAgentPool;
//         private CoinbaseClient _coinbaseClient;
//
//         public Blockchain(ApplicationDbContext context, IServiceProvider services) // , RefreshTokenWebAgentPool webAgentPool
//         {
//             _context = context; 
//             _config = services.GetRequiredService<IConfiguration>();
//             _coinbaseClient = new CoinbaseClient(new OAuthConfig { AccessToken = _config["CoinbaseApiKey"] });
//         }
//
//         private async Task<JObject> ApiBuilder(string url)
//         {
//             HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
//             webrequest.Method = "GET";
//             webrequest.ContentType = "application/x-www-form-urlencoded";
//             HttpWebResponse webresponse = (HttpWebResponse)await webrequest.GetResponseAsync();
//             Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
//             StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
//             string result = string.Empty;
//             var json = JObject.Parse(responseStream.ReadToEnd());
//             webresponse.Close();
//             return json;
//         }
//
//         //Key privateKey = new Key(); // generate a random private key
//         //PubKey publicKey = privateKey.PubKey;
//         //Console.WriteLine(publicKey); // 0251036303164f6c458e9f7abecb4e55e5ce9ec2b2f1d06d633c9653a07976560c
//
//         public async Task<List<string>> Testing()
//         {
//             var items = new List<string>();
//             Console.WriteLine("Hello World! " + new Key().GetWif(NBitcoin.Network.Main));
//             Key privateKey = new Key(); // generate a random private key
//             PubKey publicKey = privateKey.PubKey;
//             Console.WriteLine(publicKey);
//             items.Add(publicKey.ToString());
//
//             var publicKeyHash = publicKey.Hash;
//             Console.WriteLine(publicKeyHash); // f6889b21b5540353a29ed18c45ea0031280c42cf
//             var mainNetAddress = publicKeyHash.GetAddress(NBitcoin.Network.Main);
//             var testNetAddress = publicKeyHash.GetAddress(NBitcoin.Network.TestNet);
//             Console.WriteLine(mainNetAddress); // 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
//             Console.WriteLine(testNetAddress);
//             items.Add(mainNetAddress.ToString());
//             items.Add(testNetAddress.ToString());
//
//             publicKeyHash = new KeyId("14836dbe7f38c5ac3d49e8d790af808a4ee9edcf");
//
//             //var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);
//             //var mainNetAddress = publicKeyHash.GetAddress(Network.Main);
//
//             Console.WriteLine(mainNetAddress.ScriptPubKey); // OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG
//             Console.WriteLine(testNetAddress.ScriptPubKey); // OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG
//
//             items.Add(mainNetAddress.ScriptPubKey.ToString());
//             items.Add(testNetAddress.ScriptPubKey.ToString());
//
//             //Key privateKey = new Key(); // generate a random private key
//             BitcoinSecret mainNetPrivateKey = privateKey.GetBitcoinSecret(NBitcoin.Network.Main);  // generate our Bitcoin secret(also known as Wallet Import Format or simply WIF) from our private key for the mainnet
//             BitcoinSecret testNetPrivateKey = privateKey.GetBitcoinSecret(NBitcoin.Network.TestNet);  // generate our Bitcoin secret(also known as Wallet Import Format or simply WIF) from our private key for the testnet
//             Console.WriteLine(mainNetPrivateKey); // L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn
//             Console.WriteLine(testNetPrivateKey); // cVY5auviDh8LmYUW8AfafseD6p6uFoZrP7GjS3rzAerpRKE9Wmuz
//
//             bool WifIsBitcoinSecret = mainNetPrivateKey == privateKey.GetWif(NBitcoin.Network.Main);
//             Console.WriteLine(WifIsBitcoinSecret); // True
//
//             return items;
//         }
//
//         public async Task QbitTest()
//         {
//             // Create a client
//             QBitNinjaClient client = new QBitNinjaClient(NBitcoin.Network.Main);
//             // Parse transaction id to NBitcoin.uint256 so the client can eat it
//             var transactionId = uint256.Parse("f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94");
//             // Query the transaction
//             GetTransactionResponse transactionResponse = await  client.GetTransaction(transactionId);
//             NBitcoin.Transaction transaction = transactionResponse.Transaction;
//
//             Console.WriteLine(transactionResponse.TransactionId); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
//             Console.WriteLine(transaction.GetHash()); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
//
//
//             List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
//             foreach (var coin in receivedCoins)
//             {
//                 Money amount = (Money)coin.Amount;
//
//                 Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
//                 var paymentScript = coin.TxOut.ScriptPubKey;
//                 Console.WriteLine(paymentScript);  // It's the ScriptPubKey
//                 var address = paymentScript.GetDestinationAddress(NBitcoin.Network.Main);
//                 Console.WriteLine(address); // 1HfbwN6Lvma9eDsv7mdwp529tgiyfNr7jc
//                 Console.WriteLine();
//             }
//
//             var outputs = transaction.Outputs;
//             foreach (TxOut output in outputs)
//             {
//                 Money amount = output.Value;
//
//                 Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
//                 var paymentScript = output.ScriptPubKey;
//                 Console.WriteLine(paymentScript);  // It's the ScriptPubKey
//                 var address = paymentScript.GetDestinationAddress(Network.Main);
//                 Console.WriteLine(address);
//                 Console.WriteLine();
//             }
//
//             var inputs = transaction.Inputs;
//             foreach (TxIn input in inputs)
//             {
//                 OutPoint previousOutpoint = input.PrevOut;
//                 Console.WriteLine(previousOutpoint.Hash); // hash of prev tx
//                 Console.WriteLine(previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx
//                 Console.WriteLine();
//             }
//
//             Money twentyOneBtc = new Money(21, MoneyUnit.BTC);
//             var scriptPubKey = transaction.Outputs[0].ScriptPubKey;
//             TxOut txOut = transaction.Outputs.CreateNewTxOut(twentyOneBtc, scriptPubKey);
//
//             OutPoint firstOutPoint = receivedCoins[0].Outpoint;
//             Console.WriteLine(firstOutPoint.Hash); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
//             Console.WriteLine(firstOutPoint.N); // 0
//
//             Console.WriteLine(transaction.Inputs.Count); // 9
//
//             OutPoint firstPreviousOutPoint = transaction.Inputs[0].PrevOut;
//             var firstPreviousTransaction = client.GetTransaction(firstPreviousOutPoint.Hash).Result.Transaction;
//             Console.WriteLine(firstPreviousTransaction.IsCoinBase); // False
//
//             Money spentAmount = Money.Zero;
//             var spentCoins = transactionResponse.SpentCoins;
//
//             foreach (var spentCoin in spentCoins)
//             {
//                 spentAmount = (Money)spentCoin.Amount.Add(spentAmount);
//             }
//             Console.WriteLine(spentAmount.ToDecimal(MoneyUnit.BTC)); // 13.19703492
//
//             var fee = transaction.GetFee(spentCoins.ToArray());
//             Console.WriteLine(fee);
//
//         }
//
//         public static bool SelectCoins(ref HashSet<Coin> coinsToSpend,
//                                Money totalOutAmount, List<Coin> unspentCoins)
//         {
//             var haveEnough = false;
//             foreach (var coin in unspentCoins.OrderByDescending(x => x.Amount))
//             {
//                 coinsToSpend.Add(coin);
//                 // if doesn't reach amount, continue adding next coin
//                 if (coinsToSpend.Sum(x => x.Amount) < totalOutAmount) continue;
//                 else
//                 {
//                     haveEnough = true;
//                     break;
//                 }
//             }
//
//             return haveEnough;
//         }
//
//         //public static Dictionary<Coin, bool> GetUnspentCoins(IEnumerable<ISecret> secrets)
//         //{
//         //    var unspentCoins = new Dictionary<Coin, bool>();
//         //    foreach (var secret in secrets)
//         //    {
//         //        var destination =
//         //            secret.PrivateKey.ScriptPubKey.GetDestinationAddress(Config.Network);
//
//         //        var client = new QBitNinjaClient(Config.Network);
//         //        var balanceModel = client.GetBalance(destination, unspentOnly: true).Result;
//         //        foreach (var operation in balanceModel.Operations)
//         //        {
//         //            foreach (var elem in operation.ReceivedCoins.Select(coin => coin as Coin))
//         //            {
//         //                unspentCoins.Add(elem, operation.Confirmations > 0);
//         //            }
//         //        }
//         //    }
//
//         //    return unspentCoins;
//         //}
//
//         //public void SendCrypto()
//         //{
//         //    // 5. How much money we can spend?
//         //    Money availableAmount = Money.Zero;
//         //    Money unconfirmedAvailableAmount = Money.Zero;
//         //    foreach (var elem in unspentCoins)
//         //    {
//         //        // If can spend unconfirmed add all
//         //        if (Config.CanSpendUnconfirmed)
//         //        {
//         //            availableAmount += elem.Key.Amount;
//         //            if (!elem.Value)
//         //                unconfirmedAvailableAmount += elem.Key.Amount;
//         //        }
//         //        // else only add confirmed ones
//         //        else
//         //        {
//         //            if (elem.Value)
//         //            {
//         //                availableAmount += elem.Key.Amount;
//         //            }
//         //        }
//         //    }
//
//         //    // 6. How much to spend?
//         //    Money amountToSend = null;
//         //    string amountString = GetArgumentValue(args, argName: "btc", required: true);
//         //    if (string.Equals(amountString, "all", StringComparison.OrdinalIgnoreCase))
//         //    {
//         //        amountToSend = availableAmount;
//         //        amountToSend -= fee;
//         //    }
//         //    else
//         //    {
//         //        amountToSend = ParseBtcString(amountString);
//         //    }
//
//         //    // 7. Do some checks
//         //    if (amountToSend < Money.Zero || availableAmount < amountToSend + fee)
//         //        Exit("Not enough coins.");
//
//         //    decimal feePc = Math.Round((100 * fee.ToDecimal(MoneyUnit.BTC)) /
//         //                    amountToSend.ToDecimal(MoneyUnit.BTC));
//         //    if (feePc > 1)
//         //    {
//         //        Console.WriteLine();
//         //        Console.WriteLine($"The transaction fee is { feePc.ToString("0.#")}% of your transaction amount.");
//         //        Console.WriteLine($"Sending:\t {amountToSend.ToDecimal
//         //     (MoneyUnit.BTC).ToString("0.#############################")}btc");
//         //        //Console.WriteLine($"Fee:\t\t {feePc.ToDecimal(MoneyUnit.BTC).ToString("0.#############################")}btc");
//         //        ConsoleKey response = GetYesNoAnswerFromUser();
//         //        if (response == ConsoleKey.N)
//         //        {
//         //            //System.Exit("User interruption.");
//         //        }
//         //    }
//
//         //    var confirmedAvailableAmount = availableAmount - unconfirmedAvailableAmount;
//         //    var totalOutAmount = amountToSend + fee;
//         //    if (confirmedAvailableAmount < totalOutAmount)
//         //    {
//         //        var unconfirmedToSend = totalOutAmount - confirmedAvailableAmount;
//         //        Console.WriteLine();
//         //        Console.WriteLine($"In order to complete this transaction you have to spend 
//         //        { unconfirmedToSend.ToDecimal(MoneyUnit.BTC).ToString("0.#############################")}
//         //        unconfirmed btc.");
//         //        ConsoleKey response = GetYesNoAnswerFromUser();
//         //        if (response == ConsoleKey.N)
//         //        {
//         //            Exit("User interruption.");
//         //        }
//         //    }
//         //}
//
//         //public void ReceiveCrypto()
//         //{
//         //    var walletFilePath = GetWalletFilePath(args);
//         //    Safe safe = DecryptWalletByAskingForPassword(walletFilePath);
//
//         //    if (Config.ConnectionType == ConnectionType.Http)
//         //    {
//         //        // From now on we'll only work here
//         //    }
//         //    else if (Config.ConnectionType == ConnectionType.FullNode)
//         //    {
//         //        throw new NotImplementedException();
//         //    }
//         //    else
//         //    {
//         //        Exit("Invalid connection type.");
//         //    }
//
//         //    Dictionary<BitcoinAddress, List<BalanceOperation>> operationsPerReceiveAddresses =
//         //                     QueryOperationsPerSafeAddresses(safe, 7, HdPathType.Receive);
//         //}
//
//         //public static Dictionary<BitcoinAddress, List<BalanceOperation>>
//         //QueryOperationsPerSafeAddresses(Safe safe, int minUnusedKeys = 7, HdPathType? hdPathType = null)
//         //{
//         //    if (hdPathType == null)
//         //    {
//         //        Dictionary<BitcoinAddress, List<BalanceOperation>>
//         //           operationsPerReceiveAddresses = QueryOperationsPerSafeAddresses
//         //           (safe, 7, HdPathType.Receive);
//         //        Dictionary<BitcoinAddress, List<BalanceOperation>>
//         //           operationsPerChangeAddresses = QueryOperationsPerSafeAddresses
//         //           (safe, 7, HdPathType.Change);
//
//         //        var operationsPerAllAddresses =
//         //            new Dictionary<BitcoinAddress, List<BalanceOperation>>();
//         //        foreach (var elem in operationsPerReceiveAddresses)
//         //            operationsPerAllAddresses.Add(elem.Key, elem.Value);
//         //        foreach (var elem in operationsPerChangeAddresses)
//         //            operationsPerAllAddresses.Add(elem.Key, elem.Value);
//         //        return operationsPerAllAddresses;
//         //    }
//
//         //    var addresses = safe.GetFirstNAddresses(minUnusedKeys, hdPathType.GetValueOrDefault());
//         //    //var addresses = FakeData.FakeSafe.GetFirstNAddresses(minUnusedKeys);
//
//         //    var operationsPerAddresses = new Dictionary<BitcoinAddress, List<BalanceOperation>>();
//         //    var unusedKeyCount = 0;
//         //    foreach (var elem in QueryOperationsPerAddresses(addresses))
//         //    {
//         //        operationsPerAddresses.Add(elem.Key, elem.Value);
//         //        if (elem.Value.Count == 0) unusedKeyCount++;
//         //    }
//         //    Console.WriteLine($"{operationsPerAddresses.Count} {hdPathType} keys are processed.");
//
//         //    var startIndex = minUnusedKeys;
//         //    while (unusedKeyCount < minUnusedKeys)
//         //    {
//         //        addresses = new HashSet<BitcoinAddress>();
//         //        for (int i = startIndex; i < startIndex + minUnusedKeys; i++)
//         //        {
//         //            addresses.Add(safe.GetAddress(i, hdPathType.GetValueOrDefault()));
//         //            //addresses.Add(FakeData.FakeSafe.GetAddress(i));
//         //        }
//         //        foreach (var elem in QueryOperationsPerAddresses(addresses))
//         //        {
//         //            operationsPerAddresses.Add(elem.Key, elem.Value);
//         //            if (elem.Value.Count == 0) unusedKeyCount++;
//         //        }
//         //        Console.WriteLine($"{operationsPerAddresses.Count} {hdPathType} keys are processed.");
//         //        startIndex += minUnusedKeys;
//         //    }
//
//         //    return operationsPerAddresses;
//         //}
//
//         //public async Task TestPayment()
//         //{
//         //    var bitcoinPrivateKey = new BitcoinSecret("cN5YQMWV8y19ntovbsZSaeBxXaVPaK4n7vapp4V56CKx5LhrK2RS", Network.TestNet);
//         //    var network = bitcoinPrivateKey.Network;
//         //    var address = bitcoinPrivateKey.GetAddress(bitcoinPrivateKey);
//
//         //    Console.WriteLine(bitcoinPrivateKey); // cN5YQMWV8y19ntovbsZSaeBxXaVPaK4n7vapp4V56CKx5LhrK2RS
//         //    Console.WriteLine(address); // mkZzCmjAarnB31n5Ke6EZPbH64Cxexp3Jp
//
//         //    var client = new QBitNinjaClient(network);
//         //    var transactionId = uint256.Parse("0acb6e97b228b838049ffbd528571c5e3edd003f0ca8ef61940166dc3081b78a");
//         //    var transactionResponse = client.GetTransaction(transactionId).Result;
//
//         //    //3FaWBRm9Ak9QnSa1KgxmEYxsP9NuJCGYTb
//         //    var receivedCoins = transactionResponse.ReceivedCoins;
//         //    OutPoint outPointToSpend = null;
//         //    foreach (var coin in receivedCoins)
//         //    {
//         //        if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.GetAddress(ScriptPubKeyType.Legacy).ScriptPubKey)
//         //        {
//         //            outPointToSpend = coin.Outpoint;
//         //        }
//         //    }
//         //    if (outPointToSpend == null)
//         //        throw new Exception("TxOut doesn't contain our ScriptPubKey");
//         //    Console.WriteLine("We want to spend {0}. outpoint:", outPointToSpend.N + 1);
//         //}
//
//
//         public async Task<bool> SendCrypto(string fromWallet, string toWallet, float amount, string currency, string idem)
//         {
//             bool success = false;
//             try
//             {
//                 var createTx = new CreateTransaction
//                 {
//                     Type = "send",
//                     To = "1AUJ8z5RuHRTqD1eikyfUUetzGmdWLGkpT",
//                     Amount = 0.1m,
//                     Currency = "BTC",
//                     Idem = "9316dd16-0c05"
//                 };
//                 var response = await _coinbaseClient.Transactions.SendMoneyAsync("fff", createTx);
//
//                 if (response != null)
//                 {
//                     success = false;
//                 }
//             }
//             catch(Exception ex)
//             {
//                 Console.WriteLine(ex);
//             }
//             return success;
//         }
//
//     }
// }
