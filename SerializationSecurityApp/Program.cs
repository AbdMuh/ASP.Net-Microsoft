using System;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

public class Program
{
    public class Person
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Hash { get; set; }

        public void EncryptPassword()
        {
            Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Password));
        }

        public string GenerateHash()
        {
            using (var sha256 = SHA256.Create())
            {
                // Only include fields we want to protect
                var coreData = new
                {
                    Name,
                    Email,
                    Password
                };

                var jsonData = JsonSerializer.Serialize(coreData);
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(jsonData));
                return Convert.ToBase64String(bytes);
            }
        }
    }

    public static string SerializePerson(Person person)
    {
        if (string.IsNullOrEmpty(person.Name) || string.IsNullOrEmpty(person.Email) || string.IsNullOrEmpty(person.Password))
        {
            Console.WriteLine("All fields must be provided and valid.");
            return string.Empty;
        }

        // Encrypt and generate hash
        person.EncryptPassword();
        person.Hash = person.GenerateHash();

        Console.WriteLine($"🔐 Hash generated: {person.Hash}");

        var json = JsonSerializer.Serialize(person);
        Console.WriteLine($"🧾 Serialized JSON: {json}");
        return json;
    }

    public static Person? DeserializePerson(string json, bool isTrusted)
    {
        if (!isTrusted || string.IsNullOrEmpty(json))
        {
            Console.WriteLine("⚠️ Deserialization from untrusted sources is not allowed.");
            return null;
        }

        // Deserialize JSON
        var person = JsonSerializer.Deserialize<Person>(json);
        if (person == null)
        {
            Console.WriteLine("⚠️ Deserialization failed.");
            return null;
        }

        // Recalculate hash from the same core properties
        string recalculatedHash = person.GenerateHash();

        if (person.Hash != recalculatedHash)
        {
            Console.WriteLine("❌ Data integrity check failed!");
            return null;
        }

        Console.WriteLine("✅ Data integrity verified successfully!");
        return person;
    }

    public static void Main()
    {
        var person = new Person
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "1234"
        };

        var json = SerializePerson(person);
        var deserializedPerson = DeserializePerson(json, true);
    }
}
