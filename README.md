# Univariate Polynomial Algebra System (AKS-Oriented)

## Overview

This project is a specialized computer algebra system for manipulating, expanding, and reducing **univariate polynomials**. It was designed primarily as a support tool for experimenting with and implementing the **AKS primality test**. The system is written in **C#** and focuses on exact arithmetic with large integers rather than floating-point approximations.

The scope of the project is intentionally narrow: correctness, clarity, and mathematical transparency are prioritized over performance optimizations or broad symbolic algebra coverage.

This project was created in early 2024, but has been uploaded to GitHub in late 2025.

---

## Getting Started
.NET6 or higher. NO other dependencies needed, just basic C#.
The program entry point initializes an interactive environment by calling:

```csharp
InteractiveSession();
```

This launches a basic terminal interface that allows users to enter and expand polynomials, raise them to powers, and observe the resulting coefficients.

---

## Numeric Model

- **All coefficients are `BigInteger`s**.
- Decimal (floating-point) numbers are not supported.
- Rational numbers are currently being experimented with, but their implementation is **experimental**, not thoroughly tested, and **unused** by the AKS algorithm.

This design choice reflects the number-theoretic nature of the AKS primality test, which operates strictly over the integers.

---

## Monomials

A **Monomial** represents a single-term polynomial of the form:

```
a·x^k
```

### Examples

```csharp
Monomial m = new Monomial(4, 2);        // 4x^2
Monomial constant = Monomial.Constant(4); // 4
Monomial m2 = Monomial.FromString("4x^6");
```

Monomials serve as the fundamental building blocks for polynomials.

---

## Polynomials

A **Polynomial** consists of one or more monomials.

### Construction

```csharp
Polynomial p = new Polynomial(m1, m2, m3);
```

where each argument is an arbitrary `Monomial`.

### Parsing from Strings

```csharp
Polynomial p = Polynomial.FromString("7x^4 - x^4");
```

Internally, like terms are combined and normalized.

---

## Global Modulus

In `Globals.cs`, an important variable is defined:

```csharp
public static BigInteger? modulus;
```

- If `modulus` is `null`, arithmetic is performed with unbounded integers.
- If `modulus` is non-null, **all integer arithmetic is performed modulo this value**.

### Important Note on Negative Coefficients

Modular reduction in this project follows a non-negative remainder convention. For example:

```
-1 mod 5 → 4
```

rather than `-1`. While this may differ from some expectations, it is **intentional and useful** for number-theoretic algorithms such as AKS.

### Design Trade-Off

Using a global modulus is not ideal from a software engineering perspective. However, given the small scope and experimental nature of the project, this design choice keeps the code simpler and easier to reason about.

---

## Polynomial Exponentiation and Polynomial Modulus

Polynomials can be raised to powers using:

```csharp
Polynomial Power(
    Polynomial polyBase,
    BigInteger exponent,
    Polynomial? polynomialMod = null
)
```

### Example

```csharp
Polynomial basePoly = Polynomial.FromString("x + 1");
Polynomial modPoly  = Polynomial.FromString("x^5 - 1");

Polynomial result = Polynomial.Power(
    polyBase: basePoly,
    exponent: 17,
    polynomialMod: modPoly
);
```

### What Does a Polynomial Modulus Mean?

When a polynomial modulus is supplied, the result is reduced modulo that polynomial after each multiplication step. Conceptually, this means computations occur in the quotient ring:

```
Z[x] / (polynomialMod)
```

This is essential for the AKS primality test, which relies on checking congruences of the form:

```
(x + a)^n ≡ x^n + a (mod x^r − 1, n)
```

The `polynomialMod` parameter enables these reductions directly and efficiently within the algebra system.

---

## AKS Primality Test

The file `AKS.cs` contains the core method:

```csharp
public static bool AKSPrimalityTest(BigInteger n)
```

This implements the AKS primality test in a direct and readable way.

### Performance Expectations

Although the AKS algorithm runs in **polynomial time**, it is still extremely slow in practice. This implementation should be viewed as:

- An educational reference
- An algorithmic curiosity
- A demonstration of symbolic polynomial manipulation

rather than a practical primality-testing tool.

---

## Additional Global Settings

Defined in `Globals.cs`:

```csharp
public static char symbol = 'x';
public static bool safetyChecks = true;
```

- **`symbol`** controls the polynomial variable used for display and parsing.
- **`safetyChecks`** issues warnings when the global modulus is `null`, reminding the user that polynomial expansion without a modulus may result in extremely large integers.

These safeguards are particularly useful when raising polynomials to high powers.

---

## Pascal’s Triangle Curiosity

The method:

```csharp
Program.ExpandBinomialToIncreasinglyHigherPowers(int numRows)
```

expands the binomial `(x + 1)` to progressively higher powers and prints each result.

The coefficients of these expansions correspond exactly to **Pascal’s Triangle**, providing a simple but elegant demonstration of the algebra engine in action.

---

## Project Status and Intent

This is a **personal project** created for exploration and demonstration purposes.

- Bugs and edge cases may exist.
- The codebase has not been rigorously hardened or maintained according to all best practices.
- The goal is not production readiness, but clarity and mathematical experimentation.

Nevertheless, the project is presented as a showcase for those curious about my approach to algorithmic problem-solving, symbolic computation, and C# design.

---