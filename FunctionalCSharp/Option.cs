using System;
using System.Collections.Generic;

namespace FunctionalCSharp
{
    public class Option<A> {
        private readonly A theValue;
        public static Option<A> some(A a) { return new Option<A>(a); }
        public static Option<A> none() { return new Option<A>(); }

        public static Func<Option<Z>, Option<A>> lift<Z>(Func<Z, A> f) { return oz => oz.map(f); }

        public static Func<Option<Z>, Option<Y>, Option<A>> lift<Z, Y>(Func<Z, Y, A> f) {
            return (oz, oy) => oz.flatMap(z => oy.map(y => f(z, y)));
        }

        public static Func<Option<Z>, Option<Y>, Option<X>, Option<A>> lift<Z, Y, X>(Func<Z, Y, X, A> f) {
            return (oz, oy, ox) => oz.flatMap(z => oy.flatMap(y => ox.map(x => f(z, y, x))));
        }

        public static Func<Option<Z>, Option<Y>, Option<X>, Option<W>, Option<A>> lift<Z, Y, X, W>(Func<Z, Y, X, W, A> f)
        {
            return (oz, oy, ox, ow) => oz.flatMap(z => oy.flatMap(y => ox.flatMap(x => ow.map(w =>f(z, y, x, w)))));
        }

//        public static Option<A> map2<Z, Y>(Option<Z> oz, Option<Y> oy, Func<Z, Y, A> f) { return lift(f)(oz, oy); }
//
//        public static Option<A> map3<Z, Y, X>(Option<Z> oz, Option<Y> oy, Option<X> ox, Func<Z, Y, X, A> f) {
//            return lift(f)(oz, oy, ox);
//        }
//
//        public static Option<A> map4<Z, Y, X, W>(Option<Z> oz, Option<Y> oy, Option<X> ox, Option<W> ow, Func<Z, Y, X, W, A> f) {
//            return lift(f)(oz, oy, ox, ow);
//        }

        // TODO: Add a forEach or run method to perform side effects on the value?

        public A value {
            get {
                if (!hasValue) throw new Exception("Cannot get value of none.");
                return theValue;
            }
        }

        public bool hasValue { get; }

        public A getOrElse(A a) => hasValue ? theValue : a;

        // a "lazy" version. fa() will not get executed if this is a Some.
        public A getOrElse(Func<A> fa) => hasValue ? theValue : fa();

        public Option<A> filter(Func<A, bool> f) => hasValue && f(theValue) ? this : none();

        public B fold<B>(Func<B> onNone, Func<A, B> onSome) { return hasValue ? onSome(theValue) : onNone(); }

        // since C#'s type system precludes our abstracting over Applicatives
        // or Monads, pure is pretty much useless.
//        public Option<A> pure(A a) { return some(a); }

        public Option<B> map<B>(Func<A, B> f) { return hasValue ? Option<B>.some(f(value)) : Option<B>.none(); }

        public Option<B> flatMap<B>(Func<A, Option<B>> f) { return hasValue ? f(value) : Option<B>.none(); }

        public Either<E, A> asEither<E>(E ifNone) => hasValue ? theValue.right<E, A>() : ifNone.left<E, A>();

        public Validated<E, A> asValidated<E>(E ifNone) => hasValue ? theValue.valid<E, A>() : ifNone.invalid<E, A>();

        private Option(A value) {
            theValue = value;
            hasValue = true;
        }

        private Option() { hasValue = false; }
    }

    // TODO: Add linq specific extensions for Select(), etc. ?
    public static class OptionHelpers {
        // This function will wrap a null in the Option, which usually isn't what you want. So,
        // reference types (including nullables such as int?) should use a.toOption(), instead.

        // This only works for value types, not reference types. Otherwise, you could inadvertantly 
        // wrap a null in an some() rather than make it a none. See comments below about why separate
        // method names were needed for value types and reference types.
        // If you really want to wrap a null inside a some, you must explicitly call Option<?>.some(?);
        public static Option<A> some<A>(this A a) where A : struct => Option<A>.some(a);

        // Not as useful, but I can't create an extension method for a type itself
        public static Option<A> none<A>(this A a) => Option<A>.none();

        // Unfortunately, can't specify toOption() for values that are not classes and are 
        // non-nullable. This is because C# does not seem to take the type constraint into account
        // when determining method overloads. So 
        //     public static Option<A> toOption<A>(this A a) where A : struct {...}
        // gives a "member with the same signature is already declared" error. I know there
        // is no overlap, but the compiler apparently does not.
        public static Option<A> toOption<A>(this A a) where A : class {
            return a == null ? Option<A>.none() : Option<A>.some(a);
        }

