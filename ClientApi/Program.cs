using ApiNamespace;

public class Program
{
    public static async Task Main(string[] args)
    {

        var baseUrl = "http://localhost:5065";
        var httpClient = new HttpClient();

        var ApiClient = new ApiClass(baseUrl, httpClient);

        var products = await ApiClient.ProductsAllAsync();

        foreach (var product in products)
        {
            Console.WriteLine($"{product.Id}: {product.Name} - ${product.Price}");
        }

        var newProduct = new Product
        {
            Id = 5,
            Name = "Juice Pilado",
            Price = 11.5
        };
    }
}

// await ApiClient.ProductsPOSTAsync(newProduct);




















// await new ClientGenerator().GenerateClient();

// // api http://localhost:5065/api/products 
// using System.Text.Json;
// using System.Net.Http.Json;

// var httpClient = new HttpClient();
// var response = await httpClient.GetAsync("http://localhost:5065/api/products");

// if (!response.IsSuccessStatusCode)
// {
//     Console.WriteLine("Error fetching products");
//     return;
// }

// var options = new JsonSerializerOptions
// {
//     PropertyNameCaseInsensitive = true
// };

// var products = await response.Content.ReadFromJsonAsync<List<Product>>(options);

// if (products == null)
// {
//     Console.WriteLine("No products found");
//     return;
// }

// foreach (var product in products)
// {
//     Console.WriteLine($"{product.Id}: {product.Name} - ${product.Price}");
// }


// class Product
// {
//     public int Id { get; set; }
//     public required string Name { get; set; }
//     public required decimal Price { get; set; }
// }