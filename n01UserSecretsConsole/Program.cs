using Microsoft.Extensions.Configuration;  
IConfiguration? Config;

Config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var apiKey = Config["ApiKey"];
Console.WriteLine(apiKey);
