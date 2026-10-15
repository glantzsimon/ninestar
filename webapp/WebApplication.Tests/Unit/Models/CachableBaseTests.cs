using K9.WebApplication.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Models
{
    public class CachableBaseTests
    {
        private class CacheProbe : CachableBase
        {
            public T Get<T>(string key, Func<T> fetch) => GetOrAddToCache(key, fetch);
            public void Remove(string key) => RemoveFromCache(key);
        }

        [Fact]
        public void ConcurrentInstancesShareOneCalculationForTheSameKey()
        {
            var key = Guid.NewGuid().ToString("N");
            var probes = Enumerable.Range(0, 6).Select(_ => new CacheProbe()).ToArray();
            var calls = 0;
            var result = new object();
            using (var ready = new CountdownEvent(probes.Length))
            using (var start = new ManualResetEventSlim())
            using (var entered = new ManualResetEventSlim())
            using (var release = new ManualResetEventSlim())
            {
                var tasks = probes.Select(probe => Task.Factory.StartNew(() =>
                {
                    ready.Signal();
                    if (!start.Wait(TimeSpan.FromSeconds(10)))
                        throw new TimeoutException();
                    return probe.Get(key, () =>
                    {
                        Interlocked.Increment(ref calls);
                        entered.Set();
                        if (!release.Wait(TimeSpan.FromSeconds(10)))
                            throw new TimeoutException();
                        return result;
                    });
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default)).ToArray();
                try
                {
                    Assert.True(ready.Wait(TimeSpan.FromSeconds(10)));
                    start.Set();
                    Assert.True(entered.Wait(TimeSpan.FromSeconds(10)));
                }
                finally
                {
                    start.Set();
                    release.Set();
                }
                try
                {
                    Assert.True(Task.WaitAll(tasks, TimeSpan.FromSeconds(10)));
                    Assert.Equal(1, calls);
                    Assert.All(tasks, task => Assert.Same(result, task.Result));
                    Assert.Same(result, probes[0].Get<object>(key, () => { throw new Exception("Cache missed."); }));
                }
                finally
                {
                    probes[0].Remove(key);
                }
            }
        }

        [Fact]
        public void DifferentKeysDoNotWaitForEachOther()
        {
            var probe = new CacheProbe();
            var firstKey = Guid.NewGuid().ToString("N");
            var secondKey = Guid.NewGuid().ToString("N");
            using (var entered = new ManualResetEventSlim())
            using (var release = new ManualResetEventSlim())
            {
                var first = Task.Factory.StartNew(() => probe.Get(firstKey, () =>
                {
                    entered.Set();
                    if (!release.Wait(TimeSpan.FromSeconds(10)))
                        throw new TimeoutException();
                    return 1;
                }), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                Task<int> second = null;
                try
                {
                    Assert.True(entered.Wait(TimeSpan.FromSeconds(10)));
                    second = Task.Factory.StartNew(() => probe.Get(secondKey, () => 2),
                        CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    Assert.True(second.Wait(TimeSpan.FromSeconds(5)));
                    Assert.Equal(2, second.Result);
                }
                finally
                {
                    release.Set();
                    first.Wait(TimeSpan.FromSeconds(10));
                    second?.Wait(TimeSpan.FromSeconds(10));
                    probe.Remove(firstKey);
                    probe.Remove(secondKey);
                }
            }
        }

        [Fact]
        public void FailedCalculationsCanBeRetried()
        {
            var probe = new CacheProbe();
            var key = Guid.NewGuid().ToString("N");
            try
            {
                Assert.Throws<InvalidOperationException>(() =>
                    probe.Get<int>(key, () => { throw new InvalidOperationException(); }));
                Assert.Equal(7, probe.Get(key, () => 7));
            }
            finally
            {
                probe.Remove(key);
            }
        }

        [Fact]
        public void CalculationCanReuseAnotherCachedKey()
        {
            var probe = new CacheProbe();
            var parentKey = Guid.NewGuid().ToString("N");
            var childKey = Guid.NewGuid().ToString("N");
            try
            {
                Assert.Equal(8, probe.Get(parentKey, () => probe.Get(childKey, () => 7) + 1));
            }
            finally
            {
                probe.Remove(parentKey);
                probe.Remove(childKey);
            }
        }
    }
}
