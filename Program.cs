using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;

namespace Polylib
{

    internal class Program
    {

        

        // the coefficients of each printed polynomial are the entries of a row in Pascal's triangle 
        static void ExpandBinomialToIncreasinglyHigherPowers(int numRows)
        {
            var binomial = Polynomial.FromString("x + 1");
            Console.WriteLine("Expansion of (x + 1) to increasingly higher powers");
            for (int n = 0; n < numRows; n++)
            {
                var expansion = Polynomial.Power(binomial, n);
                Console.WriteLine($"({binomial})^{n} = {expansion}");
            }
        }
        static void InteractiveSession()
        {
            string manual = "~~Arguments in <angle brackets> are required. Arguments in [square brackets] are optional~~\n" +
                "Expanding polynomials to some power: expand, <polynomial>, <nonnegative integer>, [polynomial mod]\n" +
                "Apply polynomial modulus to polynomial: polymod, <polynomial>, <polynomial mod>\n" +
                "Setting the global modulus: mod, <positive integer>\n" +
                "Removing the global modulus: remove mod\n" +
                "Exiting: exit\n" +
                "Exiting: end\n" +
                "View examples of various commands: examples\n" + 
                "Recall the list of commands: commands";
            Console.WriteLine("Start of session. Type 'exit' or 'end' to end session.");
            Console.WriteLine("Commands:\n" + manual);
            while (true)
            {
                Console.Write(">>");
                string? userInput = Console.ReadLine();
                if (userInput == null)
                    continue;
                userInput = userInput.ToLower();
                if (userInput.StartsWith("mod"))
                {
                    userInput = userInput.Replace(",", "");
                    // Extract the modulus value from the user input
                    string[] inputParts = userInput.Split(' ');
                    if (inputParts.Length == 2 && BigInteger.TryParse(inputParts[1], out BigInteger modulus))
                    {
                        // Set the global modulus
                        if (modulus > 1)
                        {
                            Globals.modulus = modulus;
                            Console.WriteLine($"Global modulus set to {Globals.modulus}");
                        }
                        else
                        {
                            Console.WriteLine("Modulus must be greater than 1.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input format. Please enter 'mod n', where n is a positive integer greater than 1.");
                    }
                }
                if (userInput == "remove mod")
                {
                    Globals.modulus = null;
                }
                if (userInput.StartsWith("expand"))
                {
                    string[] inputParts = userInput.Split(',');
                    if (inputParts.Length >= 3)
                    {
                        try
                        {
                            Polynomial polynomial = Polynomial.FromString(inputParts[1]);
                            BigInteger power = BigInteger.Parse(inputParts[2]);
                            if (inputParts.Length > 3)
                            {
                                Polynomial polyMod = Polynomial.FromString(inputParts[3]);
                                Polynomial expansion = Polynomial.Power(polynomial, power, polyMod);
                                Console.WriteLine(expansion);
                            }
                            else
                            {
                                Polynomial expansion = Polynomial.Power(polynomial, power);
                                Console.WriteLine(expansion);
                            }
                        }
                        catch (Exception ex) { Console.WriteLine(
                            "An error occurred. Try checking the syntax of the polynomial and try again. Error message: " + ex.ToString()); }
                    }
                    else
                    {
                        Console.WriteLine("Not enough arguments!");
                    }
                }
                if (userInput.StartsWith("polymod"))
                {
                    string[] inputParts = userInput.Split(',');
                    if (inputParts.Length >= 2 ) 
                    {
                        try
                        {
                            Polynomial polynomial = Polynomial.FromString(inputParts[1]);
                            Polynomial polyMod = Polynomial.FromString(inputParts[2]);
                            Polynomial output = Polynomial.PolynomialMod(polynomial, polyMod);
                            Console.WriteLine(output);
                        }
                        catch (Exception ex) { Console.WriteLine("An error occurred. Try checking the syntax of the input and try again. Error message: " + ex.ToString()); }
                            
                        
                    }
                    else
                    {
                        Console.WriteLine("Not enough arguments!");
                    }
                }
                if (userInput == "commands")
                {
                    Console.WriteLine(manual);
                }
                if (userInput == "examples")
                {
                    string examples = "expand, x + 1, 5\nx^3 + 3x^2 + 3x + 1\n\n" +
                                      "expand, x + 1, 11, x^4 - 1\n- 164x^3 - 396x^2 - 396x - 164\n\n" +
                                      "polymod, 5x^9 + 3x^8 + 7x^3, x^2 - 1\n-2x3\n\n" +
                                      "mod, 17\nGlobal modulus set to 17\n\n";

                    Console.WriteLine(examples);
                }
                if (userInput.Contains("exit") || userInput.Contains("end"))
                {
                    break;
                }
                

            }
        }
        
        static void Main(string[] args)
        {
            Globals.safetyChecks = true;

            
            // Console.WriteLine(AKS.AKSPrimalityTest(BigInteger.Parse("244")));
            // ExpandBinomialToIncreasinglyHigherPowers(10);
            InteractiveSession();


        }
    }
}