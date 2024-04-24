// See https://aka.ms/new-console-template for more information

using TownSuite.ConversionServer.Common.Models.Errors;
using TownSuite.ConversionServer.Utilities.REPL;

while (true)
{
    Console.WriteLine("--- Available commands ---");
    Console.WriteLine("convert-pdf: Converts a PDF to PNG files.");
    Console.WriteLine("exit: Exits the program.");
    Console.WriteLine("Enter a command:");

    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input))
    {
        Console.WriteLine("Missing input.");
        return;
    }
    if (input.Equals("convert-pdf", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Give file path:");
        var filePath = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("Missing input.");
            return;
        }
        using var fileStream = System.IO.File.OpenRead(filePath);

        var client = new ConversionClient();
        var results = await client.ConvertPdfAsync(fileStream);

        var folderPath = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(folderPath))
        {
            Console.WriteLine("Invalid folder.");
            return;
        }
        var counter = 0;
        foreach (var item in results.Data)
        {
            counter++;
            var pngPath = Path.Combine(folderPath, $"{counter}.png");
            await File.WriteAllBytesAsync(pngPath, item);
        }
    }
    else if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
}


public class ItemResponseModel<T>
{
    public T Data { get; set; }
    public ResponseErrorModel Error { get; set; }
}