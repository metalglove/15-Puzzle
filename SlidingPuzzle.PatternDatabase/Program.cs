// See https://aka.ms/new-console-template for more information

using SlidingPuzzle.PatternDatabase;

PatternDatabase patternDatabase = new PatternDatabase(4);
await patternDatabase.InitializeDatabaseAsync();

Console.WriteLine("Successfully read pattern database!");
Console.WriteLine($"Patterns: {patternDatabase.Patterns.Count} | TotalPatternStates: {patternDatabase.Patterns.Sum(x => x.ClosedSet.Count)}!");

Console.ReadKey();