using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Creation;
public static class RandomValuesGenerator
{
    const int strLength = 8;
    public static List<object> GetRandomValuesOfType(Type type, int length)
    {
        return Enumerable.Range(0, length)
            .Select(x => GetRandomValueOfType(type))
            .ToList();
    }
    public static object GetRandomValueOfType(Type type)
    {
        if (type == typeof(string))
        {
            return getRandomString();
        }
        if (type == typeof(int))
        {
            return Random.Shared.Next(0, 255);
        }
        if (type == typeof(long))
        {
            return Random.Shared.NextInt64();
        }
        if (type == typeof(float))
        {
            return Random.Shared.NextSingle();
        }
        if (type == typeof(double)) 
        { 
            return Random.Shared.NextDouble(); 
        }
        if (type == typeof(decimal))
        {
            return Random.Shared.NextDouble();
        }
        if (type == typeof(bool))
        {
            return Random.Shared.Next() % 2 == 0;
        }
        if (type.IsEnum)
        {
            var values = type.GetEnumValues();
            return values.GetValue(Random.Shared.Next(0, values.Length))!;    
        }
        if (type == typeof(Guid))
        {
            return Guid.NewGuid();
        }
        throw new ArgumentException($"Type {type} cannot be handled");
    }
    private static string getRandomString()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, strLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

