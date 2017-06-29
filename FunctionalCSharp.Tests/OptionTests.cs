using System;
using System.Collections.Generic;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

using static FunctionalCSharp.Tests.Helpers;

// ReSharper disable ExpressionIsAlwaysNull
namespace FunctionalCSharp.Tests
{
    public class OptionTests
    {
        [Property]
        public Property testSomeInt() {
            return Prop.ForAll<int>(a => assertSome(a, Option<int>.some(a)));
        }

        [Property]
        public Property testSomeString() {
            // nulls are preserved with some()
            return Prop.ForAll<string>(a => assertSome(a, Option<string>.some(a)));
        }

        [Property]
        public Property testSomeNullable()
        {
            // nulls are preserved with some()
            return Prop.ForAll<int?>(a => assertSome(a, Option<int?>.some(a)));
        }

        [Property]
        public Property testSomeIntHelper() {
            return Prop.ForAll<int>(a => assertSome(a, a.some()));
        }

        [Fact]
        public void testNoneIntHelper() {
            assertNone(1.none());
        }

        [Fact]
        public void testNoneStringHelper()
        {
            string s = null;
            assertNone(s.none());
            s = "whatever";
            assertNone(s.none());
        }

        [Property]
        public Property testSomeDoubleHelper() {
            return Prop.ForAll<double>(a => assertSome(a, a.some()));
        }

        [Fact]
        public void testNoneNullableHelper() {
            decimal? d = null;
            assertNone(d.none());
            d = 5.5m;
            assertNone(d.none());
        }

        [Property]
        public Property testToOptionStringNotNull() {
            return Prop.ForAll<StringNoNulls>(a => assertSome(a, a.toOption()));
        }

        [Fact]
        public void testToOptionStringNull() {
            string s = null;
            var o = s.toOption();
            assertNone(o);
        }

        // for some reason, tests wouldn't run with double.minValue and maxValue.
        [Theory]
        [InlineData(99999999.99)]
        [InlineData(-0.01)]
        [InlineData(0.0)]
        [InlineData(0.01)]
        [InlineData(-99999999.99)]
        public void testToOptionNullableNotNull(double? value) {
            var o = value.toOption();
            // ReSharper disable once PossibleInvalidOperationException
            assertSome(value.Value, o);
        }

        [Fact]
        public void testToOptionNullableNull() {
            decimal? s = null;
            var o = s.toOption();
            assertNone(o);
        }

        [Property]
        public Property testGetOrElseIntNone() {
            var o = Option<int>.none();
            return Prop.ForAll<int>(a => a == o.getOrElse(a));
        }

        [Property]
        public Property testGetOrElseIntNoneLazy() {
            var o = Option<int>.none();
            return Prop.ForAll<int>(a => a == o.getOrElse(() => a));
        }

        [Property]
        public Property testGetOrElseIntNoneLazy2() {
            var o = Option<int>.none();
            return Prop.ForAll<Func<int>>(f => f() == o.getOrElse(f));
        }

        [Property]
        public Property testGetOrElseIntSome() {
            return Prop.ForAll<int, int>((a, b) => a == a.some().getOrElse(b));
        }

        [Property]
        public Property testGetOrElseStringNone() {
            var o = Option<string>.none();
            return Prop.ForAll<string>(a => a == o.getOrElse(a));
        }

        [Property]
        public Property testGetOrElseStringNoneLazy() {
            var o = Option<string>.none();
            return Prop.ForAll<Func<string>>(f => f() == o.getOrElse(f));
        }

        [Property]
        public Property testGetOrElseStringSome() {
            // only non-null strings. A null string would create a none.
            return Prop.ForAll<NonNull<string>, string>((a, b) => a.Get == a.Get.toOption().getOrElse(b));
        }

        [Fact]
        public void testGetOrElseStringNull() {
            var o = Option<string>.some(null);
            // odd behavior for nulls - because it is a some(null)
            Assert.Null(o.getOrElse("nothing"));
        }

        [Property]
        public Property testGetOrElseNullableNone() {
            var o = Option<decimal?>.none();
            return Prop.ForAll<decimal?>(a => a == o.getOrElse(a));
        }

