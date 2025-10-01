// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;

class Product
{
    public string? Name { get; set; }
    public double Price { get; set; }

    public List<string>? Tags { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        // var product = new Product
        // {
        //     Name = "Laptop",
        //     Price = 999.99,
        //     Tags = new List<string> { "Electronics", "Computers", "Portable" }
        // };

        // string json = JsonConvert.SerializeObject(product, Formatting.Indented);
        // Console.WriteLine($"The Serialized Object is: {json}");


        string json = @"{
            'Name': 'Laptop',
            'Price': 999.99,
            'Tags': ['Electronics', 'Computers', 'Portable']
        }";

        var product = JsonConvert.DeserializeObject<Product>(json);
        Console.WriteLine($"The Deserialized Object is: {product?.Name}, {product?.Price}, {string.Join(", ", product?.Tags ?? new List<string>())}");
    }
}

