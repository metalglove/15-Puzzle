namespace SlidingPuzzle.PatternDatabase
{
    public struct PatternNode
    {
        public string PatternState;
        public string ClosedPatternState;
        public int HeuristicValue;

        private int[] _patternState;
        private readonly Direction direction;
        private readonly bool isMovable;

        private const int PuzzleSize = 4;
        private const int MovablePiece = 15;

        public PatternNode(int[] puzzleState, int heuristicValue, Direction direction)
        {
            _patternState = puzzleState;
            PatternState = Hash(_patternState);
            ClosedPatternState = PatternState.Remove(PatternState.Length - 2, 2);
            HeuristicValue = heuristicValue;
            this.direction = direction;
            isMovable = true;
        }

        public List<PatternNode> GenerateChildNodes(int[] pattern)
        {
            List<PatternNode> possibleNodes = new();
            switch (direction)
            {
                case Direction.Left:
                    possibleNodes = new List<PatternNode>() { Left(pattern), Up(pattern), Down(pattern) };
                    break;
                case Direction.Right:
                    possibleNodes = new List<PatternNode>() { Right(pattern), Up(pattern), Down(pattern) };
                    break;
                case Direction.Up:
                    possibleNodes = new List<PatternNode>() { Left(pattern), Right(pattern), Up(pattern) };
                    break;
                case Direction.Down:
                    possibleNodes = new List<PatternNode>() { Left(pattern), Right(pattern), Down(pattern) };
                    break;
                case Direction.None:
                    possibleNodes = new List<PatternNode>() { Left(pattern), Right(pattern), Up(pattern), Down(pattern) };
                    break;
                default:
                    break;
            }
            possibleNodes.RemoveAll(node => !node.isMovable);

            // clean up
            _patternState = Array.Empty<int>();

            return possibleNodes;
        }

        private PatternNode Left(int[] pattern)
        {
            PatternNode leftOfNode = new PatternNode();
            int[] currentPatternState = new int[_patternState.Length];
            _patternState.CopyTo(currentPatternState, 0);

            int movablePieceLocation = Array.IndexOf(currentPatternState, MovablePiece);
            int leftOfMovablePieceLocation = movablePieceLocation - 1;

            if (!IsOutOfBounds(movablePieceLocation, Direction.Left))
            {
                int heuristic = SwapLocation(currentPatternState, movablePieceLocation, leftOfMovablePieceLocation, pattern);
                leftOfNode = new PatternNode(currentPatternState, heuristic + HeuristicValue, Direction.Left);
            }

            return leftOfNode;
        }

        private PatternNode Right(int[] pattern)
        {
            PatternNode rightOfNode = new PatternNode();
            int[] currentPatternState = new int[_patternState.Length];
            _patternState.CopyTo(currentPatternState, 0);

            int movablePieceLocation = Array.IndexOf(currentPatternState, MovablePiece);
            int rightOfMovablePieceLocation = movablePieceLocation + 1;

            if (!IsOutOfBounds(movablePieceLocation, Direction.Right))
            {
                int heuristic = SwapLocation(currentPatternState, movablePieceLocation, rightOfMovablePieceLocation, pattern);
                rightOfNode = new PatternNode(currentPatternState, heuristic + HeuristicValue, Direction.Right);
            }

            return rightOfNode;
        }

        private PatternNode Down(int[] pattern)
        {
            PatternNode downOfNode = new PatternNode();
            int[] currentPatternState = new int[_patternState.Length];
            _patternState.CopyTo(currentPatternState, 0);

            int movablePieceLocation = Array.IndexOf(currentPatternState, MovablePiece);
            int downOfMovablePieceLocation = movablePieceLocation + PuzzleSize;

            if (!IsOutOfBounds(movablePieceLocation, Direction.Down))
            {
                int heuristic = SwapLocation(currentPatternState, movablePieceLocation, downOfMovablePieceLocation, pattern);
                downOfNode = new PatternNode(currentPatternState, heuristic + HeuristicValue, Direction.Down);
            }

            return downOfNode;
        }

        private PatternNode Up(int[] pattern)
        {
            PatternNode upOfNode = new PatternNode();
            int[] currentPatternState = new int[_patternState.Length];
            _patternState.CopyTo(currentPatternState, 0);

            int movablePieceLocation = Array.IndexOf(currentPatternState, MovablePiece);
            int upOfMovablePieceLocation = movablePieceLocation - PuzzleSize;

            if (!IsOutOfBounds(movablePieceLocation, Direction.Up))
            {
                int heuristic = SwapLocation(currentPatternState, movablePieceLocation, upOfMovablePieceLocation, pattern);
                upOfNode = new PatternNode(currentPatternState, heuristic + HeuristicValue, Direction.Up);
            }

            return upOfNode;
        }

        //private int PatternManhattenDistance()
        //{
        //    int Distance = 0;
        //    for (int currentNumberInList = 0; currentNumberInList < _patternState.Length; currentNumberInList++)
        //    {
        //        int CurrentNumber = _patternState[currentNumberInList];

        //        // Only numbers from the pattern are included in the cost
        //        if (CurrentNumber == -1 || CurrentNumber == MovablePiece)
        //            continue;

        //        if (currentNumberInList != CurrentNumber)
        //        {
        //            Distance += Math.Abs(currentNumberInList % PuzzleSize - CurrentNumber % PuzzleSize) + Math.Abs(currentNumberInList / PuzzleSize - CurrentNumber / PuzzleSize);
        //        }
        //    }
        //    return Distance;
        //}

        private static int SwapLocation(int[] CurrentPatternState, int movablePieceLocation, int LRUDMovablePieceLocation, int[] pattern)
        {
            int swapValue1 = CurrentPatternState[movablePieceLocation];
            int swapValue2 = CurrentPatternState[LRUDMovablePieceLocation];
            CurrentPatternState[movablePieceLocation] = swapValue2;
            CurrentPatternState[LRUDMovablePieceLocation] = swapValue1;

            if (pattern.Contains(swapValue2))
                return 1;
            return 0;
        }
        private static bool IsOutOfBounds(int currentPosition, Direction direction)
        {
            int column = currentPosition % PuzzleSize;
            int row = (int)Math.Floor((double)currentPosition / PuzzleSize);
            switch (direction)
            {
                case Direction.Left:
                    column -= 1;
                    break;
                case Direction.Right:
                    column += 1;
                    break;
                case Direction.Up:
                    row -= 1;
                    break;
                case Direction.Down:
                    row += 1;
                    break;
                default:
                    break;
            }
            return column < 0 || column >= PuzzleSize || row < 0 || row >= PuzzleSize;
        }
        public static string Hash(int[] patternState)
        {
            SortedList<int, string> list = new SortedList<int, string>();
            for (int i = 0; i < patternState.Length; i++)
            {
                int num = patternState[i];
                if (num != -1 || num == MovablePiece)
                    list.Add(num, $"{i:D2}");
            }
            return string.Join("", list.Values);
        }
    }
}