        [Property]
        public Property testGetOrElseNullableNoneLazy()
        {
            var o = Option<DateTime?>.none();
            return Prop.ForAll<Func<DateTime?>>(f => f() == o.getOrElse(f));
        }

        [Property]
        public Property testGetOrElseNullableSome() {
            // Have to use Option<decimal?>.some() or else can get a none.
            return Prop.ForAll<decimal?, decimal>((a, b) => a == Option<decimal?>.some(a).getOrElse(b));
        }

        [Fact]
        public void testGetOrElseNullableNull() {
            var o = Option<decimal?>.some(null);
            // odd behavior for nulls - because it is a some(null)
            Assert.Null(o.getOrElse(5.5m));
        }

        [Fact]
        public void testMapNullClassNull() {
            var o1 = Option<string>.some(null);
            assertSome(null, o1);
            var o2 = o1.mapNull();
            assertNone(o2);
            // make sure o1 is unchanged
            assertSome(null, o1);
        }

        [Property]
        public Property testMapNullNotNull() {
            return Prop.ForAll<StringNoNulls>(a => {
                var some = a.toOption();
                assertSome(a, some);
                var mapped = some.mapNull();
                assertSome(a, mapped);
                // should be the same object
                Assert.Same(some, mapped);
            });
        }

        [Fact]
        public void testMapNullNullable() {
            var o1 = Option<int?>.some(null);
            assertSome(null, o1);
            var o2 = o1.mapNull();
            assertNone(o2);
            // o1 should be unchanged
            assertSome(null, o1);

            var o3 = Option<int?>.some(5);
            assertSome(5, o3);
            var o4 = o3.mapNull();
            assertSome(5, o4);
            // o3 unchanged
            assertSome(5, o3);
            // But, o4 should be a non-nullable type
            Assert.IsType<Option<int>>(o4);
        }

        [Property]
        public Property testMapWithSome() {
            return Prop.ForAll<Func<int, string>, int>((f, a) => {
                var oa = a.some();
                assertSome(f(a), oa.map(f));
            });
        }

        [Property]
        public Property testMapWithNone() {
            return Prop.ForAll<Func<int, string>, int>((f, a) => {
                var oa = Option<int>.none();
                assertNone(oa.map(f));
            });
        }

        [Property]
        public Property testMap2() {
            var an = Option<decimal>.none();
            var bn = Option<int>.none();
            return Prop.ForAll<Func<decimal, int, bool>, decimal, int>((f, a, b) => {
                assertSome(f(a, b), (a.some(), b.some()).map(f));
                assertNone((an, b.some()).map(f));
                assertNone((a.some(), bn).map(f));
                assertNone((an, bn).map(f));
            });
        }

        [Property]
        public Property testMap3() {
            var an = Option<string>.none();
            var bn = Option<int>.none();
            var cn = Option<bool>.none();
            return Prop.ForAll<Func<string, int, bool, decimal>, Tuple<string, int, bool>>((f, t) => {
                var (a, b, c) = t;
                // a is a reference type, so I'll wrap it in a some even if it's a null for testing purposes.
                var asome = Option<string>.some(a);
                assertSome(f(a, b, c), (asome, b.some(), c.some()).map(f));
                assertNone((an, b.some(), c.some()).map(f));
                assertNone((an, bn, c.some()).map(f));
                assertNone((an, b.some(), cn).map(f));
                assertNone((an, bn, cn).map(f));
                assertNone((asome, bn, c.some()).map(f));
                assertNone((asome, b.some(), cn).map(f));
                assertNone((asome, bn, cn).map(f));
            });
        }

        [Property]
        public Property testMap4() {
            var an = Option<double>.none();
            var bn = Option<DateTime>.none();
            var cn = Option<bool>.none();
            var dn = Option<int>.none();
            return Prop.ForAll<Tuple<double, DateTime, bool, int>>(t => {
                var (a, b, c, d) = t;
                assertSome(F4(a, b, c, d), (a.some(), b.some(), c.some(), d.some()).map(F4));
                assertNone((an, b.some(), c.some(), d.some()).map(F4));
                assertNone((an, b.some(), c.some(), dn).map(F4));
                assertNone((an, bn, c.some(), d.some()).map(F4));
                assertNone((an, bn, c.some(), dn).map(F4));
                assertNone((an, b.some(), cn, d.some()).map(F4));
                assertNone((an, bn, cn, dn).map(F4));
                assertNone((a.some(), bn, c.some(), d.some()).map(F4));
                assertNone((a.some(), b.some(), cn, dn).map(F4));
                assertNone((a.some(), bn, cn, d.some()).map(F4));
                assertNone((a.some(), bn, cn, dn).map(F4));
            });
        }

