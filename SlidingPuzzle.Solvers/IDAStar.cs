namespace SlidingPuzzle.Solvers
{
    public class IDAStar : SolvingBase
    {
        private readonly PatternDatabase.PatternDatabase _patternDatabase;
        private int _nodesExplored = 0;

        public IDAStar(int[] startingState, int puzzleSize) : base(startingState, puzzleSize)
        {
            _patternDatabase = new PatternDatabase.PatternDatabase(puzzleSize);
        }

        public Task InitializeAsync()
        {
            return _patternDatabase.InitializeDatabaseAsync();
        }

        protected override void Search()
        {
            double bound = Heuristic(StartingNode);
            List<Node> path = new List<Node> { StartingNode };
            while (true)
            {
                (double t, bool found) = Search(path, 0, bound);
                if (found)
                    return;
                if (t == int.MaxValue)
                    return;
                bound = t;
            }
        }

        private int LinearConflicts(int[] puzzleState)
        {
            int conflicts = 0;
            bool[] in_col = new bool[PuzzleSize * PuzzleSize];
            bool[] in_row = new bool[PuzzleSize * PuzzleSize];

            for (int y = 0; y != PuzzleSize; ++y)
            {
                for (int x = 0; x != PuzzleSize; ++x)
                {
                    int i = y * PuzzleSize + x;

                    int bx = puzzleState[i] % PuzzleSize;
                    int by = puzzleState[i] / PuzzleSize;

                    in_col[i] = (bx == x);
                    in_row[i] = (by == y);
                }
            }

            for (int y = 0; y != PuzzleSize; ++y)
            {
                for (int x = 0; x != PuzzleSize; ++x)
                {
                    int i = y * PuzzleSize + x;

                    if (puzzleState[i] == MoveablePiece)
                        continue;

                    if (in_col[i])
                    {
                        for (int r = y; r != PuzzleSize; ++r) 
                        {
                            int j = r * PuzzleSize + x;

                            if (puzzleState[j] == MoveablePiece)
                                continue;

                            if (in_col[j] && puzzleState[j] < puzzleState[i])
                                ++conflicts;
                        }
                    }

                    if (in_row[i])
                    {
                        for (int c = x; c != PuzzleSize; ++c)
                        {
                            int j = y * PuzzleSize + c;

                            if (puzzleState[j] == MoveablePiece)
                                continue;

                            if (in_row[j] && puzzleState[j] < puzzleState[i])
                                ++conflicts;
                        }
                    }
                }
            }

            return 2 * conflicts;
        }

        private double Heuristic(Node node)
        {
            double heuristic = 0;
            if (_patternDatabase.IsInitialized)
                heuristic = _patternDatabase.Heuristic(node.PuzzleState);
            if (heuristic == 0)
                heuristic = ManhattenDistance(node.PuzzleState) + LinearConflicts(node.PuzzleState);
            return heuristic;
        }

        private (double t, bool found) Search(ICollection<Node> path, double g, double bound)
        {
            Node node = path.Last();
            double f = g + Heuristic(node);
            if (f > bound)
                return (f, false);

            if (CheckCompletion(node.PuzzleState))
                return (f, true);

            Console.WriteLine($"Explored: {++_nodesExplored:D10}, Distance: {node.Distance}, Length: {node.Length}, Value: {node.Value}");

            double min = int.MaxValue;
            foreach (Node successor in GetPossibleNodes(node))
            {
                if (successor.IsEndingNode)
                    return (-1, true);

                if (path.Any(item => item.PuzzleState.SequenceEqual(successor.PuzzleState)))
                    continue;

                path.Add(successor);
                (double t, bool found) = Search(path, g + 1, bound);
                if (found)
                    return (-1, true); // FOUND
                if (t < min)
                    min = t;
                path.Remove(successor);
            }
            return (min, false);
        }
    }
}
