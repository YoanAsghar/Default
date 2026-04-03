using System.IO;
using QRCoder;
using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Default.Commands
{
    public class UtilitiesCommands
    {

        public static void AddToRoot(RootCommand root)
        {
            root.Subcommands.Add(SysinfoCommand());
            root.Subcommands.Add(GeneratePasswodCommand());
            root.Subcommands.Add(GenerateQrCommand());
        }
        public static Command SysinfoCommand()
        {
            var SystemCommand = new Command("sysinfo", "System related options");

            SystemCommand.SetAction((ParseResult) =>
            {

                void PrintSection(string title)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n  {title}");
                    Console.WriteLine($"  {"─".PadRight(30, '─')}");
                    Console.ResetColor();
                }

                void PrintField(string label, object? value, ConsoleColor valueColor = ConsoleColor.Gray)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"  {label,-20}");
                    Console.ForegroundColor = valueColor;
                    Console.WriteLine(value);
                    Console.ResetColor();
                }

                // ── SYSTEM ──────────────────────────────────
                PrintSection("   SYSTEM");
                PrintField("OS:", RuntimeInformation.OSDescription);
                PrintField("Architecture:", RuntimeInformation.OSArchitecture);
                PrintField("Hostname:", Environment.MachineName);
                PrintField("Username:", Environment.UserName);
                PrintField("Domain:", Environment.UserDomainName);
                PrintField(".NET Version:", Environment.Version);
                PrintField("System Uptime:", TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\d\ hh\h\ mm\m"));

                // ── CPU ─────────────────────────────────────
                PrintSection("  CPU");
                PrintField("Processor:", Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "Unknown");
                PrintField("Cores:", Environment.ProcessorCount);

                // CPU usage — mide durante 1 segundo para obtener valor real
                var process = Process.GetCurrentProcess();
                var startCpu = process.TotalProcessorTime;
                var startTime = DateTime.UtcNow;
                Thread.Sleep(1000);
                var cpuUsed = (process.TotalProcessorTime - startCpu).TotalMilliseconds;
                var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
                var cpuUsage = cpuUsed / (elapsed * Environment.ProcessorCount) * 100;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"  {"CPU Usage:",-20}");
                Console.ForegroundColor = cpuUsage switch
                {
                    > 80 => ConsoleColor.Red,
                    > 50 => ConsoleColor.Yellow,
                    _ => ConsoleColor.Gray
                };
                Console.WriteLine($"{cpuUsage:F1}%");
                Console.ResetColor();

                // ── MEMORY ──────────────────────────────────
                PrintSection("  MEMORY");
                var workingSet = process.WorkingSet64 / (1024.0 * 1024);
                var privateBytes = process.PrivateMemorySize64 / (1024.0 * 1024);
                PrintField("Working Set:", $"{workingSet:F1} MB");
                PrintField("Private Bytes:", $"{privateBytes:F1} MB");

                // ── DRIVES ──────────────────────────────────
                PrintSection("  DRIVES");
                foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                {
                    double totalGB = drive.TotalSize / (1024.0 * 1024 * 1024);
                    double freeGB = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
                    double usedPct = (1 - freeGB / totalGB) * 100;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"  {drive.Name,-20}");
                    Console.ForegroundColor = usedPct switch
                    {
                        > 90 => ConsoleColor.Red,
                        > 70 => ConsoleColor.Yellow,
                        _ => ConsoleColor.Gray
                    };
                    Console.WriteLine($"{freeGB:F1} GB free / {totalGB:F1} GB total ({usedPct:F0}% used) [{drive.DriveType}]");
                    Console.ResetColor();
                }

                // ── ENVIRONMENT ─────────────────────────────
                PrintSection("  ENVIRONMENT");
                PrintField("Current Dir:", Environment.CurrentDirectory);
                PrintField("System Dir:", Environment.SystemDirectory);
                PrintField("Temp Dir:", Path.GetTempPath());
                PrintField("Date & Time:", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                PrintField("Timezone:", TimeZoneInfo.Local.DisplayName);

                Console.WriteLine();
            });

            //Add the subcommands
            // SystemCommand.Add();

            return SystemCommand;
        }

        public static Command GeneratePasswodCommand()
        {
            var GeneratePassword = new Command("password");

            //Arguments
            var Length = new Argument<int>("length");
            var IncludeCaps = new Option<bool>("--caps");
            var IncludeLows = new Option<bool>("--lows");
            var IncludeNums = new Option<bool>("--nums");
            var IncludeSymbols = new Option<bool>("--symbols");

            //Add arguments
            GeneratePassword.Arguments.Add(Length);
            GeneratePassword.Options.Add(IncludeLows);
            GeneratePassword.Options.Add(IncludeCaps);
            GeneratePassword.Options.Add(IncludeNums);
            GeneratePassword.Options.Add(IncludeSymbols);

            GeneratePassword.SetAction((ParseResult) =>
            {
                int lenght = ParseResult.GetValue(Length);
                bool includeLows = ParseResult.GetValue(IncludeLows);
                bool includeCaps = ParseResult.GetValue(IncludeCaps);
                bool includeNums = ParseResult.GetValue(IncludeNums);
                bool includeSymbols = ParseResult.GetValue(IncludeSymbols);

                const string lowercase = "abcdefghijklmnopqrstuvwxyz";
                const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                const string numbers = "0123456789";
                const string symbols = "!@#$%^&*()-_+=[]{}|;:,.?/~";

                string CharactersPool = "";
                if (includeLows == false && includeCaps == false && includeNums == false && includeSymbols == false)
                {
                    includeLows = true;
                    includeCaps = true;
                    includeNums = true;
                    includeSymbols = true;
                }
                if (includeLows) CharactersPool += lowercase;
                if (includeCaps) CharactersPool += uppercase;
                if (includeNums) CharactersPool += numbers;
                if (includeSymbols) CharactersPool += symbols;

                var random = new Random();
                string RandomPassword = new string(
                    Enumerable.Range(0, lenght)
                    .Select(_ => CharactersPool[random.Next(CharactersPool.Length)])
                    .ToArray()
                    );
                Console.WriteLine(RandomPassword);
            });

            return GeneratePassword;
        }

        public static Command GenerateQrCommand()
        {
            var GenerateQrCommand = new Command("qrcode", "Creates a new qr referencing the text that you introduce");
            var TextoToConvertInQr = new Argument<string>("text");
            GenerateQrCommand.Arguments.Add(TextoToConvertInQr);
            GenerateQrCommand.SetAction((ParseResult) =>
            {
                string PathToSaveQr = Path.Combine(Environment.CurrentDirectory, "qrcode.png");
                using var qrGenerator = new QRCodeGenerator();
                var TextToQr = ParseResult.GetValue(TextoToConvertInQr);
                if (string.IsNullOrEmpty(TextToQr))
                {
                    Console.WriteLine("Provide a text to convert");
                    return;
                }

                QRCodeData qrCodeData = qrGenerator.CreateQrCode(TextToQr, QRCodeGenerator.ECCLevel.Q);
                using var pngRenderer = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = pngRenderer.GetGraphic(20); //
                File.WriteAllBytes(PathToSaveQr, qrCodeBytes);
                Console.WriteLine($"QR saved at: {PathToSaveQr}");
            });


            return GenerateQrCommand;
        }
    }
}
