using System;
using System.Collections.Generic;
using Xunit;

namespace FunctionalCSharp.Tests
{
    public class ValidatedTests
    {
        [Fact]
        public void testTupleSyntax1() {
            var v1 = 1.valid<string, int>();
            var v2 = 2.valid(); // this creates a Validated<string, int>
            var result = (v1, v2).map((i, j) => i + j);
            assertValid(3, result);
        }

        [Fact]
        public void testTupleSyntax2() {
            var v1 = "Error".invalid<int>();
            var v2 = 2.valid();
            var result = (v1, v2).map((i, j) => i + j);
            assertInvalid(new List<string>{"Error"}, result);
        }

        [Fact]
        public void testTupleSyntax3() {
            var v1 = "Error".invalid<int>();
            var v2 = "Error2".invalid<int>();
            var result = (v1, v2).map((i, j) => i + j);
            assertInvalid(new List<string>{"Error", "Error2"}, result);
        }

        // ReSharper disable UnusedParameter.Local
        private static void assertValid<E, A>(A expected, Validated<E, A> v) {
            Assert.True(v.isValid);
            Assert.Equal(expected, v.getValid);
        }

        private static void assertInvalid<E, A>(List<E> expected, Validated<E, A> v) {
            Assert.False(v.isValid);
            Assert.Equal(expected, v.getInvalid);
        }
    }
}
