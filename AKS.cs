using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Polylib
{
    internal class AKS
    {
        // Used in the final step of the AKS primality test
        static Polynomial AKSStep(BigInteger N, int a, int r)
        {
            var binomial = new Polynomial(new Monomial(1, 1), Monomial.Constant(a)); // (x + a)
            var polyMod = new Polynomial(new Monomial(1, r), Monomial.MinusOne); // (x^r - 1)
            Globals.modulus = N;

            var expansion = Polynomial.Power(binomial, N, polyMod); // (x + a)^n (mod x^r - 1, N)
            return expansion;
        }

        /// <summary>
        /// This is a famous primality test that is known to *always* yield the correct answer in polynomial runtime.
        /// However, despite being in polynomial time, it is still notoriously slow, and isn't used in practice. It is
        /// merely an algorithmic curiosity (it could be described as a galactic algorithm).
        /// See https://en.wikipedia.org/wiki/AKS_primality_test for how I implemented this algorithm.
        /// </summary>
        /// <param name="n">The integer to check for primality</param>
        /// <returns>True if n is prime, n if otherwise</returns>
        public static bool AKSPrimalityTest(BigInteger n)
        {
            if (n <= 1) return false; 
            // Step one: Determine if integer is perfect power
            if (Utilities.IsPerfectPower(n)) {
                Console.WriteLine("Determined to be composite by step 1");
                return false;
            }

            // Step two: Find an r
            BigInteger r = Utilities.FindR(n);
            Console.WriteLine($"r: {r}");
            if (!Utilities.IsCoprime(n, r)) {
                Console.WriteLine("Determined to be composite by step 2");
                return false;
            }

            // Step three
            for (BigInteger a = 2; a <= BigInteger.Min(r, n - 1); a++)
            {
                if (a % n == 0)
                { 
                    Console.WriteLine("Determined to be composite by step 3"); 
                    return false; 
                }
            }
            
            // Step four
            if (n <= r)
            { 
                Console.WriteLine("Determined to be prime by step 4"); 
                return true; 
            }

            // Step five
            double upperBound = Math.Floor(Math.Sqrt(Utilities.Totient(r)) * Utilities.GetLog2(n));
            // Console.WriteLine($"Checking to {upperBound}");
            for (int a = 1; a <= upperBound; a++)
            {
                if (a % 50 == 1)
                {
                    Console.WriteLine($"Checking a={a}");
                }
                Polynomial expansion = AKSStep(n, a, (int)r);

                if (!expansion.Equals(new Polynomial(new Monomial(1, n % r), Monomial.Constant(a)))) // if (X + a)^n != X^n + a (mod X^r - 1, n), return composite
                { 
                    Console.WriteLine($"Determined to be composite by step 5. a = {a}");
                    // Console.WriteLine($"Expansion: {expansion}");
                    return false; 
                }
               


            }

            Console.WriteLine("Determined to be prime by surpassing all steps");
            return true;
        }

    }
}
