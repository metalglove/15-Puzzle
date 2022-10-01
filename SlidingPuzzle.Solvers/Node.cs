using SlidingPuzzle.PatternDatabase;

namespace SlidingPuzzle.Solvers
{
    public class Node
    {
        public int[] PuzzleState { get; set; }
        public int Length { get; set; }
        public double Distance { get; set; }
        public double Value { get; set; }
        public Direction Direction { get; set; }
        public Node ParentNode { get; set; }
        public bool IsMoveable { get; set; }
        public bool IsEndingNode { get; set; }
    }
}
