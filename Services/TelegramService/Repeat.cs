///https://stackoverflow.com/questions/7472013/how-to-create-a-thread-task-with-a-continuous-loop
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TransmissionRemoteBot.Services.Telegram
{
    internal static class Repeat
    {
        public static Task Interval(
            TimeSpan pollInterval,
            Action action,
            CancellationToken token)
        {
            // We don't use Observable.Interval:
            // If we block, the values start bunching up behind each other.
            return Task.Factory.StartNew(
                () =>
                {
                    for (; ; )
                    {
                        action();

                        if (token.WaitCancellationRequested(pollInterval))
                            break;
                    }
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }

    static class CancellationTokenExtensions
    {
        public static bool WaitCancellationRequested(
            this CancellationToken token,
            TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }
}
