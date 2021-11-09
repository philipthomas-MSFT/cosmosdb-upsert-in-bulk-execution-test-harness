//-----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------
// See https://aka.ms/new-console-template for more information

using Bogus;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

var connectionString = await HarnessUtility.GetCosmosDBConnectionStringAsync(connectionName: System.Environment.GetEnvironmentVariable("HARNESS_COSMOSDB_CONNECTIONSTRING_NAME"));

var cosmosClient = new CosmosClient(
    connectionString: connectionString,
    clientOptions: new CosmosClientOptions { AllowBulkExecution = true });

var database = cosmosClient.GetDatabase(id: System.Environment.GetEnvironmentVariable("HARNESS_COSMOSDB_DATABASE_ID"));
var container = database.GetContainer(id: System.Environment.GetEnvironmentVariable("HARNESS_COSMOSDB_CONTAINER_ID"));

var items = new Faker<Item>()
    .RuleFor(item => item.Id, fake => Guid.NewGuid().ToString())
    .RuleFor(item => item.FirstName, fake => fake.Person.FirstName)
    .RuleFor(item => item.LastName, fake => fake.Person.LastName)
    .Generate(10);

var tasks = from item in items
    let task = Task.Run(function: async() =>
    {
        Console.WriteLine(JsonConvert.SerializeObject(item));
        await container.UpsertItemAsync<Item>(item, new PartitionKey(item.LastName)).ConfigureAwait(continueOnCapturedContext: false);
    }) select task;

await Task.WhenAll(tasks).ConfigureAwait(continueOnCapturedContext: false);

Console.WriteLine("Done!");
