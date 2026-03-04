# WhyBuildersUseStructNotInterface.md

In short: it's about compile-time safety and generating concrete types for every builder steps. Boxing concerns are secondary. 

## Prompt

i have this builder pattern:

	public interface IEveryUnitSet : IEveryBuilder {}
	public struct EveryUnitSet : IEveryUnitSet {}

	public static class EveryBuilderUnitExtensions
	{
		/// <summary>Selects frame-based execution.</summary>
		public static EveryBuilder<EveryUnitSet> Frames(this EveryBuilder<EveryBuilderStart> b)
		{
			var options = b.Options;
			options.Process = Coroutine.Process.FrameUpdate;
			return new EveryBuilder<EveryUnitSet>(b.Script, b.Token, in options);
		}

the base builder is defined like this:

    public readonly struct EveryBuilder<T> where T : struct, IEveryBuilder
    {
    internal readonly Script Script;
    internal readonly BuilderToken Token;
    internal readonly EveryOptions Options;

why is the struct the generic type rather than the interface?

## Gemini's Answer

The reason you are using the concrete struct (like EveryUnitSet) as the generic type parameter instead of the interface (IEveryUnitSet) comes down to performance and avoiding boxing within the .NET runtime. [1, 2]

By using EveryBuilder<EveryUnitSet> where the generic constraint is where T : struct, IEveryBuilder, you gain several key benefits:

## 1. Avoiding Boxing
   If you were to use the interface as the type parameter (e.g., EveryBuilder<IEveryUnitSet>), any time the builder needs to interact with T as a value, the runtime would likely box that struct. Boxing moves the value from the stack to the heap, which incurs a performance penalty and creates garbage for the GC to collect. By keeping the concrete struct as the type, it stays on the stack. [3, 4, 5, 6, 7]

## 2. JIT Specialization (Monomorphization)
   When you use a concrete struct as a generic argument, the .NET Just-In-Time (JIT) compiler generates a specialized version of the EveryBuilder<T> code specifically for that struct. [8, 9]

* Structs: The JIT creates a unique machine code implementation for each unique struct type used.
* Interfaces/Classes: Generics used with reference types (like interfaces) usually share the same implementation.
  This specialization allows the JIT to inline methods and eliminate the overhead of virtual table lookups that normally come with interfaces. [8, 10, 11, 12, 13]

## 3. Compile-Time Type Safety in Fluent APIs [14]
   In the [Builder Pattern](https://en.wikipedia.org/wiki/Builder_pattern), using concrete types allows the compiler to track the "state" of your builder through the type system. If Frames() returns EveryBuilder<EveryUnitSet>, the compiler knows exactly which extension methods are available next. If it returned a generic EveryBuilder<IEveryUnitSet>, you might lose the ability to restrict subsequent calls to only those valid for a "UnitSet" state without casting. [15, 16]
 

## Summary of Differences

| Feature [3, 5, 8, 10, 17] | EveryBuilder<EveryUnitSet> (Concrete Struct) | EveryBuilder<IEveryUnitSet> (Interface) |
|---|---|---|
| Memory | Stack-allocated; No boxing. | Potential heap allocation (Boxing). |
| Performance | Faster; Allows aggressive inlining. | Slower; Virtual method lookups. |
| Type Tracking | Exact type preserved for fluent API. | Type is "erased" to the interface level. |

[1] [https://codefinity.com](https://codefinity.com/courses/v2/01b42355-f985-4ba6-8146-359d72ba9d0c/00505c3c-66aa-45c2-abab-090e81a36cb7/54107e24-69c8-4ab8-a331-1a765647753b#:~:text=Generics%20allow%20you%20to%20define%20the%20structure,when%20you%20use%20the%20interface%20or%20class.)
[2] [https://microsoft.github.io](https://microsoft.github.io/bond/manual/compiler.html#:~:text=Generic%20structs%20are%20parameterized%20with%20one%20or,element%20type%2C%20parameter%20of%20a%20generic%20struct%29.)
[3] [https://www.reddit.com](https://www.reddit.com/r/csharp/comments/1k89t4m/whats_the_technical_reason_for_structtointerface/)
[4] [https://giannisakritidis.com](https://giannisakritidis.com/blog/Avoid-Struct-Boxing/)
[5] [https://medium.com](https://medium.com/@imani.sajad92/struct-vs-class-b4c89ba88070#:~:text=Struct%20Advantages:%20Avoiding%20Garbage%20Collection%20Overhead:%20Structs,memory%20usage%2C%20especially%20for%20small%20data%20types.)
[6] [https://learn.microsoft.com](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1871)
[7] [https://blog.devgenius.io](https://blog.devgenius.io/boxing-and-unboxing-in-rust-6802c128b345#:~:text=Boxing%20in%20Rust%20refers%20to%20the%20process,and%20thus%20move%20it%20to%20the%20heap.)
[8] [https://stackoverflow.com](https://stackoverflow.com/questions/25508615/are-value-types-boxed-when-passed-as-generic-parameters-with-an-interface-constr)
[9] [https://crates.io](https://crates.io/crates/assert_layout/0.0.1)
[10] [https://www.reddit.com](https://www.reddit.com/r/csharp/comments/atfw16/interface_vs_generic_which_is_faster/)
[11] [https://github.com](https://github.com/dotnet/csharplang/discussions/6898)
[12] [https://medium.com](https://medium.com/@ayushsrivastava1374/net-compilation-process-decoded-5bb5850dc59e#:~:text=Code%20Execution%20Management:%20It%20provides%20features%20like,into%20native%20machine%20code%20for%20efficient%20execution.)
[13] [https://www.reddit.com](https://www.reddit.com/r/dotnet/comments/16kwlj3/sonarqube_advised_me_to_implement_iequatablet_on/#:~:text=The%20JIT%20compiler%20will%20output%20a%20unique,already%20knows%20exactly%20which%20method%20to%20call.)
[14] [https://medium.com](https://medium.com/@kaly.salas.7/mastering-generics-and-traits-in-rust-best-practices-for-cleaner-code-846bf9c1ba9f#:~:text=The%20benefit?%20Compile%2Dtime%20safety:%20trying%20%60max%60%20on,promotes%20trait%2Doriented%20design%2C%20where%20behaviors%20are%20composable.)
[15] [https://www.reddit.com](https://www.reddit.com/r/rust/comments/r3jbk1/when_using_the_builder_pattern_should_the/)
[16] [https://stackoverflow.com](https://stackoverflow.com/questions/68117367/is-there-a-way-to-implement-a-builder-pattern-with-a-restrictive-api-without-b)
[17] [https://www.naukri.com](https://www.naukri.com/code360/library/difference-between-abstract-class-and-interface-in-c-sharp)
