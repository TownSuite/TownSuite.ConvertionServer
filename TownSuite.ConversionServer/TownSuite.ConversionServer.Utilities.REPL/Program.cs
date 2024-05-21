// See https://aka.ms/new-console-template for more information

using System.Net.Mime;
using TownSuite.ConversionServer.Utilities.REPL;

while (true)
{
    Console.WriteLine("--- Available commands ---");
    Console.WriteLine("convert-pdf: Converts a PDF to PNG files.");
    Console.WriteLine("convert-pdf-legacy: Converts a PDF to PNG files using byte[].");
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
        var folderPath = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(folderPath))
        {
            Console.WriteLine("Invalid folder.");
            return;
        }
        
        using var fileStream = System.IO.File.OpenRead(filePath);

        var client = new ConversionClient();
        await client.ConvertPdfAsync(fileStream,
            async (results, contentType) =>
            {
                string pngPath;
                if (contentType == MediaTypeNames.Application.Zip)
                    pngPath = Path.Combine(folderPath, $"result.zip");
                else
                    pngPath = Path.Combine(folderPath, $"result.png");
                using var fileResult = File.OpenWrite(pngPath);
                await results.CopyToAsync(fileResult);
            });
    }
    else if (input.Equals("convert-pdf-legacy", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Give file path:");
        var filePath = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("Missing input.");
            return;
        }
        var folderPath = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(folderPath))
        {
            Console.WriteLine("Invalid folder.");
            return;
        }

        var file = await System.IO.File.ReadAllBytesAsync(filePath);

        var client = new ConversionClient();
        var response = await client.ConvertPdfAsBytesAsync(file);
        if (response == null)
        {
            Console.WriteLine("An unexpected error occurred. A success status code was returned, but the response content was not found.");
            return;
        }
        if (string.IsNullOrEmpty(response.Error?.Message) == false)
        {
            Console.WriteLine($"Error: {response.Error.Message}");
            return;
        }

        Console.WriteLine($"Conversion complete. {response.Data.Count()} files returned.");
        Console.WriteLine($"Downloading files.");
        var count = 0;
        foreach (var item in response.Data)
        {
            count++;
            var downloadPath = Path.Combine(folderPath, $"result{count}.png");
            await File.WriteAllBytesAsync(downloadPath, item);
        }
        Console.WriteLine($"Downloading finished!");
    }
    else if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
}