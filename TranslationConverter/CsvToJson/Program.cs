namespace CsvToJson
{
    
    internal class Program
    {
        static void Main(string[] args)
        {
            string csvFilePath = @"C:\temp\CSVoutput.csv";
            string jsonFilePath = @"C:\temp\JSONoutput.json";

            string jsonContent = CsvHandler.CsvToJson(csvFilePath);
            File.WriteAllText(jsonFilePath, jsonContent);
            Console.WriteLine("CSV to JSON conversion completed.");

        }
    }
}
