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

        //------------------------CONSOLE EXPERIENCE---------------
        //Directory specification
        DirectoryInfo di = new DirectoryInfo("C:\\temp\\translationConverterFiles\\1_jsonInputFile");
        FileInfo[] files = di.GetFiles("*.json");

        //Reads the files in the folder
        Console.WriteLine("Input folder files:");
        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {files[i].Name}");
        }
        Console.WriteLine();

        //for debug
        int length = files.Length;
        Console.WriteLine($"The length is: {length}");


        //File selection
        Console.WriteLine("Select file:");
        string a;
        int x = 0;
        bool success = false;
        int number = 0;

        while (!success)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out number))
            {
                var idx = number - 1; //zerobased

                if (idx >= files.Length || idx < 0)
                {
                    Console.WriteLine("Your number is not on the list! Please repeat.");
                    success = false;
                }
                else
                {
                    x = idx;
                    Console.WriteLine($"Selected index is {idx}");
                    success = true;
                }
            }
            else
            {
                Console.WriteLine("Bad input. Repeat please.");
                success = false;
            }
        }
        
        int index = x;
        Console.WriteLine($"Index is {index}");
        Console.WriteLine($"The selected file path is: {files[index].FullName}");

        jsonFilePath = files[index].FullName;
        Console.WriteLine($"jsonFilePath is : {jsonFilePath}");

        //---------------------------------------------------------------------------------------------------------
        string? jsonContent = JsonHandler.ReadJsonFile(jsonFilePath);
        if (jsonContent==null)
        {
            Console.WriteLine($"Error occured, jsonContent is null.");
            Console.WriteLine("");
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
            return;
        }
        //Console.WriteLine(jsonContent);

        var data = JsonHandler.JsonToDictionary(jsonContent); // Get dictionary.
        JsonHandler.WriteDataToCsv(csvFilePath, data); // Pass to CSV writer.

        Console.WriteLine("Conversion completed successfully.");
        Console.WriteLine("");
        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();
    }
}