        [Property]
        public Property testLiftWithSome() {
            return Prop.ForAll<Func<int, string>, int>((f, a) => {
                var oa = a.some();
                assertSome(f(a), f.liftOption()(oa));
            });
        }

        [Property]
        public Property testLiftWithNone() {
            return Prop.ForAll<Func<int, string>, int>((f, a) => {
                var oa = Option<int>.none();
                assertNone(f.liftOption()(oa));
            });
        }

        [Property]
        public Property testLift2() {
            var an = Option<decimal>.none();
            var bn = Option<int>.none();
            return Prop.ForAll<Func<decimal, int, bool>, decimal, int>((f, a, b) => {
                var fl = f.liftOption();
                assertSome(f(a, b), fl(a.some(), b.some()));
                assertNone(fl(an, b.some()));
                assertNone(fl(a.some(), bn));
                assertNone(fl(an, bn));
            });
        }

        [Property]
        public Property testLift3() {
            var an = Option<string>.none();
            var bn = Option<int>.none();
            var cn = Option<bool>.none();
            return Prop.ForAll<Func<string, int, bool, decimal>, Tuple<string, int, bool>>((f, t) => {
                var (a, b, c) = t;
                var fl = f.liftOption();
                // a is a reference type, so I'll wrap it in a some even if it's a null for testing purposes.
                var asome = Option<string>.some(a);
                assertSome(f(a, b, c), fl(asome, b.some(), c.some()));
                assertNone(fl(an, b.some(), c.some()));
                assertNone(fl(an, bn, c.some()));
                assertNone(fl(an, b.some(), cn));
                assertNone(fl(an, bn, cn));
                assertNone(fl(asome, bn, c.some()));
                assertNone(fl(asome, b.some(), cn));
                assertNone(fl(asome, bn, cn));
            });
        }

        [Property]
        public Property testlift4() {
            var an = Option<double>.none();
            var bn = Option<DateTime>.none();
            var cn = Option<bool>.none();
            var dn = Option<int>.none();
            return Prop.ForAll<Tuple<double, DateTime, bool, int>>(t => {
                var (a, b, c, d) = t;
                var fl = F4.liftOption();
                assertSome(F4(a, b, c, d), fl(a.some(), b.some(), c.some(), d.some()));
                assertNone(fl(an, b.some(), c.some(), d.some()));
                assertNone(fl(an, b.some(), c.some(), dn));
                assertNone(fl(an, bn, c.some(), d.some()));
                assertNone(fl(an, bn, c.some(), dn));
                assertNone(fl(an, b.some(), cn, d.some()));
                assertNone(fl(an, bn, cn, dn));
                assertNone(fl(a.some(), bn, c.some(), d.some()));
                assertNone(fl(a.some(), b.some(), cn, dn));
                assertNone(fl(a.some(), bn, cn, d.some()));
                assertNone(fl(a.some(), bn, cn, dn));
            });
        }

        [Fact]
        public void testFlatMap() {
            Option<int> f(string s) {
                int i;
                if (int.TryParse(s, out i)) {
                    return i.some();
                }
                return Option<int>.none();
            }

            var o1 = Option<string>.none();
            var o2 = o1.flatMap(f);
            assertNone(o2);
            var o3 = "bad".toOption();
            var o4 = o3.flatMap(f);
            assertNone(o4);
            var o5 = "123".toOption();
            var o6 = o5.flatMap(f);
            assertSome(123, o6);
        }

