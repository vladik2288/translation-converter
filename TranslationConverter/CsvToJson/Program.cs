using Microsoft.Extensions.Configuration;

namespace CsvToJson;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();

        string jsonFilePath = config["filePath:jsonFile"];
        string csvFilePath = config["filePath:csvFile"];

        string jsonContent = CsvHandler.CsvToJson(csvFilePath);
        File.WriteAllText(jsonFilePath, jsonContent);
        Console.WriteLine("CSV to JSON conversion completed.");
    }
}
