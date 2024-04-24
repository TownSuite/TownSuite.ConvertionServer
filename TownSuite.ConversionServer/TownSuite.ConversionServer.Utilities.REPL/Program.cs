// See https://aka.ms/new-console-template for more information

using System.Text;
using TownSuite.ConversionServer.Common.Models.Errors;
using TownSuite.ConversionServer.Utilities.Newtonsoft;

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

        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:8443/PdfConverter/FromStream");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes("username:password")));
        using var content = new StreamContent(fileStream);
        request.Content = content;
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var resultString = await response.Content.ReadAsStringAsync();
        var jsonConverter = new NewtonsoftJsonSerializer();
        var results = jsonConverter.Deserialize<ItemResponseModel<IEnumerable<byte[]>>>(resultString);

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