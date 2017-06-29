using System;
using Xunit;

namespace FunctionalCSharp.Tests
{
    public class EitherTests
    {
        [Fact]
        public void tempTest() {
//            var e = Either<string, int>.right(1);
            var e = 1.right<string, int>();
            Assert.Equal(1, e.getRight);
        }

        [Fact]
        public void catchAllTest() {
            var e = toInt.catchAll()("123");
            Assert.Equal(123, e.getRight);

            var e2 = toInt.catchAll()("Fail");
            Assert.False(e2.isRight);
            Assert.IsType<FormatException>(e2.getLeft);
        }

        [Fact]
        public void catchOnlyFailTest() {
            var f = toInt.catchOnly<string, int, NullReferenceException>();
            var exc = Record.Exception(() => f("xyz"));
            Assert.NotNull(exc);
            Assert.IsType<FormatException>(exc);
        }

        [Fact]
        public void catchAllSuccessTest() {
            var e = toInt.catchOnly<string, int, FormatException>()("yada");
            Assert.False(e.isRight);
            Assert.IsType<FormatException>(e.getLeft);
        }

        // TODO: Tests for lift, liftEither, map2, getOrElse, fold, swap, etc.

        private readonly Func<string, int> toInt = s => int.Parse(s);
    }
}
