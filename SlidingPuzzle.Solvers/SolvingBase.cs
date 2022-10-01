using SlidingPuzzle.PatternDatabase;

namespace SlidingPuzzle.Solvers
{
    public abstract class SolvingBase
    {
        private readonly int[] PuzzleEndState;
        protected readonly int MoveablePiece;
        protected readonly int PuzzleSize;

        public Node StartingNode { get; }
        public Node EndingNode { get; private set; }

        protected SolvingBase(int[] startingState, int puzzleSize)
        {
            PuzzleSize = puzzleSize;
            StartingNode = new Node()
            {
                PuzzleState = startingState,
                Direction = Direction.None,
                Value = ManhattenDistance(startingState)
            };
            PuzzleEndState = CalculateEndState(PuzzleSize);
            MoveablePiece = PuzzleEndState.Last();
        }

        #region Moves
        public Node Left(Node fromNode)
        {
            Node leftOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Left, Length = fromNode.Length + 1 };
            int[] currentPuzzleState = new int[fromNode.PuzzleState.Length];
            fromNode.PuzzleState.CopyTo(currentPuzzleState, 0);

            int movablePieceLocation = Array.IndexOf(currentPuzzleState, MoveablePiece);
            int leftOfMovablePieceLocation = movablePieceLocation - 1;

            if (!IsOutOfBounds(movablePieceLocation, Direction.Left))
            {
                SwapLocation(currentPuzzleState, movablePieceLocation, leftOfMovablePieceLocation);
                leftOfNode = SetNodeInfo(leftOfNode, currentPuzzleState);
            }

            if (CheckCompletion(currentPuzzleState))
            {
                EndingNode = leftOfNode;
                EndingNode.IsEndingNode = true;
            }
            return leftOfNode;
        }
        public Node Right(Node fromNode)
        {
            Node rightOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Right, Length = fromNode.Length + 1 };
            int[] currentPuzzleState = new int[fromNode.PuzzleState.Length];
            fromNode.PuzzleState.CopyTo(currentPuzzleState, 0);

            int movablePieceLocation = Array.IndexOf(currentPuzzleState, MoveablePiece);
            int rightOfMovablePieceLocation = movablePieceLocation + 1;

            if (!IsOutOfBounds(movablePieceLocation, Direction.Right))
            {
                SwapLocation(currentPuzzleState, movablePieceLocation, rightOfMovablePieceLocation);
                rightOfNode = SetNodeInfo(rightOfNode, currentPuzzleState);
            }

