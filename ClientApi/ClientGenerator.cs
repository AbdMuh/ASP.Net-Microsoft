using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using NSwag;
using NSwag.CodeGeneration.CSharp;


public class ClientGenerator
{
public async Task GenerateClient()
    {
        var httpClient = new HttpClient();
        var swaggerJson = await httpClient.GetStringAsync("http://localhost:5065/swagger/v1/swagger.json");
        var document = await OpenApiDocument.FromJsonAsync(swaggerJson);
        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "ApiClass",
            CSharpGeneratorSettings =
            {
                Namespace = "ApiNamespace"
            }
        };
        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();
        await File.WriteAllTextAsync("ApiClient.cs", code);
    }
}