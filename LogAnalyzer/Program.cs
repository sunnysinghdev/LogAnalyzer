namespace LogAnalyzer
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid arguments: Folder path missing.");
                return;
            }
            string directoryPath = args[0];
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine($"Watching logs in {directoryPath} ...");

            // Analyze logs
            var analyzer =  new LogAnalyzer(directoryPath);
            analyzer.StartAnalyzing();


            Console.WriteLine("Press 'q' to quit the sample.");
            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                // Keep running until q is pressed
            }

        }


    }
    public class LogAnalyzer
    {
        private readonly string _folderPath;
        public LogAnalyzer(string folderPath)
        {
            _folderPath = folderPath;
        }
        public void StartAnalyzing()
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = _folderPath;
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.Filter = "*.txt";
            watcher.Created += OnFileCreated;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
        }
        private async void OnFileCreated(object source, FileSystemEventArgs e)
        {
            var counter = await ReadCounters(e.FullPath);
            Console.WriteLine($"File {e.ChangeType}: {Path.GetFileNameWithoutExtension(e.FullPath)}");
            Console.WriteLine($"{nameof(counter.Warning)}:{counter.Warning}, " +
                $"{nameof(counter.Error)}: {counter.Error}, " +
                $"{nameof(counter.Fail)}: {counter.Fail}");

        }
        private static async Task<Counter> ReadCounters(string filePath)
        {
            var counter = new Counter();
            var lines = await File.ReadAllLinesAsync(filePath);
            foreach (var line in lines)
            {
                if(line.Contains(nameof(counter.Warning), StringComparison.OrdinalIgnoreCase))
                    counter.Warning++;
                else if (line.Contains(nameof(counter.Error), StringComparison.OrdinalIgnoreCase))
                    counter.Error++;
                else if (line.Contains(nameof(counter.Fail), StringComparison.OrdinalIgnoreCase))
                    counter.Fail++;
            }
            return counter;
        }


    }
    public class Counter
    {
        public int Warning { get; set; }
        public int Error { get; set; }
        public int Fail { get; set; }
    }
}
