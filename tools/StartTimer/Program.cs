using System.Diagnostics;
using StartTimer;

if (args.Length == 0)
{
    Console.WriteLine("Missing required argument [path]");
    return 1;
}

var target = args[0];

var targetArgs = "";
var sepIndex = Array.IndexOf(args, "--");
if (sepIndex >= 0)
{
    var startFrom = sepIndex + 1;
    targetArgs = string.Join(" ", args[startFrom..]);
}

TimeSpan started = default;
TimeSpan firstOutput = default;
TimeSpan readyToServe = default ;
TimeSpan afterFirstRequest = default;
var listeningOnUrl = "";
var pid = -1;
var sendRequestTask = Task.CompletedTask;

var sw = Stopwatch.StartNew();

try
{
    var result = await ProcessUtil.RunAsync(target, targetArgs, Environment.CurrentDirectory, onStart: OnStart, outputDataReceived: OnOutput, errorDataReceived: OnError);
}
catch (Exception ex) when (pid == -1)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message.ToString());
    Console.ResetColor();
    return 1;
}
catch (Exception)
{ }

await sendRequestTask;

Console.WriteLine();
Console.WriteLine($"Target executable '{target}' ran successfully");
Console.WriteLine($"Started after {started.TotalMilliseconds:N2} ms");
if (firstOutput != default)
{
    Console.WriteLine($"First output after {firstOutput.TotalMilliseconds:N2} ms");
}
if (readyToServe != default)
{
    Console.WriteLine($"Ready to accept requests after {readyToServe.TotalMilliseconds:N2} ms");
}
if (afterFirstRequest != default)
{
    Console.WriteLine($"First request completed after {afterFirstRequest.TotalMilliseconds:N2} ms");
}

return 0;

void OnStart(int processId)
{
    started = sw.Elapsed;
    pid = processId;
}

void OnOutput(string output)
{
    if (firstOutput == default)
    {
        firstOutput = sw.Elapsed;
    }

    if (output.Contains("Now listening on: http://"))
    {
        var schemeIndex = output.IndexOf("http://");
        listeningOnUrl = output[schemeIndex..];
    }
    if (output.Contains("Application started. Press Ctrl+C to shut down."))
    {
        // App is ready to start
        readyToServe = sw.Elapsed;

        // Send request
        sendRequestTask = SendRequestAndStopProcess();
    }
    Console.WriteLine(output);
}

void OnError(string error)
{
    if (firstOutput == default)
    {
        firstOutput = sw.Elapsed;
    }

    Console.WriteLine(error);
}

async Task SendRequestAndStopProcess()
{
    using var httpClient = new HttpClient();
    var response = await httpClient.GetAsync(listeningOnUrl);
    
    afterFirstRequest = sw.Elapsed;
    ProcessUtil.KillProcess(pid);
}