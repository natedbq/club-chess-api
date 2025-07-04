# src
Setting up database
https://stackoverflow.com/questions/1933134/add-iis-7-apppool-identities-as-sql-server-logons
Will have to add appPool as an identity
	- make sure to assign sysadmin role
	- map to chess database
	- can just copy the normal user login to be sure it works as intended



from chatgpt
class Program
{
    static async Task Main()
    {
        var engine = new StockfishEngine();
        engine.Start(@"C:\path\to\stockfish.exe");

        string fen = "r1bqkbnr/pppppppp/n7/8/8/5N2/PPPPPPPP/RNBQKB1R w KQkq - 2 2";
        string result = await engine.EvaluatePosition(fen, depth: 15);
        Console.WriteLine("Stockfish output: " + result);

        engine.Stop();
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

class StockfishEngine
{
    private Process engineProcess;
    private StreamWriter engineInput;
    private StreamReader engineOutput;

    public void Start(string pathToStockfish)
    {
        engineProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = pathToStockfish,
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

    public async Task<string> EvaluatePosition(string fen, int depth = 15)
    {
        engineInput.WriteLine("uci");
        await WaitForOutput("uciok");

        engineInput.WriteLine($"position fen {fen}");
        engineInput.WriteLine($"go depth {depth}");

        string bestMove = "";
        while (true)
        {
            var line = await engineOutput.ReadLineAsync();
            if (line.Contains("bestmove"))
            {
                bestMove = line;
                break;
            }
        }

        return bestMove;
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