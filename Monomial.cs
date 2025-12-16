using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Polylib
{
    internal class Monomial
    {
        private BigInteger degree;
        private BigInteger coefficient;
        public BigInteger Degree
        {
            get { return this.degree; }
            set { this.degree = value; }
        }
        public BigInteger Coefficient
        {
            get { return this.coefficient; }
            set {
                this.SetCoefficient(value);
                // this.coefficient = Utilities.Modulo(value); 
            }
        }
        public void SetCoefficient(BigInteger value)
        { this.coefficient = Utilities.Modulo(value); }

        public Monomial(BigInteger coefficient, BigInteger degree) 
        {
            this.degree = degree;
            this.coefficient = Utilities.Modulo(coefficient); // remember, if Globals.modulus is null, then nothing happens
        }
        public Monomial(BigInteger coefficient)
        {
            this.coefficient = Utilities.Modulo(coefficient);
            this.degree = new BigInteger(0);
        }

        public static Monomial Multiply(Monomial a, Monomial b)
        {
            return new Monomial(a.coefficient * b.coefficient, a.degree + b.degree);
        }

        public static Monomial Divide(Monomial a, Monomial b)
        {
            return new Monomial(Utilities.Divide(a.coefficient, b.coefficient), a.degree - b.degree);
        }

        public static Polynomial Add(Monomial a, Monomial b)
        {
            return new Polynomial(a, b);
        }
        public static Polynomial Subtract(Monomial a, Monomial b)
        {
            return new Polynomial(a, -b);
        }

        // Multiplication
        public static Monomial operator *(Monomial a, Monomial b)
        {
            return Monomial.Multiply(a, b);
        }

        // Division
        public static Monomial operator /(Monomial a, Monomial b)
        {
            return Monomial.Divide(a, b);
        }
        
        public static Polynomial operator +(Monomial a, Monomial b)
        {
            return Monomial.Add(a, b);
        }
        public static Polynomial operator -(Monomial a, Monomial b)
        {
            return Monomial.Subtract(a, b);
        }

        // Absolute value
        public static Monomial Abs(Monomial a)
        {
            return new Monomial(BigInteger.Abs(a.coefficient), a.degree);
        }

        // Absolute value
        public static Monomial operator -(Monomial a)
        {
            return new Monomial(-a.coefficient, a.degree);
        }

        // implicitly convert Monomials to Polynomials when necessary
        public static implicit operator Polynomial(Monomial monomial)
        {
            Polynomial polynomial = new Polynomial();
            polynomial.AddInPlace(monomial);
            return polynomial;
        }

        // implicitly convert BigIntegers to Monomials when necessary
        public static implicit operator Monomial(BigInteger bigInteger)
        {
            return Monomial.Constant(bigInteger);
        }

        public static implicit operator Monomial(long integer)
        {
            return Monomial.Constant(integer);
        }

        public static implicit operator Monomial(int integer)
        {
            return Monomial.Constant(integer);
        }

        public static implicit operator Monomial(long[] coeffDegree)
        {
            return Monomial.Parse(coeffDegree);
        }

        public static implicit operator Monomial(int[] coeffDegree)
        {
            return Monomial.Parse(coeffDegree);
        }

        public static Monomial Constant(BigInteger c)
        {
            return new Monomial(c, new BigInteger(0));
        }
        public static Monomial FromString(string input)
        {
            string str = input.Replace("**", "^").Replace("*", "").Replace("+", "");

            if (str.Count(c => c == '^') > 1)
            {
                throw new ArgumentException("Maximum of one exponentiation operator per monomial permitted");
            }
            if (str.Count(c => c == '.') > 0)
            {
                throw new NotImplementedException("Floating point numbers are currently not allowed");
            }
            else if (str.Count(c => c == '^') == 0)
            {
                bool hasSymbol = false;
                foreach (char c in str)
                {
                    if (char.IsLetter(c))
                    {
                        hasSymbol = true;
                    }
                }
                if (hasSymbol)
                {
                    str += "^1";
                }
                else
                {
                    str += "^0";
                }
            }

            /*
            if (str.Count(c => c == '.') > 1)
            {
                throw new ArgumentException("Maximum of one decimal point per monomial permitted");
            }
            */

            string[] parts = str.Split('^');
            string strCoefficientWithVariable = parts[0];
            string strDegree = parts[1];

            string strCoefficient = "";
            foreach (char c in strCoefficientWithVariable)
            {
                if (char.IsDigit(c) || c == '.' || c == '-')
                {
                    strCoefficient += c;
                }
            }

            // if the string representation of the coefficient is absent, then assume a coefficient of 1
            if (strCoefficient.Length == 0)
            {
                strCoefficient = "1";
            }
            // if the string rperesentation is not empty, but the first character is not a number, nor a negative sign, then again, assume a coefficient of 1
            else if (!char.IsDigit(strCoefficient[0]))
            {
                if (strCoefficient[0] != '-')
                {
                    strCoefficient = "1" + strCoefficient;
                }
            }


            // if the coefficient is just a negative sign, then the coefficient is just negative 1
            if (strCoefficient == "-")
            {
                strCoefficient = "-1";
            }

            BigInteger coefficient = BigInteger.Parse(strCoefficient);
            BigInteger degree = BigInteger.Parse(strDegree);
            return new Monomial(coefficient, degree);
            
        }
        public static Monomial Parse(string str)
        {
            return Monomial.FromString(str);
        }
        public static Monomial Parse(long integer)
        {
            return Monomial.Constant(integer);
        }
        public static Monomial Parse(int integer)
        {
            return Monomial.Constant(integer);
        }
        public static Monomial Parse(long[] coeffDegree)
        {
            return new Monomial(coeffDegree[0], coeffDegree[1]);
        }
        public static Monomial Parse(int[] coeffDegree)
        {
            return new Monomial(coeffDegree[0], coeffDegree[1]);
        }
        public Monomial Copy()
        {
            return new Monomial(this.coefficient, this.degree);
        }
        public bool IsZero()
        {
            return this.coefficient == 0;
        }
        public bool IsNegative()
        { 
            return this.coefficient < 0;
        }
        public bool IsConstant()
        {
            return this.degree == 0;
        }
        public override string ToString()
        {
            if (this.coefficient == 0)
            {
                return "0";
            }

            if (this.degree == 0)
            {
                return $"{this.coefficient}";
            }

            string output = "";
            if (this.degree == 1)
                output = $"{Globals.symbol}";
            else
                output = $"{Globals.symbol}^{this.degree}";

            if (this.coefficient != 1)
                output = $"{this.coefficient}" + output;

            return output;
        }

        public bool Equals(Monomial m)
        {
            if (this.Coefficient != m.Coefficient)
                return false;
            if (this.Degree != m.Degree)
                return false;
            return true;
        }



        public static readonly Monomial One = Monomial.Constant(1);
        public static readonly Monomial Zero = Monomial.Constant(0);
        public static readonly Monomial MinusOne = Monomial.Constant(-1);

    }
}
