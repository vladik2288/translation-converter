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

        string csvFolderPath = config["folderPath:csvInput"];
        string jsonFolderPath = config["folderPath:jsonOutput"];

        //CONSOLE
        //Directory specification
        DirectoryInfo di = new DirectoryInfo(csvFolderPath);
        FileInfo[] files = di.GetFiles("*.csv");

        //Reads the files in the folder
        Console.WriteLine("Input folder files:");
        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {files[i].Name}");
        }
        Console.WriteLine();

        //File selection
        Console.WriteLine("Select file:");

        int index = -1;
        bool success = false;

        while (!success)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out int number))
            {
                var idx = number - 1; //zerobased

                if (idx >= files.Length || idx < 0)
                {
                    Console.WriteLine("Your number is not on the list! Please repeat.");
                    success = false;
                }
                else
                {
                    index = idx;
                    //Console.WriteLine($"Selected index is {idx}");
                    success = true;
                }
            }
            else
            {
                Console.WriteLine("Bad input. Repeat please.");
                success = false;
            }
        }

        string csvFilePath = files[index].FullName;
        Console.WriteLine($"csvFilePath is : {csvFilePath}");
        Console.WriteLine();
        //-------------------------------------------

        // Read CSV file
        var records = CsvHandler.ReadCsvFile(csvFilePath);

        // Build nested dictionary from records
        var nestedData = CsvHandler.BuildNestedDictionary(records);

        // Convert nested dictionary to JSON
        string jsonContent = CsvHandler.ConvertDictionaryToJson(nestedData);

        //string jsonContent = CsvHandler.CsvToJson(csvFilePath);

        string timestampedCsvFilePath = CsvHandler.GetTimestampedJsonFilePath(jsonFolderPath);
        // Write JSON to file
        File.WriteAllText(timestampedCsvFilePath, jsonContent);

        Console.WriteLine("Conversion completed successfully.");
        Console.WriteLine($"Output file location is: {timestampedCsvFilePath}");
        Console.WriteLine("");
        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();
    }
}
