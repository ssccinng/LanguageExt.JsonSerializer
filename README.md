# LanguageExt.JsonSerializer

```cs
using System;
using System.Text.Json;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.JsonSerializer;

// Set up the JSON serializer options with our custom converters
var options = new JsonSerializerOptions
{
    WriteIndented = true
}.AddLanguageExtConverters();

Console.WriteLine("LanguageExt.JsonSerializer Test");
Console.WriteLine("===============================\n");

// Test Option<T>
TestOption();

// Test Either<L, R>
TestEither();

// Test Try<T>
// TestTry();

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

void TestOption()
{
    Console.WriteLine("Testing Option<T> Serialization:");
    Console.WriteLine("-------------------------------");

    // Some case
    var someValue = Some(42);
    string someJson = JsonSerializer.Serialize(someValue, options);
    Console.WriteLine($"Some(42) serialized: {someJson}");

    var deserializedSome = JsonSerializer.Deserialize<Option<int>>(someJson, options);
    Console.WriteLine($"Deserialized: IsSome={deserializedSome.IsSome}, Value={deserializedSome.Match(v => v.ToString(), () => "None")}");

    // None case
    var noneValue = Option<int>.None;
    string noneJson = JsonSerializer.Serialize(noneValue, options);
    Console.WriteLine($"None serialized: {noneJson}");

    var deserializedNone = JsonSerializer.Deserialize<Option<int>>(noneJson, options);
    Console.WriteLine($"Deserialized: IsSome={deserializedNone.IsSome}, Value={deserializedNone.Match(v => v.ToString(), () => "None")}");
    
    Console.WriteLine();
}

void TestEither()
{
    Console.WriteLine("Testing Either<L, R> Serialization:");
    Console.WriteLine("----------------------------------");

    // Right case
    var rightValue = Right<string, int>(123);
    string rightJson = JsonSerializer.Serialize(rightValue, options);
    Console.WriteLine($"Right(123) serialized: {rightJson}");

    var deserializedRight = JsonSerializer.Deserialize<Either<string, int>>(rightJson, options);
    Console.WriteLine($"Deserialized: IsRight={deserializedRight.IsRight}, Value={deserializedRight.Match(r => r.ToString(), l => $"Left({l})")}");

    // Left case
    var leftValue = Left<string, int>("error");
    string leftJson = JsonSerializer.Serialize(leftValue, options);
    Console.WriteLine($"Left(\"error\") serialized: {leftJson}");

    var deserializedLeft = JsonSerializer.Deserialize<Either<string, int>>(leftJson, options);
    Console.WriteLine($"Deserialized: IsRight={deserializedLeft.IsRight}, Value={deserializedLeft.Match(r => r.ToString(), l => $"Left({l})")}");
    
    Console.WriteLine();
}

```