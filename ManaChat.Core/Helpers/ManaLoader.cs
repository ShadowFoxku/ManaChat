using ManaFox.Core.ConsoleTools;

namespace ManaChat.Core.Helpers
{
    public static class ManaLoader
    {
        const int BoxWidth = 48;
        static int LinesSinceSpinner = 0;
        static CancellationTokenSource? SpinnerCts;
        static Task? SpinnerTask;
        static string DisplayText = "";
        static readonly string[] SpinnerFrames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
        static string HighlightColour = ConsoleConstants.White;
        static LineCountingWriter? CountingWriter;

        public static void ShowLaunching()
        {
            HighlightColour = ConsoleConstants.BrightMagenta;
            DisplayText = "Starting up...";
            StartProcess("Launching ", "ManaChat", " instance...");
        }

        public static void ShowMigrating(string dbName)
        {
            HighlightColour = ConsoleConstants.Red;
            DisplayText = "Migrating...";
            StartProcess("Starting ", dbName, " migration...");
        }

        private static void StartProcess(string before, string highlight, string after, string subtitle = "")
        {
            ManaConsole.Init();
            PrintBox(before, highlight, after, subtitle);
            if (!ManaConsole.IsTTY)
            {
                Console.WriteLine($"  {DisplayText}");
                return;
            }

            if (CountingWriter == null)
                CountingWriter = new LineCountingWriter(Console.Out);

            Console.SetOut(CountingWriter);
            Console.WriteLine($"  {ConsoleConstants.Cyan}{SpinnerFrames[0]}{ConsoleConstants.Reset}  {DisplayText}");
            LinesSinceSpinner = 0;

            SpinnerCts = new CancellationTokenSource();
            SpinnerTask = RunSpinnerAsync(SpinnerCts.Token);
        }

        public static void ShowReady(string subtitle)
        {
            EndProcess("Your ", "ManaChat", " instance is ready!", "[✓] Loaded!", subtitle);
        }

        public static void ShowMigrationComplete(string dbName)
        {
            EndProcess("", dbName, " migrated!", "[✓] Migrated!");
        }

        public static void ShowMigrationFailed(string dbName)
        {
            EndProcess("", dbName, " migration failed.", "[X] Something went wrong");
        }

        private static void EndProcess(string before, string highlight, string after, string completeText, string subtitle = "")
        {
            if (!ManaConsole.IsTTY)
            {
                Console.WriteLine($"  {before}{highlight}{after}");
                return;
            }

            SpinnerCts?.Cancel();
            SpinnerTask?.Wait();

            int jumpUp = LinesSinceSpinner + 1;
            Console.Write($"{ConsoleConstants.CursorUp(jumpUp)}\r{ConsoleConstants.ClearLine}");
            Console.WriteLine($"  {ConsoleConstants.BrightGreen}{completeText}{ConsoleConstants.Reset}");

            if (CountingWriter != null)
            {
                Console.SetOut(CountingWriter.Inner);
                CountingWriter = null;
            }

            if (LinesSinceSpinner > 0)
                Console.Write($"{ConsoleConstants.CursorDown(LinesSinceSpinner - 1)}");

            PrintBox(before, highlight, after, subtitle);
            Console.Write(ConsoleConstants.ShowCursor);
            Console.WriteLine();
        }

        public static void ShowInstanceNamed(string instanceName)
        {
            if (!ManaConsole.IsTTY)
            {
                Console.WriteLine($"  Instance Identified as: {instanceName}");
                return;
            }

            PrintBox("Welcome to the ", "ManaChat", " network,", instanceName);
        }