            if (CheckCompletion(currentPuzzleState))
            {
                EndingNode = rightOfNode;
                EndingNode.IsEndingNode = true;
            }
            return rightOfNode;
        }
        public Node Up(Node fromNode)
        {
            Node upOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Up, Length = fromNode.Length + 1 };
            int[] currentPuzzleState = new int[fromNode.PuzzleState.Length];
            fromNode.PuzzleState.CopyTo(currentPuzzleState, 0);

            int movablePieceLocation = Array.IndexOf(currentPuzzleState, MoveablePiece);
            int upOfMovablePieceLocation = movablePieceLocation - PuzzleSize;

            if (!IsOutOfBounds(movablePieceLocation, Direction.Up))
            {
                SwapLocation(currentPuzzleState, movablePieceLocation, upOfMovablePieceLocation);
                upOfNode = SetNodeInfo(upOfNode, currentPuzzleState);
            }

            if (CheckCompletion(currentPuzzleState))
            {
                EndingNode = upOfNode;
                EndingNode.IsEndingNode = true;
            }
            return upOfNode;
        }
        public Node Down(Node fromNode)
        {
            Node downOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Down, Length = fromNode.Length + 1 };
            int[] currentPuzzleState = new int[fromNode.PuzzleState.Length];
            fromNode.PuzzleState.CopyTo(currentPuzzleState, 0);

            int movablePieceLocation = Array.IndexOf(currentPuzzleState, MoveablePiece);
            int DownOfWhitePuzzlePieceLocation = movablePieceLocation + PuzzleSize;

            if (!IsOutOfBounds(movablePieceLocation, Direction.Down))
            {
                SwapLocation(currentPuzzleState, movablePieceLocation, DownOfWhitePuzzlePieceLocation);
                downOfNode = SetNodeInfo(downOfNode, currentPuzzleState);
            }

            if (CheckCompletion(currentPuzzleState))
            {
                EndingNode = downOfNode;
                EndingNode.IsEndingNode = true;
            }
            return downOfNode;
        }
        #endregion Moves

        public List<Direction> FindPath()
        {
            List<Direction> moves = new List<Direction>();
            Search();
            Node node = EndingNode;
            while (node.ParentNode != null)
            {
                moves.Add(node.Direction);
                node = node.ParentNode;
            }
            moves.Reverse();
            return moves;
        }

        protected List<Node> GetPossibleNodes(Node fromNode)
        {
            List<Node> possibleNodes = new List<Node>();
            switch (fromNode.Direction)
            {
                case Direction.Left:
                    possibleNodes = new List<Node>() { Left(fromNode), Up(fromNode), Down(fromNode) };
                    break;
                case Direction.Right:
                    possibleNodes = new List<Node>() { Right(fromNode), Up(fromNode), Down(fromNode) };
                    break;
                case Direction.Up:
                    possibleNodes = new List<Node>() { Left(fromNode), Right(fromNode), Up(fromNode) };
                    break;
                case Direction.Down:
                    possibleNodes = new List<Node>() { Left(fromNode), Right(fromNode), Down(fromNode) };
                    break;
                case Direction.None:
                    possibleNodes = new List<Node>() { Left(fromNode), Right(fromNode), Up(fromNode), Down(fromNode) };
                    break;
                default:
                    break;
            }
            possibleNodes.RemoveAll(node => node.IsMoveable == false);
            return possibleNodes;
        }
        protected bool CheckCompletion(int[] list)
        {
            return list.SequenceEqual(PuzzleEndState);
        }
        protected double ManhattenDistance(int[] currentPuzzleState)
        {
            double distance = 0;
            for (int currentNumberInList = 0; currentNumberInList < currentPuzzleState.Length; currentNumberInList++)
            {
                int CurrentNumber = currentPuzzleState[currentNumberInList];
                if (currentNumberInList != CurrentNumber)
                {
                    distance += (Math.Abs((currentNumberInList % PuzzleSize) - (CurrentNumber % PuzzleSize)) + Math.Abs((currentNumberInList / PuzzleSize) - (CurrentNumber / PuzzleSize)));
                }
            }
            return distance;
        }
        protected abstract void Search();

        private Node SetNodeInfo(Node currentNode, int[] currentPuzzleState)
        {
            currentNode.Distance = ManhattenDistance(currentPuzzleState);
            currentNode.Value = currentNode.Distance + currentNode.Length;
            currentNode.PuzzleState = currentPuzzleState;
            currentNode.IsMoveable = true;
            return currentNode;
        }
        private static void SwapLocation(int[] currentPuzzleState, int movablePieceLocation, int lrudMovablePieceLocation)
        {
            int swapValue1 = currentPuzzleState[movablePieceLocation];
            int swapValue2 = currentPuzzleState[lrudMovablePieceLocation];
            currentPuzzleState[movablePieceLocation] = swapValue2;
            currentPuzzleState[lrudMovablePieceLocation] = swapValue1;
        }
        private bool IsOutOfBounds(int currentPosition, Direction direction)
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
        private static int[] CalculateEndState(int puzzleSize)
        {
            int[] NewPuzzleEndState = new int[puzzleSize * puzzleSize];
            for (int i = 0; i < (puzzleSize * puzzleSize); i++)
            {
                NewPuzzleEndState[i] = i;
            }
            return NewPuzzleEndState;
        }
    }
}
