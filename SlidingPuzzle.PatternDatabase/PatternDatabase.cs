using System.Text.Json;

namespace SlidingPuzzle.PatternDatabase
{
    public sealed class PatternDatabase
    {
        private readonly int _puzzleSize;
        private readonly int[] _completePuzzleState;

        public List<int[]> Groups { get; private set; }
        public List<PatternBoard> Patterns { get; private set; }
        public bool IsInitialized { get; private set; }

        public PatternDatabase(int puzzleSize)
        {
            if (puzzleSize != 4)
                throw new ArgumentOutOfRangeException(nameof(puzzleSize), "Puzzle size must be 4 (for now).");

            _puzzleSize = puzzleSize;

            // Create complete puzzle state.
            _completePuzzleState = new int[puzzleSize * puzzleSize];
            for (int i = 0; i < _puzzleSize; i++)
                _completePuzzleState[i] = i;

            Groups = new List<int[]>()
            {
                // 663
                //new int[] {0,4,5,8,9,12},
                //new int[] {6,7,10,11,13,14},
                //new int[] {1,2,3},

                // 555
                new int[] {0,1,2,3,6},
                new int[] {4,5,8,9,12},
                new int[] {7,10,11,13,14}
            };

            Patterns = new List<PatternBoard>(Groups.Count);
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                if (!TryGetSolutionDirectoryInfo(out DirectoryInfo directoryInfo))
                    throw new Exception("Solution folder not found!");

                string path = Path.Combine(directoryInfo.FullName, $"PatternDatabaseN{_puzzleSize}_string.json");
                Console.WriteLine(path);

                if (File.Exists(path))
                {
                    Patterns = ReadFromJsonFile<List<PatternBoard>>(path);
                    IsInitialized = true;
                    return;
                }

                List<Task<PatternBoard>> tasks = new();
                foreach (int[] pattern in Groups)
                    tasks.Add(Task.Run(() => BuildPatternDatabase(pattern)));

                Patterns = (await Task.WhenAll(tasks)).ToList();

                WriteToJsonFile(path, Patterns);
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            static bool TryGetSolutionDirectoryInfo(out DirectoryInfo directoryInfo)
            {
                directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                DirectoryInfo? tempDirectoryInfo = new(Directory.GetCurrentDirectory());
                while (tempDirectoryInfo != null && !tempDirectoryInfo.GetFiles("*.sln").Any())
                    tempDirectoryInfo = tempDirectoryInfo.Parent;

                if (tempDirectoryInfo == null)
                    return false;

                directoryInfo = tempDirectoryInfo;
                return true;
            }
        }

        private PatternBoard BuildPatternDatabase(int[] group)
        {
            PatternBoard board = new PatternBoard(_puzzleSize, group);
            board.GeneratePermutations();
            return board;
        }

        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter? writer = null;
            try
            {
                string contentsToWriteToFile = JsonSerializer.Serialize(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader? reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonSerializer.Deserialize<T>(fileContents)!;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public int Heuristic(int[] puzzleState)
        {
            int heuristic = 0;
            foreach (PatternBoard pattern in Patterns)
            {
                string hashed = pattern.Hash(puzzleState);
                if (pattern.ClosedSet.TryGetValue(hashed, out int h))
                        heuristic += h;
            }
            return heuristic;
        }
    }
}