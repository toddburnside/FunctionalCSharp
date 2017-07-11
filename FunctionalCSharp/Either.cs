using System;

namespace FunctionalCSharp
{
    public class Either<E, A> {
        private readonly E leftValue;
        private readonly A rightValue;

        public static Either<E, A> left(E e) { return new Either<E, A>(e, default(A), false); }
        public static Either<E, A> right(A a) { return new Either<E, A>(default(E), a, true); }

        public static Func<Either<E, Z>, Either<E, A>> lift<Z>(Func<Z, A> f) { return ez => ez.map(f); }

        public static Func<Either<E, Z>, Either<E, Y>, Either<E, A>> lift<Z, Y>(Func<Z, Y, A> f) {
            return (ez, ey) => ez.flatMap(z => ey.map(y => f(z, y)));
        }

        public static Func<Either<E, Z>, Either<E, Y>, Either<E, X>, Either<E, A>> lift<Z, Y, X>(Func<Z, Y, X, A> f) {
            return (oz, oy, ox) => oz.flatMap(z => oy.flatMap(y => ox.map(x => f(z, y, x))));
        }

        public static Either<E, A> map2<Z, Y>(Either<E, Z> ez, Either<E, Y> ey, Func<Z, Y, A> f) {
            return lift(f)(ez, ey);
        }

        public static Either<E, A> map3<Z, Y, X>(Either<E, Z> ez, Either<E, Y> ey, Either<E, X> ex, Func<Z, Y, X, A> f) {
            return lift(f)(ez, ey, ex);
        }

        // TODO: Add more maps and lifts

        public bool isRight { get; }

        public A getRight {
            get {
                if (!isRight) throw new Exception("Cannot get right of a left.");
                return rightValue;
            }
        }

        public E getLeft {
            get {
                if (isRight) throw new Exception("Cannot get left of a right.");
                return leftValue;
            }
        }

        public A getOrElse(A a) { return isRight ? rightValue : a; }

        // a "lazy" version. fa() will not get executed if this is a Right.
        public A getOrElse(Func<A> fa) { return isRight ? rightValue : fa(); }

        public B fold<B>(Func<E, B> fe, Func<A, B>  fa) { return isRight ? fa(rightValue) : fe(leftValue); }

        public Either<A, E> swap => new Either<A, E>(rightValue, leftValue, !isRight);

        public Either<E, B> map<B>(Func<A, B> f) {
            return isRight ? Either<E, B>.right(f(rightValue)) : Either<E, B>.left(leftValue);
        }

        // Add more map and lift instance methods

        public Either<F, A> leftMap<F>(Func<E, F> f) {
            return isRight ? Either<F, A>.right(rightValue) : Either<F, A>.left(f(leftValue));
        }

        public Either<E, B> flatMap<B>(Func<A, Either<E, B>> f) {
            return isRight ? f(rightValue) : Either<E, B>.left(leftValue);
        }

        public Either<F, A> leftFlatMap<F>(Func<E, Either<F, A>> f) {
            return isRight ? Either<F, A>.right(rightValue) : f(leftValue);
        }

        public Option<A> asOption => isRight ? Option<A>.some(rightValue) : Option<A>.none();

        public Validated<E, A> asValidated => isRight ? rightValue.valid<E, A>() : this.leftValue.invalid<E, A>();

        private Either(E e, A a, bool isRight) {
            leftValue = e;
            rightValue = a;
            this.isRight = isRight;
        }
    }

    // extension methods must be defined in a NON-GENERIC static class, so
    // with C#'s limited type inference and inablity to partially apply types,
    // there will be many times when all the types will need to be supplied.
    public static class EitherHelpers {
        public static Either<E, A> right<E, A>(this A a) { return Either<E, A>.right(a); }
        public static Either<E, A> left<E, A>(this E e) { return Either<E, A>.left(e); }

        public static Either<E, A> toEither<E, A>(this A a, E ifNull) where A : class {
            return a == null ? ifNull.left<E, A>() : a.right<E, A>();
        }

        public static Either<E, A> toEither<E, A>(this A? a, E ifNull) where A : struct {
            return a.HasValue ? a.Value.right<E, A>() : ifNull.left<E, A>();
        }

