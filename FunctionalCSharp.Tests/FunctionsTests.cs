using System;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

using static FunctionalCSharp.Tests.Helpers;

namespace FunctionalCSharp.Tests
{
    public class FunctionsTests {

        [Property]
        public Property curryF2Test() {
            return Prop.ForAll<Func<int, string, string>, int, string>(
                (forig, i, s) => forig(i, s) == forig.curry()(i)(s));
        }

        [Property]
        public Property curryF3Test() {
            return Prop.ForAll<Func<int?, string, double, string>, Tuple<int?, string, double>>((forig, t) => {
                var (a, b, c) = t;
                var f = forig.curry()(a);
                return forig(a, b, c) == f(b)(c);
            });
        }

        [Property]
        public Property curryF4Test() {
            return Prop.ForAll<Tuple<double, DateTime, bool, int>>(t => {
                var (a, b, c, d) = t;
                var fc = F4.curry()(a);
                return F4(a, b, c, d) == fc(b)(c)(d);
            });
        }

        [Property]
        public Property curryF5Test() {
            return Prop.ForAll<Tuple<byte, double, DateTime, bool, int>>(t => {
                var (a, b, c, d, e) = t;
                var fc = F5.curry()(a)(b)(c);
                return F5(a, b, c, d, e) == fc(d)(e);
            });
        }

        [Property]
        public Property uncurryF2Test() {
            return Prop.ForAll<Func<int, string, string>, int, string>((f, a, b) => {
                var fnew = f.curry().uncurry();
                return f(a, b) == fnew(a, b);
            });
        }

        [Property]
        public Property uncurryF3Test() {
            return Prop.ForAll<Func<int, string, double, string>, Tuple<int, string, double>>((f, t) => {
                var (a, b, c) = t;
                var fnew = f.curry().uncurry();
                return f(a, b, c) == fnew(a, b, c);
            });
        }

        [Property]
        public Property uncurryF4Test() {
            return Prop.ForAll<Tuple<double, DateTime, bool, int>>(t => {
                var (a, b, c, d) = t;
                var fnew = F4.curry().uncurry();
                return F4(a, b, c, d) == fnew(a, b, c, d);
            });
        }

        [Property]
        public Property uncurryF5Test() {
            return Prop.ForAll<Tuple<byte, double, DateTime, bool, int>>(t => {
                var (a, b, c, d, e) = t;
                var fnew = F5.curry().uncurry();
                return F5(a, b, c, d, e) == fnew(a, b, c, d, e);
            });
        }

        [Property]
        public Property pApply1_F2Test() {
            return Prop.ForAll<Func<double, bool, int>, double, bool>((f, a, b) => {
                var fap = f.pApply1(a);
                return f(a, b) == fap(b);
            });
        }

        [Property]
        public Property pApply2_F2Test() {
            return Prop.ForAll<Func<double, bool, int>, double, bool>((f, a, b) => {
                var fap = f.pApply2(b);
                return f(a, b) == fap(a);
            });
        }

        [Property]
        public Property pApply1_F3Test() {
            return Prop.ForAll<Func<int, byte, DateTime, bool>, Tuple<int, byte, DateTime>>((f, t) => {
                var (a, b, c) = t;
                var fap = f.pApply1(a);
                return f(a, b, c) == fap(b, c);
            });
        }

        [Property]
        public Property pApply2_F3Test() {
            return Prop.ForAll<Func<int, byte, DateTime, bool>, Tuple<int, byte, DateTime>>((f, t) => {
                var (a, b, c) = t;
                var fap = f.pApply2(b);
                return f(a, b, c) == fap(a, c);
            });
        }

        [Property]
        public Property pApply3_F3Test() {
            return Prop.ForAll<Func<int, byte, DateTime, bool>, Tuple<int, byte, DateTime>>((f, t) => {
                var (a, b, c) = t;
                var fap = f.pApply3(c);
                return f(a, b, c) == fap(a, b);
            });
        }

        [Property]
        public Property pApplyMultiple_F3Test() {
            return Prop.ForAll<Func<int, byte, DateTime, bool>, Tuple<int, byte, DateTime>>((f, t) => {
                var (a, b, c) = t;
                var fap = f.pApply3(c).pApply1(a);
                return f(a, b, c) == fap(b);
            });
        }

        [Property]
        public Property pApply1_F4Test() {
            return Prop.ForAll<Tuple<double, DateTime, bool, int>>(t => {
                var (a, b, c, d) = t;
                var fap = F4.pApply1(a);
                return F4(a, b, c, d) == fap(b, c, d);
            });
        }

        [Property]
        public Property pApply2_F4Test() {
            return Prop.ForAll<Tuple<double, DateTime, bool, int>>(t => {
                var (a, b, c, d) = t;
                var fap = F4.pApply2(b);
                return F4(a, b, c, d) == fap(a, c, d);
            });
        }

