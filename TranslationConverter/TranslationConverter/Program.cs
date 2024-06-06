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

        string jsonFolderPath = config["folderPath:jsonInput"];
        string csvOutputFolderPath = config["folderPath:csvOutput"];

        // CONSOLE
        //Directory specification
        DirectoryInfo di = new DirectoryInfo(jsonFolderPath);
        FileInfo[] files = di.GetFiles("*.json");

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

        string jsonFilePath = files[index].FullName;
        Console.WriteLine($"jsonFilePath is : {jsonFilePath}");
        Console.WriteLine();
        //-------------------------------------------

        string? jsonContent = JsonHandler.ReadJsonFile(jsonFilePath);
        if (jsonContent==null)
        {
            Console.WriteLine($"Error occured, jsonContent is null.");
            Console.WriteLine("");
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
            return;
        }

        var data = JsonHandler.JsonToDictionary(jsonContent); // Get dictionary.

        string timestampedCsvFilePath = JsonHandler.GetTimestampedCsvFilePath(csvOutputFolderPath);
        JsonHandler.WriteDataToCsv(timestampedCsvFilePath, data); // Pass to CSV writer.

        Console.WriteLine("Conversion completed successfully.");
        Console.WriteLine($"Output file location is: {timestampedCsvFilePath}");
        Console.WriteLine("");
        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();
    }
}
