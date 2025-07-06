
namespace SimpleRateLimiterNS
{
    public interface ISimpleRateLimiter
    {
        void Dispose();
        ValueTask WaitAsync();
        ValueTask WaitAsync(TimeSpan timeOut);
    }
}