        public static Either<E, A> mapNull<E, A>(this Either<E, A> ea, E ifNull) where A : class {
            return ea.flatMap(a => a == null ? ifNull.left<E, A>() : a.right<E, A>());
        }

        public static Either<E, A> mapNull<E, A>(this Either<E, A?> eaq, E ifNull) where A : struct {
            return eaq.flatMap(aq => aq.HasValue ? aq.Value.right<E, A>() : ifNull.left<E, A>());
        }

        // NOTE: There are Func<> types defined in .NET for up to 16 parameters.

        // Unfortunately, catchOnly requires you specify ALL the generic parameter types...
        public static Func<A, Either<E, Z>> catchOnly<A, Z, E>(this Func<A, Z> f) where E: Exception {
            return a => {
                try { return f(a).right<E, Z>(); }
                catch (E e) { return e.left<E, Z>(); }
            };
        }

        public static Func<A, B, Either<E, Z>> catchOnly<A, B, Z, E>(this Func<A, B, Z> f) where E : Exception {
            return (a, b) => {
                try { return f(a, b).right<E, Z>(); }
                catch (E e) { return e.left<E, Z>(); }
            };
        }

        public static Func<A, B, C, Either<E, Z>> catchOnly<A, B, C, Z, E>(this Func<A, B, C, Z> f) where E : Exception
        {
            return (a, b, c) => {
                try { return f(a, b, c).right<E, Z>(); }
                catch (E e) { return e.left<E, Z>(); }
            };
        }

        public static Func<A, B, C, D, Either<E, Z>> catchOnly<A, B, C, D, Z, E>(this Func<A, B, C, D, Z> f) where E : Exception
        {
            return (a, b, c, d) => {
                try { return f(a, b, c, d).right<E, Z>(); }
                catch (E e) { return e.left<E, Z>(); }
            };
        }

        public static Func<A, Either<Exception, Z>> catchAll<A, Z>(this Func<A, Z> f) {
            return catchOnly<A, Z, Exception>(f);
        }

        public static Func<A, B, Either<Exception, Z>> catchAll<A, B, Z>(this Func<A, B, Z> f) {
            return catchOnly<A, B, Z, Exception>(f);
        }

        public static Func<A, B, C, Either<Exception, Z>> catchAll<A, B, C, Z>(this Func<A, B, C, Z> f)
        {
            return catchOnly<A, B, C, Z, Exception>(f);
        }

        public static Func<A, B, C, D, Either<Exception, Z>> catchAll<A, B, C, D, Z>(this Func<A, B, C, D, Z> f)
        {
            return catchOnly<A, B, C, D, Z, Exception>(f);
        }

        /// <summary>
        /// Converts a string into an int, capturing any exception.
        /// Unless you need the exception, consider using toInt() on Option.
        /// </summary>
        public static Either<Exception, int> toIntE(this string s) {
            try {
                return int.Parse(s).right<Exception, int>();
            }
            catch (Exception e) {
                return e.left<Exception, int>();
            }
        }

        /// <summary>
        /// Converts a string into a double, capturing any exception.
        /// Unless you need the exception, consider using toDouble() on Option.
        /// </summary>
        public static Either<Exception, double> toDoubleE(this string s)
        {
            try
            {
                return double.Parse(s).right<Exception, double>();
            }
            catch (Exception e)
            {
                return e.left<Exception, double>();
            }
        }

        // TODO: Add liftEither helper methods, and maybe map2, map3.
        public static Func<Either<E, A>, Either<E, Z>> liftEither<E, A, Z>(Func<A, Z> f) { return Either<E, Z>.lift(f); }

        public static Func<Either<E, A>, Either<E, B>, Either<E, Z>> liftEither<E, A, B, Z>(Func<A, B, Z> f) {
            return Either<E, Z>.lift(f);
        }

        public static Func<Either<E, A>, Either<E, B>, Either<E, C>, Either<E, Z>> liftEither<E, A, B, C, Z>(
            Func<A, B, C, Z> f) {
            return Either<E, Z>.lift(f);
        }
    }
}
