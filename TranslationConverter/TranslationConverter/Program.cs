using Microsoft.Extensions.Configuration;

namespace TranslationConverter;

public class TranslationConverter
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();

        string jsonFilePath = config["filePath:jsonFile"];
        string csvFilePath = config["filePath:csvFile"];

        Console.WriteLine(jsonFilePath);

        string? jsonContent = JsonHandler.ReadJsonFile(jsonFilePath);
        if (jsonContent==null)
        {
            Console.WriteLine($"Error occured, {jsonContent} is null.");
            return;
        }

        Console.WriteLine(jsonContent);
        //ConvertJsonToCsv(jsonContent, csvFilePath);
        //JsonHandler.WriteDataToCsv(csvFilePath);

        var data = JsonHandler.JsonToDictionary(jsonContent); // Get dictionary.
        JsonHandler.WriteDataToCsv(csvFilePath, data); // Pass to CSV writer.
    }


    

    
}
