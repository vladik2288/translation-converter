using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.Text.Json;
using System.Text;

namespace TranslationConverter;

public static class JsonHandler
{
    public static string? ReadJsonFile(string jsonFilePath)
    {
        string? jsonContent = null;
        try
        {
            using (StreamReader reader = new StreamReader(jsonFilePath))
            {
                jsonContent = reader.ReadToEnd();
            }
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        //Console.WriteLine(jsonContent);
        return jsonContent;
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
            JsonValueKind.Undefined => throw new NotImplementedException(),
            JsonValueKind.Object => throw new NotImplementedException(),
            JsonValueKind.Array => throw new NotImplementedException(),
            _ => jsonElement.ToString(), // Default case (treat as string)
        };
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
                flatDictionary.Add(key.TrimStart('.'), DeserializeJsonValue(value));
            }
        }
    }

    public static Dictionary<string, object> JsonToDictionary(string jsonContent)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        using (JsonDocument document = JsonDocument.Parse(jsonContent))
        {
            JsonElement root = document.RootElement;

            // Flatten the entire JSON into a single level dictionary
            FlattenJson(root, data, "");
        }
        return data; // Return the filled dictionary.
    }

    public static void WriteDataToCsv(string csvFilePath, Dictionary<string, object> data)
    {
        using (var writer = new StreamWriter(csvFilePath, false, Encoding.Unicode))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true, Delimiter = ";" }))
        {
            // Write header (column names)
            csv.WriteField("Key");
            csv.WriteField("Text");
            csv.NextRecord();

            // Write rows
            foreach (var kvp in data)
            {
                csv.WriteField(kvp.Key.TrimStart('.')); // Remove the leading dot from the key
                csv.WriteField(kvp.Value?.ToString() ?? ""); // Handle null values
                csv.NextRecord();
            }
        }
    }
}

