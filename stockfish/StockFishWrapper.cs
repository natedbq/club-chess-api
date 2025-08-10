using chess.api.models;
using System.Diagnostics;
using System.Numerics;

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

    public async Task<Evaluation> EngineMoves(string fen, int depth = 15)
    {
        engineInput.WriteLine("uci");
        await WaitForOutput("uciok");

        var white = fen.Split(" ")[1] == "w";

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
            if (line.StartsWith("bestmove"))
            {
                break;
            }
            if (line.StartsWith($"info depth {depth}") && (line.Contains(" cp ") || line.Contains(" mate ")))
            {
                var multipv = line.Substring(line.IndexOf("multipv ")).Split(" ")[1];
                if (line.Contains(" cp "))
                {
                    eval = line.Substring(line.IndexOf("cp ")).Split(" ")[1];
                    if (!lines.ContainsKey(multipv))
                    {
                        lines.Add(multipv, line);
                        eval += line + "\n";
                    }
                    if (lines.Count() >= 4)
                    {
                        break;
                    }
                }
                else if(line.Contains(" mate "))
                {
                    eval = line.Substring(line.IndexOf("mate ")).Split(" ")[1];
                    if (!lines.ContainsKey(multipv))
                    {
                        lines.Add(multipv, line);
                        eval += line + "\n";
                    }
                    if (lines.Count() >= 4)
                    {
                        break;
                    }
                }
            }
        }
            
        watch.Stop();

        var evaluation = new Evaluation()
        {
            FEN = fen,
            KNodes = 1,
            Depth = depth,
            Pvs = new List<PV>(),
        };

        foreach(var lineKey in lines.Keys)
        {//"info depth 20 seldepth 40 multipv 1 score cp 18 nodes 1619172 nps 847734 hashfull 501 tbhits 0 time 1910 pv e7e5 e2e4 g8f6 f1b5 f6e4 e1g1 e4g5 a3c4 e5e4 f3g5 h6g5 d2d4 d7d5 c4e5 c8d7 b5c6 b7c6 f2f3 f8d6 e5d7 d8d7 c1g5"
            var line = lines[lineKey];
            if(line.Contains(" cp "))
            {
                var cp = line.Substring(line.IndexOf("cp ")).Split(" ")[1];
                var pvNum = line.Substring(line.IndexOf("multipv ")).Split(" ")[1];
                var moves = line.Substring(line.IndexOf(" pv ") + 4);
                PV pv = new PV()
                {
                    Index = int.Parse(pvNum),
                    Eval = cp,
                    Moves = moves
                };
                evaluation.Pvs.Add(pv);
            }
            else if(line.Contains(" mate "))
            {
                var mate = line.Substring(line.IndexOf("mate ")).Split(" ")[1];
                var pvNum = line.Substring(line.IndexOf("multipv ")).Split(" ")[1];
                var moves = line.Substring(line.IndexOf(" pv ") + 4);
                PV pv = new PV()
                {
                    Index = int.Parse(pvNum),
                    Eval = "m"+mate,
                    Moves = moves
                };
                evaluation.Pvs.Add(pv);
            }
        }

        if (white)
        {
        }
        else
        {
            evaluation.Pvs = evaluation.Pvs.OrderByDescending(pv => pv.Index).ToList();
        }

        return evaluation;
    }

    public async Task<SinglePointEval> EvaluatePosition(string fen, int depth = 15)
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
            else if (line.StartsWith($"info depth {depth}") && line.Contains(" mate "))
            {
                eval = "m"+line.Substring(line.IndexOf("mate ")).Split(" ")[1];
                break;
            }
        }

        return new SinglePointEval()
        {
            Value = eval
        };
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