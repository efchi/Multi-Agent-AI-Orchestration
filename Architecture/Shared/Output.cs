using System.Collections.Concurrent;

namespace Agents.Architecture.Shared
{
    public static class Output
    {
        #region Constants

        public const ConsoleColor DefaultForegroundColor = ConsoleColor.White;
        public const ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;

        #endregion

        #region Background Output Logic

        class OutputItem
        {
            public string Text { get; set; } = string.Empty;
            public ConsoleColor ForegroundColor { get; set; }
            public ConsoleColor BackgroundColor { get; set; }
        }

        class ExitSignal : OutputItem { }

        static ConcurrentQueue<OutputItem> Queue { get; } = new();
        static EventWaitHandle WriteWaitHandle { get; } = new(false, EventResetMode.AutoReset);
        static EventWaitHandle? ExitWaitHandle { get; set; }
        
        public static void Initialize(EventWaitHandle? exitWaitHandle = default) =>
            ExitWaitHandle = exitWaitHandle;

        static Output()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    WriteWaitHandle.WaitOne();

                    while (Queue.TryDequeue(out var item))
                    {
                        BackgroundWrite(item.Text, item.ForegroundColor, item.BackgroundColor);

                        if (ExitWaitHandle != default && item is ExitSignal)
                        {
                            // Signal the main thread that the Output thread
                            // has written all the output and is ready to terminate.
                            ExitWaitHandle.Set();
                            return;
                        }
                    }
                }
            })
            { IsBackground = true };

            thread.Start();
        }

        static void BackgroundWrite(string text, ConsoleColor foregroundColor = ConsoleColor.Gray, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            var defaultForeground = Console.ForegroundColor;
            var defaultBackground = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(text);
            Console.ForegroundColor = defaultForeground;
            Console.BackgroundColor = defaultBackground;
        }

        #endregion

        #region Background Output Methods

        public static void WriteLine(ConsoleColor? foregroundColor = default, ConsoleColor? backgroundColor = default) =>
            WriteLine(string.Empty, foregroundColor, backgroundColor);

        static readonly object LockObj = new();
        public static void WriteLine(string text, ConsoleColor? foregroundColor = default, ConsoleColor? backgroundColor = default)
        {
            lock (LockObj)
            {
                text = text.Replace(Environment.NewLine, string.Empty);
                Write(text, foregroundColor, backgroundColor);
                Write(Environment.NewLine, DefaultForegroundColor, DefaultBackgroundColor);
            }
        }

        public static void Write(string text, ConsoleColor? foregroundColor = default, ConsoleColor? backgroundColor = default)
        {
            foregroundColor ??= DefaultForegroundColor;
            backgroundColor ??= DefaultBackgroundColor;

            Queue.Enqueue(new OutputItem
            {
                Text = text,
                ForegroundColor = foregroundColor.Value,
                BackgroundColor = backgroundColor.Value,
            });
            WriteWaitHandle.Set();
        }

        internal static void SignalExit()
        {
            Queue.Enqueue(new ExitSignal());
            WriteWaitHandle.Set();
        }

        #endregion

        #region Custom Output Methods

        public static void WriteInfoLine(string text) =>
            WriteLine(text, ConsoleColor.White);

        public static void WriteDebugLine(string text) =>
            WriteLine(text, ConsoleColor.DarkGray);

        public static void WriteEventLine(string text) =>
            WriteLine(text, ConsoleColor.Cyan);

        public static void WriteResponseLine(string text) =>
            WriteLine(text, ConsoleColor.Green);

        public static void WriteErrorLine(string text) =>
            WriteLine(text, ConsoleColor.Red);

        public static void WriteWarningLine(string text) =>
            WriteLine(text, ConsoleColor.Yellow);

        #endregion
    }
}
