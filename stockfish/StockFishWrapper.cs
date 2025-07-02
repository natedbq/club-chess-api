using System.Diagnostics;

class StockfishWrapper
{
    private Process engineProcess;
    private StreamWriter engineInput;
    private StreamReader engineOutput;

    public void Start()
    {
        engineProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "D:\\projects\\chess-game\\stockfish\\stockfish.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        engineProcess.Start();
        engineInput = engineProcess.StandardInput;
        engineOutput = engineProcess.StandardOutput;
    }

    public async Task<Dictionary<string, string>> EngineMoves(string fen, int depth = 15)
    {
        engineInput.WriteLine("uci");
        await WaitForOutput("uciok");

        engineInput.WriteLine("setoption name MultiPV value 4");
        engineInput.WriteLine($"position fen {fen}");
        engineInput.WriteLine($"go depth {depth}");

        Stopwatch watch = Stopwatch.StartNew();
        watch.Start();

        string eval = "";
        var lines = new Dictionary<string, string>();
        while (true)
        {
            var line = await engineOutput.ReadLineAsync();
            if (line.StartsWith($"info depth {depth}") && line.Contains(" cp "))
            {
                var multipv = line.Substring(line.IndexOf("multipv ")).Split(" ")[1];
                eval = line.Substring(line.IndexOf("cp ")).Split(" ")[1];
                if (!lines.ContainsKey(multipv))
                {
                    lines.Add(multipv, line);
                    eval += line + "\n";
                }
                if(lines.Count() >= 4)
                {
                    break;
                }
            }
        }
            
        watch.Stop();

        return lines;
    }

    public async Task<string> EvaluatePosition(string fen, int depth = 15)
    {
        engineInput.WriteLine("uci");
        await WaitForOutput("uciok");

        engineInput.WriteLine("setoption name MultiPV value 1");
        engineInput.WriteLine($"position fen {fen}");
        engineInput.WriteLine($"go depth {depth}");

        string eval = "";
        while (true)
        {
            var line = await engineOutput.ReadLineAsync();
            if (line.StartsWith($"info depth {depth}") && line.Contains(" cp "))
            {
                eval = line.Substring(line.IndexOf("cp ")).Split(" ")[1];
                break;
            }
        }

        return eval;
    }

    private async Task WaitForOutput(string expected)
    {
        string line;
        while ((line = await engineOutput.ReadLineAsync()) != null)
        {
            if (line.Contains(expected))
                break;
        }
    }

    public void Stop()
    {
        engineInput.WriteLine("quit");
        engineProcess.Close();
    }
}