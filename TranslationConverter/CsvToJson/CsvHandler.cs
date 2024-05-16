using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using System.Text.Json;

namespace CsvToJson;


public static class CsvHandler
{
    public static string CsvToJson(string csvFilePath)
    {
        //var records = new List<Dictionary<string, string>>();
        var records = new List<CsvRow>();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        //reading
        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, config))
        {
            var rows = csv.GetRecords<CsvRow>();
            records.AddRange(rows);
        }

        //decode keys
        var nestedData = new Dictionary<string, object>();
        foreach (var record in records)
        {
            InsertNestedDictionary(nestedData, record.Key, record.Text);
        }

        string json = JsonSerializer.Serialize(nestedData, new JsonSerializerOptions { WriteIndented = true });
        return json;
    }

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
}