        [Property]
        public Property pApply3_F4Test() {
            return Prop.ForAll<Tuple<double, DateTime, bool, int>>(t => {
                var (a, b, c, d) = t;
                var fap = F4.pApply3(c);
                return F4(a, b, c, d) == fap(a, b, d);
            });
        }

        [Property]
        public Property pApply4_F4Test() {
            return Prop.ForAll<Tuple<double, DateTime, bool, int>>(t => {
                var (a, b, c, d) = t;
                var fap = F4.pApply4(d);
                return F4(a, b, c, d) == fap(a, b, c);
            });
        }

        [Property]
        public Property pApplyMultiple_F4Test() {
            return Prop.ForAll<Tuple<double, DateTime, bool, int>>(t => {
                var (a, b, c, d) = t;
                var fap = F4.pApply1(a).pApply1(b).pApply1(c);
                return F4(a, b, c, d) == fap(d);
            });
        }

        [Property]
        public Property pApply1_F5Test() {
            return Prop.ForAll<Tuple<byte, double, DateTime, bool, int>>(t => {
                var (a, b, c, d, e) = t;
                var fap = F5.pApply1(a);
                return F5(a, b, c, d, e) == fap(b, c, d, e);
            });
        }

        [Property]
        public Property pApply2_F5Test() {
            return Prop.ForAll<Tuple<byte, double, DateTime, bool, int>>(t => {
                var (a, b, c, d, e) = t;
                var fap = F5.pApply2(b);
                return F5(a, b, c, d, e) == fap(a, c, d, e);
            });
        }

        [Property]
        public Property pApply3_F5Test() {
            return Prop.ForAll<Tuple<byte, double, DateTime, bool, int>>(t => {
                var (a, b, c, d, e) = t;
                var fap = F5.pApply3(c);
                return F5(a, b, c, d, e) == fap(a, b, d, e);
            });
        }

        [Property]
        public Property pApply4_F5Test() {
            return Prop.ForAll<Tuple<byte, double, DateTime, bool, int>>(t => {
                var (a, b, c, d, e) = t;
                var fap = F5.pApply4(d);
                return F5(a, b, c, d, e) == fap(a, b, c, e);
            });
        }

        [Property]
        public Property pApply5_F5Test() {
            return Prop.ForAll<Tuple<byte, double, DateTime, bool, int>>(t => {
                var (a, b, c, d, e) = t;
                var fap = F5.pApply5(e);
                return F5(a, b, c, d, e) == fap(a, b, c, d);
            });
        }

        [Property]
        public Property pApplyMultiple_F5Test() {
            return Prop.ForAll<Tuple<byte, double, DateTime, bool, int>>(t => {
                var (a, b, c, d, e) = t;
                var fap = F5.pApply3(c).pApply4(e).pApply1(a).pApply1(b);
                return F5(a, b, c, d, e) == fap(d);
            });
        }

        [Property]
        public Property swapTest() {
            return Prop.ForAll<Func<int, string, string>, int, string>((forig, i, s) => {
                var fswap = forig.swap();
                return forig(i, s) == fswap(s, i);
            });
        }

        [Property]
        public Property composeTest() {
            return Prop.ForAll<Func<string, int>, Func<double, string>, double>((f, g, d) => {
                var h = f.compose(g);
                var s = g(d);
                var i = f(s);
                return i == h(d);
            });
        }

        [Property]
        public Property andThenTest() {
            return Prop.ForAll<Func<string, int>, Func<double, string>, double>((f, g, d) => {
                var h = g.andThen(f);
                var s = g(d);
                var i = f(s);
                return i == h(d);
            });
        }

        [Property]
        public Property identityIntTest() {
            return Prop.ForAll<int>(a => Functions.identity(a) == a);
        }

        [Property]
        public Property identityStrTest() {
            return Prop.ForAll<string>(a => Functions.identity(a) == a);
        }

        [Property]
        public Property identityDblTest() {
            // double.NaN == double.NaN = false.
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Prop.ForAll<NormalFloat>(a => Functions.identity(a.Get) == a.Get);
        }

        /// <summary>
        /// Test using currying for a method. 
        /// The method needs to get assigned to a Func delegate first.
        /// </summary>
        [Fact]
        public void methodCurryTest()
        {
            Func<int, int, int, int> f = sum;
            var result = f.curry()(1)(2)(3);
            Assert.Equal(6, result);
        }

        /// <summary>
        /// Test using partial application for a method. 
        /// The method needs to get assigned to a Func delegate first.
        /// </summary>
        [Fact]
        public void methodPartialApplyTest() {
            Func<int, int, int, int> f = sum;
            var o = 1.some();
            var o2 = o.map(f.pApply3(2).pApply2(5));
            Assert.Equal(8, o2.value);
        }

        public int sum(int a, int b, int c) { return a + b + c; }
    }
}
