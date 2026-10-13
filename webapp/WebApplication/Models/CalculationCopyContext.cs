using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace K9.WebApplication.Models
{
    internal sealed class CalculationCopyContext
    {
        private sealed class ReferenceComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }

        private readonly Dictionary<object, object> _copies =
            new Dictionary<object, object>(new ReferenceComparer());

        internal void UseCopy<T>(T original, T copy) where T : class
        {
            if (original != null) _copies[original] = copy;
        }

        internal T Copy<T>(T source, Func<T, T> shallowCopy, Action<T, T> copyChildren) where T : class
        {
            if (source == null) return null;
            if (_copies.TryGetValue(source, out var existing)) return (T)existing;
            var copy = shallowCopy(source);
            _copies.Add(source, copy);
            copyChildren(source, copy);
            return copy;
        }
    }
}
