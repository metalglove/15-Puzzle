using System.Text;

namespace SlidingPuzzle.PatternDatabase
{
    public class PatternBoard
    {
        public int[] PatternState { get; set; }
        public int[] Pattern { get; set; }
        public Dictionary<string, int> ClosedSet { get; set; }
        public int PuzzleSize { get; set; }
        public int MovablePiece { get; set; }

        public PatternBoard(int puzzleSize, int[] pattern)
        {
            int[] board = new int[puzzleSize * puzzleSize];
            Console.WriteLine("Pattern:" + string.Join(",", pattern));
            Array.Fill(board, -1);
            foreach (int index in pattern)
            {
                board[index] = index;
            }
            int whitePuzzlePiece = board.Length - 1;
            board[whitePuzzlePiece] = whitePuzzlePiece;
            Console.WriteLine("Board:" + string.Join(",", board));
            PatternState = board;
            Pattern = pattern;
            PuzzleSize = puzzleSize;
            MovablePiece = (PuzzleSize * PuzzleSize) - 1;
            ClosedSet = new Dictionary<string, int>();
        }

        public string Hash(int[] puzzleState)
        {
            int[] board = new int[puzzleState.Length];
            Array.Fill(board, -1);
            for (int i = 0; i < board.Length; i++)
            {
                int num = puzzleState[i];
                if (Pattern.Contains(num) || num == MovablePiece)
                    board[i] = num;
            }
            string patternStateHased = PatternNode.Hash(board);
            return patternStateHased.Remove(patternStateHased.Length - 2, 2); // to closed patternstate hash
        }

        public void GeneratePermutations()
        {
            PatternNode startingNode = new PatternNode(PatternState, 0, Direction.None);
            string pattern = string.Join(",", Pattern);

            int iterations = 0;
            int totalIterations = PermutationsFormula(PuzzleSize * PuzzleSize, Pattern.Length + 1);

            Console.WriteLine("Total ITERATIONS: " + totalIterations);

            // pre-allocate the dictionaries.
            ClosedSet = new Dictionary<string, int>(totalIterations);
            Dictionary<string, PatternNode> openSet = new(totalIterations)
            {
                { startingNode.PatternState, startingNode }
            };
            HashSet<int> visited = new(totalIterations);

            // after the dictionaries are pre-allocated, the GC has to be called (performance).
            GC.Collect();

            DateTime dateTime = DateTime.Now;

            while (openSet.Any())
            {
                if (iterations % 100_000 == 0)
                {
                    DateTime now = DateTime.Now;
                    double delta = (now - dateTime).TotalMilliseconds;
                    StringBuilder stringBuilder = new();
                    stringBuilder.Append($"Pattern: {pattern} | Iteration: {iterations:D8} of {totalIterations:D8} | Time Elapsed: {delta}ms{Environment.NewLine}");
                    stringBuilder.Append($"Visited: {visited.Count}{Environment.NewLine}");
                    stringBuilder.Append($"OpenSet: {openSet.Count}{Environment.NewLine}");
                    stringBuilder.Append($"ClosedSet: {ClosedSet.Count}");
                    Console.WriteLine(stringBuilder.ToString());
                    dateTime = now;
                }

                KeyValuePair<string, PatternNode> node = openSet.First();
                openSet.Remove(node.Key);
                visited.Add(node.Key.GetHashCode());

                string closedPatternState = node.Value.ClosedPatternState;
                if (ClosedSet.TryGetValue(closedPatternState, out int heuristicValue))
                {
                    if (heuristicValue > node.Value.HeuristicValue)
                        ClosedSet[closedPatternState] = node.Value.HeuristicValue;
                }
                else
                    ClosedSet.Add(closedPatternState, node.Value.HeuristicValue);

                List<PatternNode> childNodes = node.Value.GenerateChildNodes(Pattern);
                foreach (PatternNode childNode in childNodes)
                {
                    string childHashCode = childNode.PatternState;
                    if (visited.Contains(childHashCode.GetHashCode()))
                        continue;
                    if (!openSet.ContainsKey(childHashCode))
                        openSet.Add(childHashCode, childNode);
                }

                iterations++;
            }
        }

        private float Factorial(int n)
        {
            if (n <= 1)
                return 1;
            return n * Factorial(n - 1);
        }

        private int PermutationsFormula(int n, int r) // also known as nPr
        {
            return (int)Math.Floor(Factorial(n) / Factorial(n - r));
        }
    }
}