        [Fact]
        public void testFlatMap2() {
            Option<string> noneFunc(int a, string b) => Option<string>.none();
            Option<string> someFunc(int a, string b) => $"{a} {b}".toOption();
            var na = Option<int>.none();
            var nb = Option<string>.none();
            var sa = 1.some();
            var sb = "string".toOption();
            assertSome("1 string", (sa, sb).flatMap(someFunc));
            assertNone((na, sb).flatMap(someFunc));
            assertNone((sa, nb).flatMap(someFunc));
            assertNone((na, nb).flatMap(someFunc));
            
            assertNone((sa, sb).flatMap(noneFunc));
            assertNone((na, sb).flatMap(noneFunc));
            assertNone((sa, nb).flatMap(noneFunc));
            assertNone((na, nb).flatMap(noneFunc));
        }

        [Fact]
        public void testFlatMap3() {
            Option<string> noneFunc(int a, string b, decimal c) => Option<string>.none();
            Option<string> someFunc(int a, string b, decimal c) => $"{a} {b} {c}".toOption();
            var na = Option<int>.none();
            var nb = Option<string>.none();
            var nc = Option<decimal>.none();
            var sa = 1.some();
            var sb = "string".toOption();
            var sc = 93.2m.some();
            assertSome("1 string 93.2", (sa, sb, sc).flatMap(someFunc));
            assertNone((na, sb, sc).flatMap(someFunc));
            assertNone((sa, nb, sc).flatMap(someFunc));
            assertNone((sa, sb, nc).flatMap(someFunc));
            assertNone((na, nb, sc).flatMap(someFunc));
            assertNone((sa, nb, nc).flatMap(someFunc));
            assertNone((na, sb, nc).flatMap(someFunc));
            assertNone((na, nb, nc).flatMap(someFunc));

            assertNone((sa, sb, sc).flatMap(noneFunc));
            assertNone((na, sb, sc).flatMap(noneFunc));
            assertNone((sa, nb, sc).flatMap(noneFunc));
            assertNone((sa, sb, nc).flatMap(noneFunc));
            assertNone((na, nb, sc).flatMap(noneFunc));
            assertNone((sa, nb, nc).flatMap(noneFunc));
            assertNone((na, sb, nc).flatMap(someFunc));
            assertNone((na, nb, nc).flatMap(someFunc));
        }

        [Fact]
        public void testFlatMap4() {
            Option<string> noneFunc(int a, string b, decimal c, byte d) => Option<string>.none();
            Option<string> someFunc(int a, string b, decimal c, byte d) => $"{a} {b} {c} {d}".toOption();
            var na = Option<int>.none();
            var nb = Option<string>.none();
            var nc = Option<decimal>.none();
            var nd = Option<byte>.none();
            var sa = 1.some();
            var sb = "string".toOption();
            var sc = 93.2m.some();
            var sd = Option<byte>.some(7);
            assertSome("1 string 93.2 7", (sa, sb, sc, sd).flatMap(someFunc));
            assertNone((na, sb, sc, sd).flatMap(someFunc));
            assertNone((sa, nb, sc, sd).flatMap(someFunc));
            assertNone((sa, sb, nc, sd).flatMap(someFunc));
            assertNone((sa, sb, sc, nd).flatMap(someFunc));

            assertNone((na, nb, sc, sd).flatMap(someFunc));
            assertNone((na, sb, nc, sd).flatMap(someFunc));
            assertNone((na, sb, sc, nd).flatMap(someFunc));
            assertNone((sa, nb, nc, sd).flatMap(someFunc));
            assertNone((sa, nb, sc, nd).flatMap(someFunc));
            assertNone((sa, sb, nc, nd).flatMap(someFunc));

            assertNone((na, nb, nc, sd).flatMap(someFunc));
            assertNone((na, nb, sc, nd).flatMap(someFunc));
            assertNone((na, sb, nc, nd).flatMap(someFunc));
            assertNone((sa, nb, nc, nd).flatMap(someFunc));
            assertNone((na, nb, nc, nd).flatMap(someFunc));

            assertNone((sa, sb, sc, sd).flatMap(noneFunc));
            assertNone((na, sb, sc, sd).flatMap(noneFunc));
            assertNone((sa, nb, sc, sd).flatMap(noneFunc));
            assertNone((sa, sb, nc, sd).flatMap(noneFunc));
            assertNone((sa, sb, sc, nd).flatMap(noneFunc));

            assertNone((na, nb, sc, sd).flatMap(noneFunc));
            assertNone((na, sb, nc, sd).flatMap(noneFunc));
            assertNone((na, sb, sc, nd).flatMap(noneFunc));
            assertNone((sa, nb, nc, sd).flatMap(noneFunc));
            assertNone((sa, nb, sc, nd).flatMap(noneFunc));
            assertNone((sa, sb, nc, nd).flatMap(noneFunc));

            assertNone((na, nb, nc, sd).flatMap(noneFunc));
            assertNone((na, nb, sc, nd).flatMap(noneFunc));
            assertNone((na, sb, nc, nd).flatMap(noneFunc));
            assertNone((sa, nb, nc, nd).flatMap(noneFunc));
            assertNone((na, nb, nc, nd).flatMap(someFunc));
        }

