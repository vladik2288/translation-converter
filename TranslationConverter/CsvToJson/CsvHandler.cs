using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace CsvToJson;

public static class CsvHandler
{
    // Reads a CSV file and returns a list of CsvRow records.
    public static List<CsvRow> ReadCsvFile(string csvFilePath)
    {
        var records = new List<CsvRow>();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, config))
        {
            var rows = csv.GetRecords<CsvRow>();
            records.AddRange(rows);
        }

        return records;
    }

    // Builds a nested dictionary from a list of CsvRow records.
    public static Dictionary<string, object> BuildNestedDictionary(List<CsvRow> records)
    {
        var nestedData = new Dictionary<string, object>();

        foreach (var record in records)
        {
            InsertNestedDictionary(nestedData, record.Key, record.Text);
        }

        return nestedData;
    }

    // Converts a nested dictionary to a JSON string.
    public static string ConvertDictionaryToJson(Dictionary<string, object> nestedData)
    {
        string json = JsonSerializer.Serialize(nestedData, new JsonSerializerOptions 
        { 
            //added encoder for cyrillic letters
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic), 
            WriteIndented = true 
        });
        return json;
    }

    // Converts a CSV file to a JSON string.
    public static string CsvToJson(string csvFilePath)
    {
        var records = ReadCsvFile(csvFilePath);
        var nestedData = BuildNestedDictionary(records);
        string json = ConvertDictionaryToJson(nestedData);
        return json;
    }

    // Inserts a value into a nested dictionary based on a key path.
    private static void InsertNestedDictionary(Dictionary<string, object> currentDict, string keyPath, string value)
    {
        string[] parts = keyPath.Split('.');
        Dictionary<string, object> cursor = currentDict;

        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (!cursor.ContainsKey(parts[i]))
            {
                cursor[parts[i]] = new Dictionary<string, object>();
            }
            cursor = (Dictionary<string, object>)cursor[parts[i]];
        }

        cursor[parts[^1]] = value;  // `parts[^1]` is the last element in parts, equivalent to `parts[parts.Length - 1]`
    }

    //Create a current time timestamp for generated file
    public static string GetTimestampedJsonFilePath(string baseFolderPath)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return Path.Combine(baseFolderPath, $"jsonOutput_{timestamp}.json");
    }
}