# FunctionalCSharp
A library to help with functional programming style in C#.

## why?

There are already a number of libraries for doing functional programming in C#. Some of these even include persistent data types such as the standard List. However, my main interest was in exploring the potentials and shortcomings of C# as a language for doing strongly typed functional programming. More on that soon.

## what?

A library containing some data types and helper functions. The style of this library was heavily influenced by [Typelevel Cats](http://typelevel.org/cats), which in turn was based on [Scalaz](https://github.com/scalaz/scalaz).

#### Data Types
* `Option<A>` - A "container" datatype that holds either a value of type A or nothing. This is similar to the nullable data types in C#, such as int?, but can also be used for reference types. It can be used instead of null and has the advantage that null cannot be tracked by the type system, but Option can. There are helper methods for converting nulls, handling functions that may return nulls, parsing strings, etc.
* `Either<E, A>` - A disjunction that contains either an E value (a left), or an A value (a right). By convention, the E is used for errors and the A for success. Eiter is right biased, meaning that it is a monad on the right value and the standard map() and flatMap() functions operate on the A. There are also methods for operating on the E values, and swapping E and A. In addition, there are helper methods for things like wrapping functions that throw exceptions, etc.
* `Validated<E, A>` - LIke Either<E, A>, this is a disjunction. However, in this case the "left" side is an IReadOnlyList<E>. In languages like Scala or Haskell, the "left" would be a SemiGroup, but it is not possible to abstract over the concept of a SemiGroup in C#, so I had to settle for a list. This isn't so bad, since a list is the Free Monoid (and all Monoids are SemiGroups), so a list of items can be "interpreted" into any other Monoid. The value of Validated over Either is that Either, being a Monad, fails fast on errors. But Validated is a Applicative Functor, so it has the ability to collect multiple errors. This makes it valuable for things like data validation.

#### Helper Functions
In addition to the helper functions related to the above data types, there are helper functions for the following:
* Currying and Uncurrying Functions
* Partial Application of Functions
* Composing Functions
* Swapping Function Parameters

## status

I was thinking I might use this library for C# projects at work, but the opportunity has not yet arisen. So, although this library is fairly complete and has a lot of unit tests, it has not seen much "real world" usage. As a result, I have not taken the time to come up with reasonable documentation. For now, the best resource for learning to use the library is the tests, or looking at the source code. Since I am not using this at work, and all my personal projects are in Scala, I doubt I will be spending significant time improving the situation. Looking at the documentation for the corresponding data types in Scala or Typlevel Cats would also be useful.

As far as completeness, the `Option` data type is fairly complete, while the `Either` and `Validated` data types need to be fleshed out a bit with some of the functionality that is already in `Option`.
