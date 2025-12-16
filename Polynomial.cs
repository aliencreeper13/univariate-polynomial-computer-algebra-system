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
    internal class Polynomial
    {
        private Dictionary<BigInteger, Monomial> terms = new Dictionary<BigInteger, Monomial>();
        public Polynomial(params Monomial[] monomials)
        {
            // this.terms.Clear();
            this.AddInPlace(Monomial.Zero);
            foreach (Monomial monomial in monomials)
            {
                this.AddInPlace((Monomial)monomial);
            }

        }
        public static Polynomial Parse(Monomial monomial)
        {
            return new Polynomial(monomial);
        }
        public static readonly Polynomial Zero = new Polynomial();
        public static readonly Polynomial One = new Polynomial(new Monomial(new BigInteger(1), new BigInteger(0)));
        public static readonly Polynomial BasicBinomial = new Polynomial(new Monomial(1, 1), Monomial.One);
        public static readonly Polynomial BasicBinomialMinusOne = new Polynomial(new Monomial(1, 1), Monomial.MinusOne);

        public bool IsZero()
        {
            bool answer = true;
            foreach (Monomial monomial in this.GetAllTerms())
            {
                if (!monomial.IsZero())
                    answer = false;
            }
            return answer;
        }
        public static Polynomial BasicMultinomial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Must be non-negative");
            }
            Polynomial multinomial = new Polynomial();
            for (int i = 0; i < n; i++)
            {
                multinomial.AddInPlace(new Monomial(1, i));
            }
            return multinomial;
        }
        public override string ToString()
        {
            bool onFirstTerm = true; // this variable is used to ensure that we don't include a '+' sign at the beginning
            if (this.IsZero())
                return "0";

            string output = "";

            foreach (Monomial monomial in this.GetNonzeroTerms())
            {
                if (monomial.IsNegative())
                    output += " - " + Monomial.Abs(monomial).ToString();
                else
                {
                    if (onFirstTerm)
                    {
                        output += monomial.ToString();
                        onFirstTerm = false;
                    }
                        
                    else
                        output += " + " + monomial.ToString();
                }
                    
            }
            return output;
        }
        public static Polynomial FromString(string input)
        {
            input = input.Replace("-", "+-").Replace(" ", "").Replace("(", "").Replace(")", "");
            string[] monomialStrings = input.Split(new char[] { '+', '-' }, StringSplitOptions.RemoveEmptyEntries);
            Polynomial outputPolynomial = new Polynomial();
            foreach (string monomialString in monomialStrings)
            {
                Monomial monomial = Monomial.FromString(monomialString);
                outputPolynomial.AddInPlace(monomial);
            }
            return outputPolynomial;
        }
        public void AddInPlace(BigInteger coefficient, BigInteger degree)
        {
            coefficient = Utilities.Modulo(coefficient);
            

            if (this.terms.ContainsKey(degree))
            {
                terms[degree].Coefficient += coefficient;
            }
            else
            {
                Monomial newMonomial = new Monomial(coefficient, degree);
                terms.Add(degree, newMonomial);
            }
        }
        public static Polynomial Multiply(Polynomial poly1, Polynomial poly2)
        {
            Polynomial productPolynomial = new Polynomial();
            Monomial[] poly1Terms = poly1.GetNonzeroTerms();
            Monomial[] poly2Terms = poly2.GetNonzeroTerms();
            foreach (Monomial monomial1 in poly1Terms)
            {
                foreach (Monomial monomial2 in poly2Terms)
                {
                    productPolynomial.AddInPlace(monomial1 * monomial2);
                }
            }
            return productPolynomial;
        }

        public static Polynomial Power(Polynomial polyBase, BigInteger exponent, Polynomial? polynomialMod = null, bool logs = false)
        {
            if (Globals.safetyChecks && Globals.modulus == null)
            {
                Utilities.Warn("Gloabls.modulus is currently null. Keep this in mind when expanding large polynomials or raising polynomials to large degrees");
            }
            if (exponent < 0)
            {
                throw new ArgumentException("Exponents must be non-negative");
            }
            Polynomial result = Polynomial.One;
            Polynomial baseValue = polyBase;
            if (polynomialMod != null)
            {
                result = Polynomial.PolynomialMod(result, polynomialMod);
                baseValue = Polynomial.PolynomialMod(baseValue, polynomialMod);
            }
                
            

            while (exponent > 0)
            {
                if (logs)
                    Console.WriteLine($"Current exponent: {exponent}");
                if (exponent % 2 == 1)
                {
                    result *= baseValue;
                    if (polynomialMod != null)
                        result = Polynomial.PolynomialMod(result, polynomialMod);
                }

                baseValue *= baseValue;
                if (polynomialMod != null)
                    baseValue = Polynomial.PolynomialMod(baseValue, polynomialMod);

                exponent /= 2;
            }
            return result;
        }

        public static Polynomial[] Divide(Polynomial dividend, Polynomial divisor)
        {
            Polynomial[] result = PolynomialLongDivide(dividend, divisor);
            return result;
            
        }

        public static Polynomial PolynomialMod(Polynomial poly, Polynomial polyMod)
        {
            Polynomial[] result = PolynomialLongDivide(poly, polyMod, true);
            Polynomial mod = result[1];
            return mod;
        }

        private static Polynomial?[] PolynomialLongDivide(Polynomial dividend, Polynomial divisor, bool remainderOnly=false)
        {
            Polynomial? outputQuotient = Polynomial.Zero;

            if (remainderOnly)
                outputQuotient = null;

            if (dividend.Degree < divisor.Degree)
            {
                Polynomial?[] earlyOutput = { outputQuotient, dividend };
                return earlyOutput;
            }

            Polynomial currentDividend = dividend;

            while (true)
            {
                Monomial divisorHighestDegreeTerm = divisor.HighestDegreeTerm();
                Monomial currentDividendHighestDegreeTerm = currentDividend.HighestDegreeTerm();

                /*
                Console.WriteLine($"Current dividend: {currentDividend}. Current divisor: {divisor}");
                Console.WriteLine($"{currentDividendHighestDegreeTerm} / {divisorHighestDegreeTerm}\n");
                */

                Monomial HighestDegreeTermsQuotient = currentDividendHighestDegreeTerm / divisorHighestDegreeTerm;

                currentDividend = Polynomial.Subtract(currentDividend, divisor * HighestDegreeTermsQuotient);


                if (!remainderOnly)
                    outputQuotient.AddInPlace(HighestDegreeTermsQuotient);

                if (currentDividend.Degree < divisor.Degree)
                {
                    Polynomial?[] finalOutput = { outputQuotient, currentDividend };
                    return finalOutput;
                }
            }
        }
        public static Polynomial Add(Polynomial poly1, Polynomial poly2)
        {
            Polynomial output = new Polynomial();
            foreach (Monomial monomial in poly1.GetNonzeroTerms())
            {
                output.AddInPlace(monomial);
            }
            foreach (Monomial monomial in poly2.GetNonzeroTerms())
            {
                output.AddInPlace(monomial);
            }
            return output;
        }
        public static Polynomial Subtract(Polynomial poly1, Polynomial poly2)
        {
            return Polynomial.Add(poly1, -poly2);
        }
        public static Polynomial operator *(Polynomial poly1, Polynomial poly2)
        {
            return Polynomial.Multiply(poly1, poly2);
        }
        public static Polynomial operator +(Polynomial poly1, Polynomial poly2)
        {
            return Polynomial.Add(poly1, poly2);
        }
        public static Polynomial operator -(Polynomial poly1, Polynomial poly2)
        {
            return Polynomial.Subtract(poly1, poly2);
        }
        public static Polynomial operator -(Polynomial polynomial)
        {
            return Polynomial.Multiply(polynomial, Monomial.MinusOne);
        }
        public void AddInPlace(Monomial monomial)
        {
            this.AddInPlace(monomial.Coefficient, monomial.Degree);
        }

        public Polynomial Copy()
        { 
            Polynomial theCopy = new Polynomial();
            foreach (Monomial monomial in this.GetNonzeroTerms())
            {
                theCopy.AddInPlace(monomial);
            }
            return theCopy;
        }

        // returns an array of all terms in the polynomial, including those whose coefficient is 0
        public Monomial[] GetAllTerms()
        {
            List<Monomial> termsAsList = new List<Monomial> (this.terms.Values);
            termsAsList.Sort((x, y) => y.Degree.CompareTo(x.Degree));

            return termsAsList.ToArray();
        }
        
        // returns an array of all nonzero terms in the polynomial
        public Monomial[] GetNonzeroTerms()
        { 
            Monomial[] allTerms= this.GetAllTerms();
            Monomial[] nonzeroTerms = allTerms.Where(monomial => monomial.Coefficient != 0).ToArray();
            if (nonzeroTerms.Length == 0 )
            {
                return new Monomial[] { Monomial.Zero };
            }
            return nonzeroTerms;
        }
        
        // returns an array of all coefficients in the polynomial, including the zeros
        public BigInteger[] GetAllCoefficients()
        {
            Monomial[] allTerms = this.GetAllTerms();
            BigInteger[] allCoefficients = new BigInteger[allTerms.Length];
            for (int i = 0; i < allTerms.Length; i++)
            {
                allCoefficients[i] = allTerms[i].Coefficient;
            }
            return allCoefficients;
        } 

        // returns an array of all nonzero coefficients in the polynomial
        public BigInteger[] GetCoefficients()
        {
            Monomial[] terms = this.GetNonzeroTerms();
            BigInteger[] coefficients = new BigInteger[terms.Length];
            for (int i = 0; i < terms.Length; i++)
            {
                coefficients[i] = terms[i].Coefficient;
            }
            return coefficients;
        }
        public BigInteger SumOfCoefficients(bool applyModulo = true)
        {
            BigInteger sum = 0;
            foreach (BigInteger coefficient in this.GetCoefficients())
            {
                sum += coefficient;
            }
            if (applyModulo)
            {
                sum = Utilities.Modulo(sum);
            }
            return sum;
        }
        public Monomial MonomialAtDegree(BigInteger degree)
        {
            if (!this.terms.ContainsKey(degree))
                return Monomial.Zero;
            return this.terms[degree];
        }
        public BigInteger Degree
        {
            get { 
                return this.GetNonzeroTerms()[0].Degree;
                }
        }
        public Monomial HighestDegreeTerm()
        {
            return this.MonomialAtDegree(this.Degree);
        }

        public bool Equals(Polynomial p)
        {
            for (BigInteger d = 0; d <= BigInteger.Max(this.Degree, p.Degree); d++)
            {
                Monomial m1 = this.MonomialAtDegree(d);
                Monomial m2 = p.MonomialAtDegree(d);
                if (!m1.Equals(m2))
                    return false;
            }
            return true;
        }
        public bool Equals(Monomial m)
        {
            return this.Equals(new Polynomial(m));
        }
    }
}
