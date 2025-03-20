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

Console.WriteLine("LanguageExt.JsonSerializer Test");
Console.WriteLine("===============================\n");

// Test Option<T>
TestOption();

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

// void TestTry()
// {
//     Console.WriteLine("Testing Try<T> Serialization:");
//     Console.WriteLine("----------------------------");

//     // Success case
//     var successValue = Try<int>(() => 999);
//     string successJson = JsonSerializer.Serialize(successValue, options);
//     Console.WriteLine($"Success(999) serialized: {successJson}");

//     var deserializedSuccess = JsonSerializer.Deserialize<Try<int>>(successJson, options);
//     Console.WriteLine($"Deserialized: IsSucc={deserializedSuccess.IsSucc}, Value={deserializedSuccess.Match(v => v.ToString(), ex => $"Exception: {ex.Message}")}");

//     // Failure case
//     var failureValue = Try<int>(() => throw new InvalidOperationException("Something went wrong"));
//     string failureJson = JsonSerializer.Serialize(failureValue, options);
//     Console.WriteLine($"Failure serialized: {failureJson}");

//     var deserializedFailure = JsonSerializer.Deserialize<Try<int>>(failureJson, options);
//     Console.WriteLine($"Deserialized: IsSucc={deserializedFailure.IsSucc}, Value={deserializedFailure.Match(v => v.ToString(), ex => $"Exception: {ex.Message}")}");
    
//     Console.WriteLine();
// }

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
