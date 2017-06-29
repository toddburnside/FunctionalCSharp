using System;
using Xunit;

namespace FunctionalCSharp.Tests
{
    internal static class Helpers {
        internal static void assertSome<T>(T expected, Option<T> o)
        {
            Assert.True(o.hasValue);
            Assert.Equal(expected, o.value);
        }

        internal static void assertNone<T>(Option<T> o)
        {
            Assert.False(o.hasValue);
            var exc = Record.Exception(() => o.value);
            Assert.IsType<Exception>(exc);
        }

        internal static void assertLeft<E, A>(Either<E, A> e, E left) {
            Assert.False(e.isRight);
            Assert.Equal(left, e.getLeft);
            var exc = Record.Exception(() => e.getRight);
            Assert.IsType<Exception>(exc);
        }

        internal static void assertRight<E, A>(Either<E, A> e, A right)
        {
            Assert.True(e.isRight);
            Assert.Equal(right, e.getRight);
            var exc = Record.Exception(() => e.getLeft);
            Assert.IsType<Exception>(exc);
        }

        // FsCheck doesn't generate functions with more than 3 parameters.
        public static Func<double, DateTime, bool, int, string> F4 = (a, b, c, d) => $"{a} {b} {c} {d}";
        public static Func<byte, double, DateTime, bool, int, string> F5 = (a, b, c, d, e) => $"{a} {b} {c} {d} {e}";
    }
}
