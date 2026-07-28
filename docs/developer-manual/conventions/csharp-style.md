# C# Style

This article covers the conventions for C# code in [`source/`](../../../source/). It is about style and structure on top of the language itself — for the repository layout the code lives in, see [Architecture](../architecture/overview.md).

## File Layout

Every file starts with a file-scoped namespace declaration (`namespace CliNetCore.Hosting;`), not a namespace block. Beyond the usings that [`ImplicitUsings`](../architecture/overview.md) already brings in, additional `using` directives are wrapped in a `#region Using Directives` block above the namespace declaration.

Within a type, members may be grouped into `#region` blocks when the file is large enough to benefit from it, for example `#region Constructors`, `#region Private Fields`, `#region Public Static Methods`, `#region Public Methods`, or `#region <InterfaceName> Implementation` for the members that implement a specific interface. A small file with only a handful of members does not need regions at all — do not add them just for the sake of having them.

## Documentation Comments

Every public and internal member has an XML documentation comment (`GenerateDocumentationFile` is enabled in [the project files](../architecture/overview.md), so these ship in the compiled output). A `<summary>` explains not just what a member does, but why it exists and how it relates to the rest of the design — the reasoning belongs next to the code, not only in a commit message. Parameters and return values are documented with `<param>` and `<returns>`. When a member implements an interface member or overrides a base member and has nothing to add beyond what is already documented there, use `<inheritdoc/>` instead of repeating the text.

Prose inside documentation comments follows the same punctuation rule as the rest of the documentation: periods, not semicolons (see [Markdown Style](markdown-style.md)).

## Plain Comments

A plain `//` comment inside a method body is not prose — it explains a non-obvious *why* (a hidden constraint, an ordering requirement, a workaround) that the code next to it does not already make clear. It follows the opposite punctuation rule from documentation comments and the rest of the documentation: sentences are separated by semicolons, and the last one takes no terminal punctuation.

```csharp
// The fallback is only registered if the consumer did not register one of their own, so that the type stays usable out of the box while still
// letting a caller replace the behavior entirely simply by registering their own implementation
```

Do not write a comment that only restates what the code already says. If removing it would not confuse a future reader, it should not be there.

## Types and Members

- **Classes are `sealed` by default.** A type is only left unsealed when it is deliberately designed as a base class for extension. Where a type needs to reuse behavior from a sealed framework type it cannot inherit from (for example, a concrete host or builder from `Microsoft.Extensions.Hosting`), it wraps that type and forwards to it (composition) instead.
- **Prefer expression-bodied members** (`=>`) for members whose implementation is a single expression, including constructors, property getters, and simple methods.
- **Access instance members through `this.`** explicitly, even in a constructor or method where the name would resolve unambiguously without it.
- **Async methods are suffixed `Async`** and return `Task` or `Task<T>`. They accept a `CancellationToken` (typically the last parameter) whenever the work they do can meaningfully be canceled.
- **Prefer factory methods over public constructors** when a type has invariants to establish or work to do beyond simple field assignment. An `internal` constructor paired with a `public static` creation method (or a dedicated builder type) keeps construction explicit and controllable.
- **Interfaces are prefixed with `I`** (`IDisposable`), following the standard .NET convention.
- **Private fields are `camelCase`** with no leading underscore and no `_` prefix.

## Nullability and Implicit Usings

Every project enables `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>` (see [Architecture](../architecture/overview.md)). Write code that is genuinely null-safe rather than silencing the analyzer with `!`; a nullable parameter or return type should mean that `null` is a real, handled case.

## Formatting

C# is **not** formatted by [dprint](../tooling/formatting-dprint.md) — dprint has no C# plugin in this project, so `.csproj` and `.slnx` files are formatted by it (they are XML), but `.cs` files are not. Indentation and basic whitespace rules for C# come from [`.editorconfig`](../../../.editorconfig) (4 spaces, no tabs), which most editors, including Visual Studio Code, apply automatically. Beyond what `.editorconfig` enforces, matching the conventions on this page during review is the only guard C# has today.

## Related

- Where these files live in the repository: [Architecture](../architecture/overview.md).
- The punctuation rule for prose versus plain comments also governs [Markdown Style](markdown-style.md).
- Formatting for the file types dprint does own: [dprint](../tooling/formatting-dprint.md).
