namespace SlidingPuzzle.Solvers
{
    public class AStar : SolvingBase
    {
        public AStar(int[] startingState, int puzzleSize) : base(startingState, puzzleSize)
        {

        }

        protected override void Search()
        {
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();

            openList.Add(StartingNode);
            while (EndingNode == null)
            {
                double LowestValue = openList.Min(item => item.Value);
                Node BestValueNode = openList.First(node => node.Value.Equals(LowestValue));
                openList.Remove(BestValueNode);
                closedList.Add(BestValueNode);
                foreach (Node possibleNode in GetPossibleNodes(BestValueNode))
                {
                    if (!openList.Exists(item => item.PuzzleState.SequenceEqual(possibleNode.PuzzleState)))
                    {
                        if (!closedList.Exists(item => item.PuzzleState.SequenceEqual(possibleNode.PuzzleState)))
                        {
                            openList.Add(possibleNode);
                        }
                    }
                }
                Console.WriteLine("openlist = " + openList.Count + "| closedlist = " + closedList.Count + "| bestvaluenode " + BestValueNode.Value);
            }
        }
    }
}
