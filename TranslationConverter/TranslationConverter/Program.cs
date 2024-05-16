using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

namespace TranslationConverter;

public class TranslationConverter
{

    //rework utf 8

    static void Main(string[] args)
    {

        //string jsonFilePath = @"C:\temp\locale\en-GB\translations.json";
        string jsonFilePath = @"C:\temp\translation.json";
        string csvFilePath = @"C:\temp\CSVoutput.csv";

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
