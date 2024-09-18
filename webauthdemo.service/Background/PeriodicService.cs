using System.Runtime.ExceptionServices;

namespace webauthdemo.service.Background;

internal class PeriodicService(ILogger<PeriodicService> logger) : IHostedLifecycleService
{
    private static readonly EventId StartingEventId = new(1, "Starting");
    private static readonly EventId StartEventId = new(2, "Start");
    private static readonly EventId StartedEventId = new(3, "Started");
    private static readonly EventId StopEventId = new(4, "Stop");
    private static readonly EventId StoppingEventId = new(5, "Stopping");
    private static readonly EventId StoppedEventId = new(6, "Stopped");
    private static readonly EventId TimerEnterEventId = new(7, "TimerEnter");
    private static readonly EventId TimerExitEventId = new(8, "TimerExit");
    private static readonly EventId TimerQuitEventId = new(9, "TimerQuit");
    private static readonly EventId ErrorEventId = new(10, "Error");

    private readonly CancellationTokenSource _cts = new();
    private PeriodicTimer? _timer;
    private Task? _task;

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(StartEventId, "Start");
        _timer = new PeriodicTimer(
            TimeSpan.FromMilliseconds(Random.Shared.Next(2000, 3000)),
            TimeProvider.System);
        _task = Task.Run(TimerLoop, _cts.Token);
        return Task.CompletedTask;
    }

    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(StopEventId, "Stop");
        _timer?.Dispose();
        _timer = null;
        await _cts.CancelAsync();
        await _task;
        _task.Dispose();
        _task = null;
    }

    Task IHostedLifecycleService.StartedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(StartedEventId, "Started");
        return Task.CompletedTask;
    }

    Task IHostedLifecycleService.StartingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(StartingEventId, "Starting");
        return Task.CompletedTask;
    }

    Task IHostedLifecycleService.StoppedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(StoppedEventId, "Stopped");
        _cts.Dispose();
        return Task.CompletedTask;
    }

    Task IHostedLifecycleService.StoppingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(StoppingEventId, "Stopping");
        return Task.CompletedTask;
    }
    
    private async Task Main(CancellationToken cancellation)
    {
        await Task.Delay(Random.Shared.Next(50, 100), cancellation);
    }

    private async Task TimerLoop()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                ExceptionDispatchInfo edi = null;
                logger.LogInformation(TimerEnterEventId, "Timer enter");
                try
                {
                    await Main(_cts.Token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ErrorEventId, ex, "Error");
                    edi = ExceptionDispatchInfo.Capture(ex);
                }

                if (edi?.SourceException is OperationCanceledException)
                {
                    edi.Throw();
                }

                logger.LogInformation(TimerExitEventId, "Timer exit");
            }

            logger.LogInformation(TimerQuitEventId, "Timer quit");
        }
        catch (Exception ex)
        {
            logger.LogError(TimerQuitEventId, ex, "Timer quit");
            Console.WriteLine(ex);
        }
    }
}