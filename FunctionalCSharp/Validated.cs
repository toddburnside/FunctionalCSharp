using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionalCSharp
{
    // I can't generalize the SemiGroup in E, because C# won't do that. So
    // I had to fix on a IReadOnlyList<E>. This isn't too bad, since Lists
    // are Free Monoids that can be interpreted into any other Monoid, and all
    // Monoids are also SemiGroups.
    // The most common will be a List<String>, which means a Validated<String, ?>
    // Unfortunately, a type alias for Validated<String, ?> is not possible. Using the 
    // valid() method in the ValidatedHelpers can make this a little less painful.
    public class Validated<E, A> {
        private readonly IReadOnlyList<E> invalidValue;
        private readonly A validValue;

        public static Validated<E, A> valid(A a) { return new Validated<E, A>(null, a, true);}
        public static Validated<E, A> invalid(E e) { return new Validated<E, A>(new List<E> {e}, default(A), false); }
        private static Validated<E, A> invalid(IReadOnlyList<E> le) { return new Validated<E, A>(le, default(A), false); }

        public static Validated<E, (Z, Y)> product<Z, Y>(Validated<E, Z> vz, Validated<E, Y> vy) {
            var errors = new List<E>();
            accumulate(vz, errors);
            accumulate(vy, errors);
            return errors.Any()
                ? Validated<E, (Z, Y)>.invalid(errors)
                : Validated<E, (Z, Y)>.valid((vz.validValue, vy.validValue));
            // If I implemented apply as a primitive, I could derive map from pure and apply. Then,
            // I could implement product with map2:
            // return Validated<E, Tuple<Z, Y>>.map2(vz, vy, Tuple.Create);
        }

        public static Validated<E, (Z, Y, X)> tuple3<Z, Y, X>(Validated<E, Z> vz, Validated<E, Y> vy, Validated<E, X> vx)
        {
            var errors = new List<E>();
            accumulate(vz, errors);
            accumulate(vy, errors);
            accumulate(vx, errors);
            return errors.Any()
                ? Validated<E, (Z, Y, X)>.invalid(errors)
                : Validated<E, (Z, Y, X)>.valid((vz.validValue, vy.validValue, vx.validValue));
        }

        public static Validated<E, (Z, Y, X, W)> tuple4<Z, Y, X, W>(Validated<E, Z> vz, Validated<E, Y> vy, Validated<E, X> vx, Validated<E, W> vw)
        {
            var errors = new List<E>();
            accumulate(vz, errors);
            accumulate(vy, errors);
            accumulate(vx, errors);
            accumulate(vw, errors);
            return errors.Any()
                ? Validated<E, (Z, Y, X, W)>.invalid(errors)
                : Validated<E, (Z, Y, X, W)>.valid((vz.validValue, vy.validValue, vx.validValue, vw.validValue));
        }

        public static Validated<E, A> map2<Z, Y>(Validated<E, Z> vz, Validated<E, Y> vy, Func<Z, Y, A> f) {
            var vt = Validated<E, (Z, Y)>.product(vz, vy);
            if (vt.isValid) {
                var t = vt.validValue;
                return valid(f(t.Item1, t.Item2));
            }
            return invalid(vt.invalidValue);
            // if we implemented apply instead of product, map2 would be:
            // Validated<E, Func<Z, A>> vfza = vy.map(f.pApply2);
            // return vz.apply<A>(vfza);
        }

        public static Validated<E, A> map3<Z, Y, X>(Validated<E, Z> vz, Validated<E, Y> vy, Validated<E, X> vx, Func<Z, Y, X, A> f)
        {
            var vt = Validated<E, (Z, Y, X)>.tuple3(vz, vy, vx);
            if (vt.isValid)
            {
                var t = vt.validValue;
                return valid(f(t.Item1, t.Item2, t.Item3));
            }
            return invalid(vt.invalidValue);
        }

        public static Validated<E, A> map4<Z, Y, X, W>(Validated<E, Z> vz, Validated<E, Y> vy, Validated<E, X> vx, Validated<E, W> vw, Func<Z, Y, X, W, A> f)
        {
            var vt = Validated<E, (Z, Y, X)>.tuple4(vz, vy, vx, vw);
            if (vt.isValid)
            {
                var t = vt.validValue;
                return valid(f(t.Item1, t.Item2, t.Item3, t.Item4));
            }
            return invalid(vt.invalidValue);
        }

        // TODO: lifts
        public static Func<Validated<E, Z>, Validated<E, A>> lift<Z>(Func<Z, A> f) { return vz => vz.map(f); }

        public static Func<Validated<E, Z>, Validated<E, Y>, Validated<E, A>> lift<Z, Y>(Func<Z, Y, A> f) {
            return (oz, oy) => map2(oz, oy, f);
        }

        public static Func<Validated<E, Z>, Validated<E, Y>, Validated<E, X>, Validated<E, A>> lift<Z, Y, X>(Func<Z, Y, X, A> f)
        {
            return (oz, oy, ox) => map3(oz, oy, ox, f);
        }

        // A side-effecting helper method - not visible to the outside. Needed because we don't have an immutable list in C#
        private static void accumulate<B>(Validated<E, B> vb, List<E> errors) {
            if (!vb.isValid) errors.AddRange(vb.invalidValue);
        }

        public bool isValid { get; }

        public IReadOnlyList<E> getInvalid {
            get {
                if (isValid) throw new Exception("Cannot get invalid of a valid.");
                return invalidValue;
            }
        }

        public A getValid {
            get {
                if (!isValid) throw new Exception("Cannot get valid of an invalid.");
                return validValue;
            }
        }

        public A getOrElse(A a) { return isValid ? validValue : a; }

        // a "lazy" version. fa() will not get executed if this is a Valid.
        public A getOrElse(Func<A> fa) { return isValid ? validValue : fa(); }

        public B fold<B>(Func<IReadOnlyList<E>, B> fe, Func<A, B> fa) {
            return isValid ? fa(validValue) : fe(this.invalidValue);
        }

        public Validated<E, B> map<B>(Func<A, B> f) {
            return isValid ? Validated<E, B>.valid(f(validValue)) : Validated<E, B>.invalid(invalidValue);
            // could also implement this as "return apply(Validated<E, Func<A, B>>.pure(f));", but it would be less efficient in this case
        }

        // Validated is an Applicative Functor, not a Monad.
        public Validated<E, B> apply<B>(Validated<E, Func<A, B>> f) {
            var vt = Validated<E, (A, Func<A, B>)>.product(this, f);
            return vt.map(t => t.Item2(t.Item1));
            // Could also implement apply as a primitive and implement map and product in terms of primitives
//            if (isValid) {
//                return f.isValid ? Validated<E, B>.valid(f.validValue(this.validValue)) : Validated<E, B>.invalid(f.invalidValue);
//            }
//
//            if (f.isValid) return Validated<E, B>.invalid(this.invalidValue);
//
//            var le = new List<E>(invalidValue);
//            le.AddRange(f.invalidValue);
//            return Validated<E, B>.invalid(le);
        }

        public static Validated<E, A> pure(A a) => a.valid<E, A>();

        public Option<A> asOption => isValid ? Option<A>.some(validValue) : Option<A>.none();

        public Either<IReadOnlyList<E>, A> asEither
            => isValid ? validValue.right<IReadOnlyList<E>, A>() : this.invalidValue.left<IReadOnlyList<E>, A>();

        private Validated(IReadOnlyList<E> le, A a, bool isValid) {
            invalidValue = le;
            validValue = a;
            this.isValid = isValid;
        }
    }

    public static class ValidatedHelpers {
        public static Validated<E, A> valid<E, A>(this A a) { return Validated<E, A>.valid(a); }
        public static Validated<string, A> valid<A>(this A a) { return Validated<string, A>.valid(a); }
        public static Validated<E, A> invalid<E, A>(this E e) { return Validated<E, A>.invalid(e); }

        public static Validated<string, A> invalid<A>(this string e) { return Validated<string, A>.invalid(e); }

        // Does this one make sense. How to differentiate between this and the previous one...Maybe someone would want the whole list as E
//        public static Validated<E, A> invalid<E, A>(this IReadOnlyList<E> le) { return Validated<E, A>.invalid(le); }

        public static Validated<E, A> map<E, A, Z, Y>(this (Validated<E, Z> vz, Validated<E, Y> vy) t, Func<Z, Y, A> f) {
            return Validated<E, A>.map2(t.vz, t.vy, f);
        }

        public static Validated<E, A> map<E, A, Z, Y, X>(this (Validated<E, Z> vz, Validated<E, Y> vy, Validated<E, X> vx) t, Func<Z, Y, X, A> f) {
            return Validated<E, A>.map3(t.vz, t.vy, t.vx, f);
        }

        public static Validated<E, A> map<E, A, Z, Y, X, W>(this (Validated<E, Z> vz, Validated<E, Y> vy, Validated<E, X> vx, Validated<E, W> vw) t, Func<Z, Y, X, W, A> f) {
            return Validated<E, A>.map4(t.vz, t.vy, t.vx, t.vw, f);
        }
    }
}
