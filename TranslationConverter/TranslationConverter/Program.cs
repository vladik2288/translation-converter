using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

public class JsonToCsvConverter
{

    static void Main(string[] args)
    {
        string jsonFilePath = @"C:\temp\translation.json";
        string csvFilePath = @"C:\temp\output.csv";

        ConvertJsonToCsv(jsonFilePath, csvFilePath);
    }

    public static void ConvertJsonToCsv(string jsonFilePath, string csvFilePath)
    {
        try
        {
            string jsonContent;
            using (StreamReader reader = new StreamReader(jsonFilePath)) 
            {
                jsonContent = reader.ReadToEnd();
            }
            //Console.WriteLine("StreamReader output: " + jsonContentStreamReader);

            // Deserialize JSON to a dictionary of dictionaries
            Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>();
            using (JsonDocument document = JsonDocument.Parse(jsonContent))
            {
                JsonElement root = document.RootElement;

                foreach (JsonProperty topLevelProperty in root.EnumerateObject())
                {
                    string topLevelKey = topLevelProperty.Name;
                    JsonElement topLevelValue = topLevelProperty.Value;

                    if (topLevelValue.ValueKind == JsonValueKind.Object)
                    {
                        // Extract nested properties into a flat dictionary
                        Dictionary<string, object> flatDictionary = new Dictionary<string, object>();
                        FlattenJson(topLevelValue, flatDictionary, topLevelKey);
                        data.Add(topLevelKey, flatDictionary);
                    }
                }
            }

            // Write data to CSV
            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true, Delimiter=";" }))
            {
                // Write header (column names)
                csv.WriteField("Category");
                csv.WriteField("Key");
                csv.WriteField("Text");
                csv.NextRecord();

                // Write rows
                foreach (var category in data.Keys)
                {
                    foreach (var kvp in data[category])
                    {
                        csv.WriteField(category);
                        csv.WriteField(kvp.Key);
                        csv.WriteField(kvp.Value?.ToString() ?? ""); // Handle null values
                        csv.NextRecord();
                    }
                }
            }

            Console.WriteLine("Conversion completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static void FlattenJson(JsonElement element, Dictionary<string, object> flatDictionary, string prefix)
    {
        foreach (JsonProperty property in element.EnumerateObject())
        {
            string key = $"{prefix}.{property.Name}";
            JsonElement value = property.Value;

            if (value.ValueKind == JsonValueKind.Object)
            {
                FlattenJson(value, flatDictionary, key); // Recursively flatten nested objects
            }
            else
            {
                flatDictionary.Add(key, DeserializeJsonValue(value)); // Add key-value pair to flat dictionary
            }
        }
    }

    private static object DeserializeJsonValue(JsonElement jsonElement)
    {
        // Handle different value types
        return jsonElement.ValueKind switch
        {
            JsonValueKind.Number => jsonElement.GetDouble(),
            JsonValueKind.String => jsonElement.GetString(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => jsonElement.ToString(), // Default case (treat as string)
        };
    }
}