        public static Option<A> toOption<A>(this A? a) where A : struct {
            return a.HasValue ? a.Value.some() : Option<A>.none();
        }

        // mapNull functions need to be extension methods becase I need to add 
        // constraints to the generic parameters.
        public static Option<A> mapNull<A>(this Option<A> oa) where A : class {
            // for some, we'll just return the original
            return oa.flatMap(a => a == null ? Option<A>.none() : oa);
        }

        public static Option<A> mapNull<A>(this Option<A?> oaq) where A : struct {
            return oaq.flatMap(aq => aq.HasValue ? aq.Value.some() : Option<A>.none());
        }

        public static Func<Option<A>, Option<Z>> liftOption<A, Z>(this Func<A, Z> f) { return Option<Z>.lift(f); }

        public static Func<Option<A>, Option<B>, Option<Z>> liftOption<A, B, Z>(this Func<A, B, Z> f) {
            return Option<Z>.lift(f);
        }

        public static Func<Option<A>, Option<B>, Option<C>, Option<Z>> liftOption<A, B, C, Z>(this Func<A, B, C, Z> f) {
            return Option<Z>.lift(f);
        }

        public static Func<Option<A>, Option<B>, Option<C>, Option<D>, Option<Z>> liftOption<A, B, C, D, Z>(this Func<A, B, C, D, Z> f) {
            return Option<Z>.lift(f);
        }

        public static Option<Z> map<A, B, Z>(this (Option<A> oa, Option<B> ob) t, Func<A, B, Z> f) {
            return Option<Z>.lift(f)(t.oa, t.ob);
        }

        public static Option<Z> map<A, B, C, Z>(this (Option<A> oa, Option<B> ob, Option<C> oc) t, Func<A, B, C, Z> f) {
            return Option<Z>.lift(f)(t.oa, t.ob, t.oc);
        }

        public static Option<Z> map<A, B, C, D, Z>(this (Option<A> oa, Option<B> ob, Option<C> oc, Option<D> od) t, Func<A, B, C, D, Z> f) {
            return Option<Z>.lift(f)(t.oa, t.ob, t.oc, t.od);
        }

        public static Option<Z> flatMap<A, B, Z>(this (Option<A> oa, Option<B> ob) t, Func<A, B, Option<Z>> f) {
            return t.oa.flatMap(a => t.ob.flatMap(b => f(a, b)));
        }

        public static Option<Z> flatMap<A, B, C, Z>(this (Option<A> oa, Option<B> ob, Option<C> oc) t, Func<A, B, C, Option<Z>> f) {
            return t.oa.flatMap(a => t.ob.flatMap(b => t.oc.flatMap(c => f(a, b, c))));
        }

        public static Option<Z> flatMap<A, B, C, D, Z>(this (Option<A> oa, Option<B> ob, Option<C> oc, Option<D> od) t, Func<A, B, C, D, Option<Z>> f) {
            return t.oa.flatMap(a => t.ob.flatMap(b => t.oc.flatMap(c => t.od.flatMap(d => f(a, b, c, d)))));
        }

        public static Option<A> flatten<A>(this Option<Option<A>> ooa) => ooa.hasValue ? ooa.value : Option<A>.none();

        // This method must enumerate the Enumerable<Option<A>> into a List<A> in memory.
        public static Option<List<A>> sequence<A>(this IEnumerable<Option<A>> oas) {
            // Do this mutably so we only need to enumerate the list once. 
            // We could do .Any() and then Select(), but would need to enumerate twice
            var newList = new List<A>();
            foreach (var oa in oas) {
                if (!oa.hasValue) {
                    return Option<List<A>>.none();
                }
                newList.Add(oa.value);
            }
            return newList.toOption();
        }

        // This method must enumerate the Enumerable<Option<A>> into a List<A> in memory.
        public static Option<List<B>> traverse<A, B>(this IEnumerable<Option<A>> oas, Func<A, B> f)
        {
            // Do this mutably so we only need to enumerate the list once. 
            // We could do .Any() and then Select(), but would need to enumerate twice
            var newList = new List<B>();
            foreach (var oa in oas)
            {
                if (!oa.hasValue)
                {
                    return Option<List<B>>.none();
                }
                newList.Add(f(oa.value));
            }
            return newList.toOption();
        }
    }
}
