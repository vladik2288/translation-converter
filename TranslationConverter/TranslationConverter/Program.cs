using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

namespace TranslationConverter;

public class TranslationConverter
{
    //rewrite switch
    //conversion problem
    //try catch

    //rework utf 8

    static void Main(string[] args)
    {

        string jsonFilePath = @"C:\temp\locale\ua-UA\translations.json";
        string csvFilePath = @"C:\temp\output.csv";

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