        private static void PrintBox(string before, string highlight = "", string after = "", string subtitle = "")
        {
            int innerWidth = BoxWidth - 2;
            string fullText = before + highlight + after;
            int totalPad = innerWidth - fullText.Length;
            int leftPad = Math.Max(0, totalPad / 2);
            int rightPad = Math.Max(0, totalPad - leftPad);

            Console.WriteLine();
            Console.WriteLine($"  {ConsoleConstants.Cyan}┌{new string('─', innerWidth)}┐{ConsoleConstants.Reset}");
            PrintBoxLine($"{ConsoleConstants.BrightWhite}{before}{ConsoleConstants.Reset}" +
                $"{HighlightColour}{highlight}{ConsoleConstants.Reset}" +
                $"{ConsoleConstants.BrightWhite}{after}{ConsoleConstants.Reset}", leftPad, rightPad);

            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                int tPad = innerWidth - subtitle.Length;
                int lPad = Math.Max(0, tPad / 2);
                int rPad = Math.Max(0, tPad - lPad);
                PrintBoxLine($"{ConsoleConstants.BrightCyan}{ConsoleConstants.Bold}{ConsoleConstants.Italic}{subtitle}{ConsoleConstants.Reset}", lPad, rPad);
            }

            Console.WriteLine($"  {ConsoleConstants.Cyan}└{new string('─', innerWidth)}┘{ConsoleConstants.Reset}");
            Console.WriteLine();
        }

        private static void PrintBoxLine(string text, int leftPad, int rightPad)
        {
            Console.Write($"  {ConsoleConstants.Cyan}│{ConsoleConstants.Reset}{new string(' ', leftPad)}");
            Console.Write(text);
            Console.WriteLine($"{new string(' ', rightPad)}{ConsoleConstants.Cyan}│{ConsoleConstants.Reset}");
        }

        // we make our spinners gamer in this house
        private static int R = 0;
        private static int G = 0;
        private static int B = 0;
        private static double T = 0;
        private static async Task RunSpinnerAsync(CancellationToken ct)
        {
            Console.Write(ConsoleConstants.HideCursor);
            int i = 0;

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await Task.Delay(80, ct);
                    i++;
                    UpdateColours();
                    int down = LinesSinceSpinner + 1;
                    Console.Write($"{ConsoleConstants.CursorUp(down)}\r{ConsoleConstants.ClearLine}");
                    Console.Write($"  [{ConsoleConstants.FgRgb(R, G, B)}{SpinnerFrames[i % SpinnerFrames.Length]}{ConsoleConstants.Reset}]  {DisplayText}");
                    if (down > 0)
                        Console.Write($"{ConsoleConstants.CursorDown(down)}");
                    Console.Write($"\r");
                }
            }
            catch (TaskCanceledException) { }
        }

        private static void UpdateColours()
        {
            T += 0.075;
            R = (int)((Math.Sin(T) + 1) * 127.5);
            G = (int)((Math.Sin(T + (2 * Math.PI / 3)) + 1) * 127.5);
            B = (int)((Math.Sin(T + (4 * Math.PI / 3)) + 1) * 127.5);
        }

        private class LineCountingWriter(TextWriter inner) : TextWriter
        {
            public readonly TextWriter Inner = inner;
            public override System.Text.Encoding Encoding => Inner.Encoding;

            public override void Write(char value)
            {
                if (value == '\n') Interlocked.Increment(ref LinesSinceSpinner);
                Inner.Write(value);
            }

            public override void Write(string? value)
            {
                if (value != null)
                    foreach (var c in value)
                        if (c == '\n') Interlocked.Increment(ref LinesSinceSpinner);
                Inner.Write(value);
            }

            public override void WriteLine(string? value)
            {
                if (value != null)
                    foreach (var c in value)
                        if (c == '\n') Interlocked.Increment(ref LinesSinceSpinner);
                Interlocked.Increment(ref LinesSinceSpinner);
                Inner.WriteLine(value);
            }

            public override void WriteLine()
            {
                Interlocked.Increment(ref LinesSinceSpinner);
                Inner.WriteLine();
            }

            public override void Flush() => Inner.Flush();
        }
    }
}
