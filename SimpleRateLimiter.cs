namespace SimpleRateLimiterNS;

public class SimpleRateLimiter : IDisposable
{
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly TimeSpan _slideTimeWindowSec;
    private readonly TimeSpan _minDelayPerRequest = TimeSpan.Zero;

    public SimpleRateLimiter(int maxCalls, TimeSpan slideTimeWindowSec)
    {
        _semaphoreSlim = new SemaphoreSlim(maxCalls, maxCalls);
        _slideTimeWindowSec = slideTimeWindowSec;
    }

    public SimpleRateLimiter(int maxCalls, TimeSpan slideTimeWindowSec, TimeSpan minDelayPerRequest)
    {
        _semaphoreSlim = new SemaphoreSlim(maxCalls, maxCalls);
        _slideTimeWindowSec = slideTimeWindowSec;
        _minDelayPerRequest = minDelayPerRequest;
    }

    public void Dispose()
    {
        _semaphoreSlim?.Dispose();
        GC.SuppressFinalize(this);
    }

    ~SimpleRateLimiter()
    {
        Dispose();
    }

    public async ValueTask WaitAsync(TimeSpan timeOut)
    {
        if (_minDelayPerRequest > TimeSpan.Zero)
        {
            await Task.Delay(_minDelayPerRequest).ConfigureAwait(false);
        }

        await _semaphoreSlim.WaitAsync(timeOut).ConfigureAwait(false);
        _ = TimedRelease();
    }

    public async ValueTask WaitAsync()
    {
        if (_minDelayPerRequest > TimeSpan.Zero)
        {
            await Task.Delay(_minDelayPerRequest).ConfigureAwait(false);
        }

        await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
        _ = TimedRelease();
    }

    private async ValueTask TimedRelease()
    {
        await Task.Delay(_slideTimeWindowSec).ConfigureAwait(false);
        _semaphoreSlim?.Release();
    }
}