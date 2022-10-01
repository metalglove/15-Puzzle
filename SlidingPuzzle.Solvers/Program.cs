using SlidingPuzzle.PatternDatabase;
using SlidingPuzzle.Solvers;

internal class Program
{
    const int PuzzleSize = 4;
    const int MovablePiece = PuzzleSize * PuzzleSize - 1;

    private static async Task Main(string[] args)
    {
        int[] puzzleState = CreatePuzzleState();

        await SolvePuzzle(puzzleState);

        Console.ReadKey();
    }

    private static bool CheckSolvability(List<int> tempPuzzleState)
    {
        int inversions = 0;
        List<int> checker = tempPuzzleState.ToList();
        checker.Remove(MovablePiece);
        for (int i = 0; i < checker.Count; i++)
        {
            // Check if a larger number exists after the current
            // place in the array, if so increment inversions.
            for (int j = i + 1; j < checker.Count; j++)
                if (checker[i] > checker[j])
                    inversions++;
        }
        // If inversions is even, the puzzle is solvable.
        return inversions % 2 == 0;
    }

    private static int[] CreatePuzzleState()
    {
        List<int> tempPuzzleState = new List<int>();
        for (int i = 0; i < PuzzleSize * PuzzleSize; i++)
            tempPuzzleState.Add(i);

        Random random = new Random();
        tempPuzzleState = tempPuzzleState.OrderBy(item => random.Next()).ToList();
        while (!CheckSolvability(tempPuzzleState))
        {
            tempPuzzleState.ForEach(inter => Console.Write(inter + " "));
            Console.WriteLine("");
            tempPuzzleState = tempPuzzleState.OrderBy(item => random.Next()).ToList();
            Console.WriteLine("Not solvable game state, creating new game state.");
        }
        tempPuzzleState.ForEach(inter => Console.Write(inter + " "));
        Console.WriteLine("");
        return tempPuzzleState.ToArray();
    }

    private static async Task SolvePuzzle(int[] puzzleState)
    {
        //AStar solver = new AStar(puzzleState, PuzzleSize);
        IDAStar solver = new IDAStar(puzzleState, PuzzleSize);
        await solver.InitializeAsync();

        DateTime StartTime = DateTime.Now;
        List<Direction> directions = solver.FindPath();

        DateTime EndingTime = DateTime.Now;
        var diff = EndingTime.Subtract(StartTime);
        var res = string.Format("Hours: {0} Minutes: {1} Seconds: {2} Milliseconds: {3}", diff.Hours, diff.Minutes, diff.Seconds, diff.Milliseconds);
        Console.WriteLine(res);
        Console.WriteLine("Puzzle solved. Moves: " + directions.Count);
        foreach (Direction Direction in directions)
        {
            Console.Write(Direction + " ");
        }
    }
}