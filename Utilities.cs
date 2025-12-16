using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Polylib
{
    internal class Utilities
    {
        public static void Warn(string message) 
        {
            Console.WriteLine($"\n!! WARNING !!\n{message}\nSet Globals.safetyChecks to false to disable these warnings\n");
        }
        // Given an integer, compute its value modulo Globals.modulus. If Globals.modulus is null, then just return the integer untouched
        public static BigInteger Modulo(BigInteger a, BigInteger? b=null)
        {
            if (b == null)
            {
                b = Globals.modulus;
            }

            if (b == null)
            {
                return a;
            }
            if (b == 0)
            {
                throw new ArgumentException("Modulus cannot be zero");
            }
            else if (b < 0)
            {
                throw new NotImplementedException();
            }

            BigInteger result = a % (BigInteger)b;

            if (!Globals.modIgnoreSign && result < 0)
            {
                result += BigInteger.Abs((BigInteger)b);
            }

            return result;
        }


        private static BigInteger ModularMultiplicativeInverse(BigInteger b, BigInteger mod)
        {
            if (mod == 0)
            {
                throw new ArgumentException("Modulus cannot be zero");
            }
            else if (mod < 0)
            {
                throw new NotImplementedException("Modulus cannot be negative for now");
            }
            if (BigInteger.GreatestCommonDivisor(b, mod) != 1)
            {
                throw new BadModulusException("base is not invertible for the given modulus");
            }
            if (b < 0)
            {
                b = mod + b;
            }

            BigInteger m0 = mod;
            BigInteger x0 = 0;
            BigInteger x1 = 1;

            if (mod == 1)
            {
                return 0;
            }

            while (b > 1)
            {
                BigInteger q = b / mod;
                BigInteger t = mod;

                mod = b % mod;
                b = t;

                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }

            if (x1 < 0)
            {
                x1 += m0;
            }

            return x1;
        }

        private static BigInteger ModDivide(BigInteger a, BigInteger b, BigInteger mod)
        {
            BigInteger modularInverse = ModularMultiplicativeInverse(b, mod);
            BigInteger result = (a * modularInverse) % mod;
            if (!Globals.modIgnoreSign && result < 0)
            {
                result += BigInteger.Abs(mod);
            }
            return result;
        }

        public static BigInteger Divide(BigInteger a, BigInteger b)
        {
            if (Globals.modulus == null)
            {
                if (a % b != 0)
                    throw new ArgumentException($"Encountered non-integer value during division ({a} and {b})");
                return a / b;
            }
            else
            {
                BigInteger modularInverse = ModularMultiplicativeInverse(b, mod: (BigInteger)Globals.modulus);
                if (Globals.modIgnoreSign && (a < 0 != b < 0))
                    return -ModDivide(BigInteger.Abs(a), BigInteger.Abs(b), mod: (BigInteger)Globals.modulus);
                else
                    return ModDivide(a, b, mod: (BigInteger)Globals.modulus);
            }
            
        }
        public static BigInteger GCD(BigInteger a, BigInteger b)
        {
            if (a < 0 || b < 0)
                throw new NotImplementedException("Does not accept negative numbers");
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }
        public static BigInteger Power(BigInteger x, BigInteger n, BigInteger? mod=null)
        {
            if (n < 0)
                throw new ArgumentException("Exponent must be non-negative.");

            BigInteger result = 1;
            BigInteger baseValue = x;

            while (n > 0)
            {
                if (n % 2 == 1)
                {
                    // If n is odd, multiply result by baseValue
                    result *= baseValue;
                    if (mod != null)
                        result %= (BigInteger) mod;
                }

                // Square baseValue for the next iteration
                baseValue *= baseValue;
                if (mod != null)
                    baseValue %= (BigInteger) mod;

                // Divide n by 2 for the next iteration
                n /= 2;
            }

            return result;
        }

        public static int Totient(BigInteger n)
        {
            int amount = 0;
            for (int k = 1; k < n + 1; k++)
            {
                if (GCD(k, n) == 1)
                    amount++;
            }
            return amount;
        }


        public static bool IsCoprime(BigInteger n, BigInteger m)
        {
            return GCD(n, m) == 1;
        }

        public static BigInteger Ord(BigInteger n, BigInteger a)
        {
            // Console.WriteLine($"The modulus is {n}, the base is {a}");
            if (!IsCoprime(n, a))
            {
                throw new ArgumentException("The two inputs must be coprime");
            }
            if (n <= 1)
            {
                throw new ArgumentException("The `n` input must be 2 or greater");
            }
            BigInteger k = 1;
            while (true)
            {
                // Console.WriteLine($"IN ORD, trying k={k}, where the result is {BigInteger.ModPow(a, k, n)}");
                if (BigInteger.ModPow(a, k, n) == 1)
                {
                    return k;
                }
                k++;
            }
        }
        
        // Used in the AKS Primality test
        public static long FindR(BigInteger n, bool oldImplementation=true)
        {
            if (oldImplementation)
            {
                double maxK = BigInteger.Log(n, 2) * BigInteger.Log(n, 2);
                bool nextR = true;
                long r = 1;

                while (nextR)
                {
                    if (GCD(r, n) != 1)
                    {
                        r++;
                        continue;
                    }
                    r++;

                    nextR = false;
                    long k = 0;
                    while (k <= maxK && !nextR)
                    {
                        k++;
                        BigInteger powerResult = Power(n, k, r);
                        if (powerResult == 0 || powerResult == 1)
                            nextR = true;

                    }

                }
                return r;
            }
            else
            {
                long r = 2;
                double lowerBound = BigInteger.Log(n, 2) * BigInteger.Log(n, 2);
                // Console.WriteLine($"The lower bound: {lowerBound}");
                while (true)
                {
                    // Console.WriteLine(r);
                    if (!IsCoprime(n, r))
                    {
                        r++;
                        continue;
                    }
                    if ((long)Ord(n: n, a: r) > lowerBound)
                    {
                        return r;
                    }
                    // Console.WriteLine($"Tested r={r}. Did not pass because {(long)Ord(n: n, a: r)} > {lowerBound}");
                    r++;
                }
            }
            
            
        }

       


        /// Checks if a positive BigInteger n is a perfect power, i.e., if there exist integers a > 1 and b > 1 such that n = a^b.
        /// Returns true if it is a perfect power, false otherwise.
        /// Handles n <= 1 as not perfect powers (as per standard definition excluding trivial cases).
        public static bool IsPerfectPower(BigInteger n)
        {
            if (n <= 1) return false;

            // Special case for powers of 2 (BigInteger has built-in support)
            if (n.IsPowerOfTwo) return true;

            // Determine the maximum exponent to check: floor(log2(n))
            int maxExponent = GetLog2(n);

            // Check for each exponent b from 2 to maxExponent
            for (int b = 2; b <= maxExponent; b++)
            {
                // Compute the b-th root using binary search
                BigInteger root = FindIntegerRoot(n, b);
                if (root != BigInteger.Zero && BigInteger.Pow(root, b) == n)
                {
                    return true;
                }
            }

            return false;
        }

        /// Computes floor(log2(n)) for n > 0.
        public static int GetLog2(BigInteger n)
        {
            // Manual bit length calculation - works on all .NET versions
            if (n <= 0)
                throw new ArgumentOutOfRangeException(nameof(n));

            // Count bits by shifting
            int length = 0;
            BigInteger temp = n;
            while (temp > BigInteger.Zero)
            {
                temp >>= 1;
                length++;
            }
            return length - 1; // This is floor(log2(n))

        }
        /// Finds the largest integer r such that r^b <= n using binary search.
        /// Returns r if r^b == n exactly, otherwise zero to indicate no exact match.
        public static BigInteger FindIntegerRoot(BigInteger n, int exponent)
        {
            BigInteger low = BigInteger.One;
            BigInteger high = BigInteger.Pow(2, (GetLog2(n) / exponent) + 2); // Safe upper bound

            // Expand high if necessary (rare)
            while (BigInteger.Pow(high, exponent) <= n)
            {
                high = high * 2;
            }

            BigInteger result = BigInteger.Zero;

            while (low <= high)
            {
                BigInteger mid = low + (high - low) / 2;
                BigInteger midPow = BigInteger.Pow(mid, exponent);

                if (midPow == n)
                {
                    return mid; // Exact match
                }
                else if (midPow < n)
                {
                    result = mid;
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return result;
        }
    }

}
