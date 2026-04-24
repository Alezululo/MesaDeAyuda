using Microsoft.AspNetCore.Identity;

public class HashGenerator
{
    public static void Generar()
    {
        var hasher = new PasswordHasher<object>();

        Console.WriteLine(hasher.HashPassword(null, "1234"));
    }
}