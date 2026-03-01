// ┌─────────────────────────────────────────────────────────────────────────┐
// │  CancellationToken Demo                                                 │
// │                                                                         │
// │  Simulates 20 steps of slow work (500 ms each ≈ 10 s total).           │
// │  A background listener watches for Enter and calls cts.Cancel().       │
// │                                                                         │
// │  VERSION 1 (current):                                                   │
// │    Task.Delay does NOT receive the token.                               │
// │    Pressing Enter signals cancellation, but nobody checks it.          │
// │    The loop always runs all 20 steps.                                   │
// │                                                                         │
// │  VERSION 2 (one-line change):                                           │
// │    Pass cts.Token to Task.Delay.                                        │
// │    Pressing Enter throws OperationCanceledException immediately.        │
// │    The loop stops at whatever step it was on.                           │
// └─────────────────────────────────────────────────────────────────────────┘

using var cts = new CancellationTokenSource();

// Background task: block on ReadLine, then cancel.
_ = Task.Run(() =>
{
    Console.ReadLine();
    Console.WriteLine("\n  → Cancellation requested.");
    cts.Cancel();
});

Console.WriteLine("20 steps × 500 ms = ~10 s of work.");
Console.WriteLine("Press ENTER at any time to cancel.\n");

var timer = System.Diagnostics.Stopwatch.StartNew();

try
{
    for (var i = 1; i <= 20; i++)
    {
        // ── VERSION 1 ──────────────────────────────────────────────────────
        // Token not passed. Cancellation is requested but never observed.
        // The loop runs all 20 steps regardless of Enter.
        await Task.Delay(500);

        // ── VERSION 2 ──────────────────────────────────────────────────────
        // Swap the line above for this one. That is the only change needed.
        // Task.Delay checks the token on every tick and throws when cancelled.
        // await Task.Delay(500, cts.Token);
        // ───────────────────────────────────────────────────────────────────

        Console.WriteLine($"  step {i,2}/20   elapsed: {timer.Elapsed.TotalSeconds,5:F1}s");
    }

    Console.WriteLine("\n  All 20 steps completed.");
}
catch (OperationCanceledException)
{
    Console.WriteLine("\n  OperationCanceledException caught — processing stopped.");
    Console.WriteLine("  Only the steps that finished before cancellation ran.");
}

Console.WriteLine($"\n  Total time: {timer.Elapsed.TotalSeconds:F1}s");
