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

        // Read CSV file
        var records = CsvHandler.ReadCsvFile(csvFilePath);

        // Build nested dictionary from records
        var nestedData = CsvHandler.BuildNestedDictionary(records);

        // Convert nested dictionary to JSON
        string jsonContent = CsvHandler.ConvertDictionaryToJson(nestedData);

        //string jsonContent = CsvHandler.CsvToJson(csvFilePath);

        // Write JSON to file
        File.WriteAllText(jsonFilePath, jsonContent);
        Console.WriteLine("CSV to JSON conversion completed.");
    }
}
