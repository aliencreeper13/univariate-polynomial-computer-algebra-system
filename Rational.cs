using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Polylib
{
    // currently not used, may not be complete
    internal class Rational
    {
        private BigInteger numerator;
        private BigInteger denominator;
        public static readonly Rational One = new Rational(1, 1);
        public static readonly Rational Zero = new Rational(0, 1);
        public static readonly Rational MinusOne = new Rational(-1, 1);
        public BigInteger Numerator
        {
            get { return this.numerator; }
            set { this.numerator = Utilities.Modulo(value); }
        }
        public BigInteger Denominator
        {
            get { return this.denominator; }
            set
            {
                if (value == 0)
                    throw new DivideByZeroException();
                this.denominator = Utilities.Modulo(value);
            }
        }
        public Rational(BigInteger numerator, BigInteger denominator) 
        {
            this.Numerator = numerator;
            this.Denominator = denominator;
            this.Simplify();
        }
        public void Simplify()
        {


            // Take absolute values for simplification
            this.Numerator = BigInteger.Abs(this.Numerator);
            this.Denominator = BigInteger.Abs(this.Denominator);

            BigInteger gcd = BigInteger.GreatestCommonDivisor(this.Numerator, this.Denominator);
            this.Numerator /= gcd;
            this.Denominator /= gcd;

            // Adjust the sign if the rational number was negative
            if (this.isNegative())
            {
                this.Numerator = -this.Numerator;
            }
        }
        public bool isNegative()
        {
            return (this.Numerator < 0 && this.Denominator > 0) || (this.Numerator > 0 && this.Denominator < 0);
        }
        public Rational Copy()
        {
            Rational copy = new Rational(this.Numerator, this.Denominator);
            return copy;
        }
        public static Rational operator +(Rational r1, Rational r2)
        {
            // Cross multiply to find the sum
            BigInteger newNumerator = (r1.Numerator * r2.Denominator) + (r2.Numerator * r1.Denominator);
            BigInteger newDenominator = r1.Denominator * r2.Denominator;

            Rational result = new Rational(newNumerator, newDenominator);
            // result.Simplify();
            return result;
        }
        public static Rational operator -(Rational r1, Rational r2)
        {
            Rational result = r1 + (-r2);
            return result;
        }
        public static Rational operator *(Rational r1, Rational r2)
        {
            Rational product = new Rational(r1.Numerator * r2.Numerator, r1.Denominator*r2.Denominator);
            return product;
        }
        public static Rational operator /(Rational r1, Rational r2)
        {
            Rational product = new Rational(r1.Numerator * r2.Denominator, r1.Denominator * r2.Numerator);
            return product;
        }
        public static Rational operator -(Rational r)
        {
            Rational copy = r.Copy();
            if (copy.isNegative())
            {
                copy.Numerator = BigInteger.Abs(copy.Numerator);
                copy.Denominator = BigInteger.Abs(copy.Denominator);
                
            }
            else
            {
                copy.Numerator = BigInteger.Abs(copy.Numerator);
                copy.Denominator = BigInteger.Abs(copy.Denominator);
                copy.Numerator *= -1;
            }
            return copy;
                
        }
    }
}
