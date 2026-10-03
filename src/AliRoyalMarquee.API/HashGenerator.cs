using System;
using Microsoft.AspNetCore.Identity;

namespace HashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var hasher = new PasswordHasher<object>();
            var hash = hasher.HashPassword(null, "admin123");
            Console.WriteLine(hash);
        }
    }
}
