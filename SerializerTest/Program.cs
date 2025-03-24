using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.JsonSerializer;
using System;
using System.Text.Json;
using static LanguageExt.Prelude;

// Set up the JSON serializer options with our custom converters
var options = new JsonSerializerOptions
{
    WriteIndented = true
}.AddLanguageExtConverters();

// Set up another options instance with our array-style option converter
var arrayOptions = new JsonSerializerOptions
{
    WriteIndented = true
}.AddLanguageExtConverters();

Console.WriteLine("LanguageExt.JsonSerializer Test");
Console.WriteLine("===============================\n");

// Test Option<T>
TestChannelOption();
TestOption();

// Add the new test for array-style Option serialization
TestArrayOption();

// Test Either<L, R>
TestEither();

// Test Try<T>
// TestTry();

// Test Arr<T>
TestArr();

// Test Lst<T>
TestLst();

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

void TestChannelOption() {
    Console.WriteLine("Testing Option<Battery> in Channel class:");
    Console.WriteLine("---------------------------------------");
    
    var channel = new Channel(1, Some(new Battery("!", BatteryLevel.OK)));
    
    string json = JsonSerializer.Serialize(channel, options);
    Console.WriteLine($"Channel with Option<Battery> serialized: {json}");
    
    try {
        var deserializedChannel = JsonSerializer.Deserialize<Channel>(json, options);
        Console.WriteLine($"Deserialized: {deserializedChannel}");
        Console.WriteLine($"Battery IsSome: {deserializedChannel.Battery.IsSome}");
        deserializedChannel.Battery.IfSome(b => 
            Console.WriteLine($"Battery Code: {b.Code}, Level: {b.Level}")
        );
    }
    catch (Exception ex) {
        Console.WriteLine($"Deserialization error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
    
    Console.WriteLine();
    
    // Test with None
    var channelWithNoBattery = new Channel(2, Option<Battery>.None);
    string noneJson = JsonSerializer.Serialize(channelWithNoBattery, options);
    Console.WriteLine($"Channel with Option<Battery>.None serialized: {noneJson}");
    
    try {
        var deserializedNoneChannel = JsonSerializer.Deserialize<Channel>(noneJson, options);
        Console.WriteLine($"Deserialized: {deserializedNoneChannel}");
        Console.WriteLine($"Battery IsNone: {deserializedNoneChannel.Battery.IsNone}");
    }
    catch (Exception ex) {
        Console.WriteLine($"Deserialization error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
    
    Console.WriteLine();
}

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

void TestArrayOption()
{
    Console.WriteLine("Testing Array-Style Option<T> Serialization:");
    Console.WriteLine("------------------------------------------");

    // Some case
    var someValue = Some(42);
    string someJson = JsonSerializer.Serialize(someValue, arrayOptions);
    Console.WriteLine($"Some(42) serialized as array: {someJson}");

    var deserializedSome = JsonSerializer.Deserialize<Option<int>>(someJson, arrayOptions);
    Console.WriteLine($"Deserialized: IsSome={deserializedSome.IsSome}, Value={deserializedSome.Match(v => v.ToString(), () => "None")}");

    // None case
    var noneValue = Option<int>.None;
    string noneJson = JsonSerializer.Serialize(noneValue, arrayOptions);
    Console.WriteLine($"None serialized as array: {noneJson}");

    var deserializedNone = JsonSerializer.Deserialize<Option<int>>(noneJson, arrayOptions);
    Console.WriteLine($"Deserialized: IsSome={deserializedNone.IsSome}, Value={deserializedNone.Match(v => v.ToString(), () => "None")}");
    
    // Test with a complex object
    var complexOption = Some(new Battery("B123", BatteryLevel.OK));
    string complexJson = JsonSerializer.Serialize(complexOption, arrayOptions);
    Console.WriteLine($"Some(Battery) serialized as array: {complexJson}");
    
    var deserializedComplex = JsonSerializer.Deserialize<Option<Battery>>(complexJson, arrayOptions);
    Console.WriteLine($"Deserialized Battery: IsSome={deserializedComplex.IsSome}");
    deserializedComplex.IfSome(b => Console.WriteLine($"  Battery Code: {b.Code}, Level: {b.Level}"));
    
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


void TestArr()
{
    Console.WriteLine("Testing Arr<T> Serialization:");
    Console.WriteLine("----------------------------");

    // Create an Arr
    var arrValue = Arr.create(1, 2, 3, 4, 5);
    string arrJson = JsonSerializer.Serialize(arrValue, options);
    Console.WriteLine($"Arr<int> serialized: {arrJson}");

    var deserializedArr = JsonSerializer.Deserialize<Arr<int>>(arrJson, options);
    Console.WriteLine($"Deserialized: Count={deserializedArr.Count}, Values=[{string.Join(", ", deserializedArr)}]");
    
    // Test empty Arr
    var emptyArr = Arr<string>.Empty;
    string emptyArrJson = JsonSerializer.Serialize(emptyArr, options);
    Console.WriteLine($"Empty Arr<string> serialized: {emptyArrJson}");
    
    var deserializedEmptyArr = JsonSerializer.Deserialize<Arr<string>>(emptyArrJson, options);
    Console.WriteLine($"Deserialized empty: Count={deserializedEmptyArr.Count}, IsEmpty={deserializedEmptyArr.IsEmpty}");
    
    Console.WriteLine();
}

void TestLst()
{
    Console.WriteLine("Testing Lst<T> Serialization:");
    Console.WriteLine("----------------------------");
    var list = new List<int> { 1, 2, 3, 4 };
    // Create a Lst - fix the creation method
    var lstValue = new Lst<string> ([ "apple", "banana", "cherry"] );
    string lstJson = JsonSerializer.Serialize(lstValue, options);
    Console.WriteLine($"Lst<string> serialized: {lstJson}");

    var deserializedLst = JsonSerializer.Deserialize<Lst<string>>(lstJson, options);
    Console.WriteLine($"Deserialized: Count={deserializedLst.Count}, Values=[{string.Join(", ", deserializedLst)}]");
    
    // Test empty Lst - fix the empty creation
    var emptyLst = Lst<double>.Empty;
    string emptyLstJson = JsonSerializer.Serialize(emptyLst, options);
    Console.WriteLine($"Empty Lst<double> serialized: {emptyLstJson}");
    
    var deserializedEmptyLst = JsonSerializer.Deserialize<Lst<double>>(emptyLstJson, options);
    Console.WriteLine($"Deserialized empty: Count={deserializedEmptyLst.Count}, IsEmpty={deserializedEmptyLst.IsEmpty}");
    
    Console.WriteLine();
}
public record Channel(int ChannelId, Option<Battery> Battery);
//public record Channel(int ChannelId, Option<Battery> BattleWithLevel);
/// <summary>
/// 电池
/// </summary>
/// <param name="Code"></param>
/// <param name="Level"></param>
public record Battery(string Code, BatteryLevel Level);

public enum BatteryLevel
{
    None = 0,
    OK = 1,
    NG = 2,
    Fake = 3,
    REWORK = 4,
    EMPTY = 5,
    ManualReWork = 6,
    OK2 = 7,
    REWORK2 = 8,
    Unknown = 999,
    E99 = 100,
    DSD = 101,
}