        [Property]
        public Property testFlatten() {
            var none = Option<decimal>.none();
            var nonenone = Option<Option<decimal>>.none();
            var somenone = Option<Option<decimal>>.some(none);
            return Prop.ForAll<decimal>(a => {
                var oa = a.some();
                var ooa = oa.toOption();
                assertSome(a, ooa.flatten());
                assertNone(somenone.flatten());
                assertNone(nonenone.flatten());
            });
        }

        [Property]
        public Property testFilter() {
            var none = Option<int?>.none();
            bool isTrue(int? x) => true;
            bool isFalse(int? x) => false;
            return Prop.ForAll<int?>(a => {
                var oa = Option<int?>.some(a); // nulls wrapped in some.
                assertSome(a, oa.filter(isTrue));
                assertNone(oa.filter(isFalse));
                assertNone(none.filter(isTrue));
                assertNone(none.filter(isFalse));
            });
        }

        [Property]
        public Property testFold() {
            var none = Option<int>.none();
            return Prop.ForAll<Func<int, string>, Func<string>, int>((onSome, onNone, a) => {
                var someResult = onSome(a);
                var nonResult = onNone();
                Assert.Equal(someResult, a.some().fold(onNone, onSome));
                Assert.Equal(nonResult, none.fold(onNone, onSome));
            });
        }

        [Property]
        public Property testSequence() {
            var none = Option<int>.none();
            return Prop.ForAll<int, int, int>((a, b, c) => {
                var oa = a.some();
                var ob = b.some();
                var oc = c.some();
                assertSome(new List<int>{a, b, c}, new List<Option<int>>{oa, ob, oc}.sequence());
                assertNone(new List<Option<int>>{none, ob, oc}.sequence());
                assertNone(new List<Option<int>>{oa, none, oc}.sequence());
                assertNone(new List<Option<int>>{oa, ob, none}.sequence());
                assertNone(new List<Option<int>>{none, none, oc}.sequence());
                assertNone(new List<Option<int>>{none, ob, none }.sequence());
                assertNone(new List<Option<int>>{oa, none, none }.sequence());
                assertNone(new List<Option<int>>{none, none, none }.sequence());
            });
        }

        [Property]
        public Property testTraverse() {
            var none = Option<int>.none();
            return Prop.ForAll<Func<int, string>, Tuple<int, int, int>>((f, t) => {
                var (a, b, c) = t;
                var oa = a.some();
                var ob = b.some();
                var oc = c.some();
                assertSome(new List<string>{f(a), f(b), f(c)}, new List<Option<int>>{oa, ob, oc}.traverse(f));
                assertNone(new List<Option<int>>{none, ob, oc}.traverse(f));
                assertNone(new List<Option<int>>{oa, none, oc}.traverse(f));
                assertNone(new List<Option<int>>{oa, ob, none}.traverse(f));
                assertNone(new List<Option<int>>{none, none, oc}.traverse(f));
                assertNone(new List<Option<int>>{none, ob, none }.traverse(f));
                assertNone(new List<Option<int>>{oa, none, none }.traverse(f));
                assertNone(new List<Option<int>>{none, none, none }.traverse(f));
            });
        }

        // test all the conversions in a ConversionsTests?
        [Fact]
        public void testEitherToOption() {
            var e = 1.right<string, int>();
            var o = e.asOption;
            assertSome(1, o);
            var e2 = o.asEither("wee");
            Assert.True(e2.isRight);
            Assert.Equal(1, e2.getRight);
            assertRight(e2, 1);
        }
    }
}
