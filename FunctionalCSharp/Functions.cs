using System;

namespace FunctionalCSharp
{
    public static class Functions
    {
        // NOTE: There are Func<> types defined in .NET for up to 16 parameters.

        
        public static Func<A, Func<B, Z>> curry<A, B, Z>(this Func<A, B, Z> f) { return a => b => f(a, b); }
        public static Func<A, Func<B, Func<C, Z>>> curry<A, B, C, Z>(this Func<A, B, C, Z> f) { return a => b => c => f(a, b, c); }
        public static Func<A, Func<B, Func<C, Func<D, Z>>>> curry<A, B, C, D, Z>(this Func<A, B, C, D, Z> f) { return a => b => c => d => f(a, b, c, d); }
        public static Func<A, Func<B, Func<C, Func<D, Func<E, Z>>>>> curry<A, B, C, D, E, Z>(this Func<A, B, C, D, E, Z> f) { return a => b => c => d => e => f(a, b, c, d, e); }

        public static Func<A, B, Z> uncurry<A, B, Z>(this Func<A, Func<B, Z>> f) { return (a, b) => f(a)(b); }
        public static Func<A, B, C, Z> uncurry<A, B, C, Z>(this Func<A, Func<B, Func<C, Z>>> f) { return (a, b, c) => f(a)(b)(c); }
        public static Func<A, B, C, D, Z> uncurry<A, B, C, D, Z>(this Func<A, Func<B, Func<C, Func<D, Z>>>> f) { return (a, b, c, d) => f(a)(b)(c)(d); }
        public static Func<A, B, C, D, E, Z> uncurry<A, B, C, D, E, Z>(this Func<A, Func<B, Func<C, Func<D, Func<E, Z>>>>> f) { return (a, b, c, d, e) => f(a)(b)(c)(d)(e); }

        public static Func<B, Z> pApply1<A, B, Z>(this Func<A, B, Z> f, A a) { return b => f(a, b); }
        public static Func<A, Z> pApply2<A, B, Z>(this Func<A, B, Z> f, B b) { return a => f(a, b); }
        public static Func<B, C, Z> pApply1<A, B, C, Z>(this Func<A, B, C, Z> f, A a) { return (b, c) => f(a, b, c); }
        public static Func<A, C, Z> pApply2<A, B, C, Z>(this Func<A, B, C, Z> f, B b) { return (a, c) => f(a, b, c); }
        public static Func<A, B, Z> pApply3<A, B, C, Z>(this Func<A, B, C, Z> f, C c) { return (a, b) => f(a, b, c); }
        public static Func<B, C, D, Z> pApply1<A, B, C, D, Z>(this Func<A, B, C, D, Z> f, A a) { return (b, c, d) => f(a, b, c, d); }
        public static Func<A, C, D, Z> pApply2<A, B, C, D, Z>(this Func<A, B, C, D, Z> f, B b) { return (a, c, d) => f(a, b, c, d); }
        public static Func<A, B, D, Z> pApply3<A, B, C, D, Z>(this Func<A, B, C, D, Z> f, C c) { return (a, b, d) => f(a, b, c, d); }
        public static Func<A, B, C, Z> pApply4<A, B, C, D, Z>(this Func<A, B, C, D, Z> f, D d) { return (a, b, c) => f(a, b, c, d); }
        public static Func<B, C, D, E, Z> pApply1<A, B, C, D, E, Z>(this Func<A, B, C, D, E, Z> f, A a) { return (b, c, d, e) => f(a, b, c, d, e); }
        public static Func<A, C, D, E, Z> pApply2<A, B, C, D, E, Z>(this Func<A, B, C, D, E, Z> f, B b) { return (a, c, d, e) => f(a, b, c, d, e); }
        public static Func<A, B, D, E, Z> pApply3<A, B, C, D, E, Z>(this Func<A, B, C, D, E, Z> f, C c) { return (a, b, d, e) => f(a, b, c, d, e); }
        public static Func<A, B, C, E, Z> pApply4<A, B, C, D, E, Z>(this Func<A, B, C, D, E, Z> f, D d) { return (a, b, c, e) => f(a, b, c, d, e); }
        public static Func<A, B, C, D, Z> pApply5<A, B, C, D, E, Z>(this Func<A, B, C, D, E, Z> f, E e) { return (a, b, c, d) => f(a, b, c, d, e); }

        public static Func<B, A, Z> swap<A, B, Z>(this Func<A, B, Z> f) { return (b, a) => f(a, b); }

        public static Func<A, Z> compose<A, B, Z>(this Func<B, Z> f, Func<A, B> g) { return a => f(g(a)); }
        public static Func<A, Z> andThen<A, B, Z>(this Func<A, B> f, Func<B, Z> g) { return a => g(f(a)); }

        public static A identity<A>(A a) { return a; }
    }
}
