using Assets.Scripts.EMath.MathCalculator.Exceptions;
using Assets.Scripts.EMath.MathCalculator.Parsers.Node;
using Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context;
using Assets.Scripts.EMath.MathCalculator.Tokenizers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.EMath.MathCalculator.Parsers
{
    class Parser
    {
        private const double divisionLimit = 0.00000000001;

        private readonly Tokenizer tokenizer;
        private readonly INode evaluatedNode;

        public ISet<string> variables { get; }

        public Parser(string str)
        {
            tokenizer = new Tokenizer(new StringReader(str));
            variables = new HashSet<string>();

            evaluatedNode = Parse();
        }

        public double Eval()
        {
            return evaluatedNode.Eval(null);
        }

        public double Eval(Dictionary<string, double> vars)
        {
            return evaluatedNode.Eval(new DictionaryContext(vars));
        }

        private INode Parse()
        {
            var expr = ParseAddSubtract();

            if (tokenizer.currentToken != Token.EOF)
            {
                throw new SyntaxException("La expresión no es válida");
            }

            return expr;
        }

        private INode ParseAddSubtract()
        {
            var lhs = ParseMultiplyDivide();

            while (true)
            {
                Func<double, double, double> op = null;

                if (tokenizer.currentToken == Token.Add)
                {
                    op = (a, b) => a + b;
                }
                else if (tokenizer.currentToken == Token.Subtract)
                {
                    op = (a, b) => a - b;
                }

                if (op == null)
                {
                    return lhs;
                }

                tokenizer.NextToken();
                var rhs = ParseMultiplyDivide();

                lhs = new NodeBinary(lhs, rhs, op);
            }
        }

        private INode ParseMultiplyDivide()
        {
            var lhs = ParseExponentiation();

            while (true)
            {
                Func<double, double, double> op = null;

                if (tokenizer.currentToken == Token.Multiply)
                {
                    op = (a, b) => a * b;
                }
                else if (tokenizer.currentToken == Token.Divide)
                {
                    op = (a, b) => Math.Abs(b) > divisionLimit ? a / b : double.NaN;
                }

                if (op == null)
                {
                    return lhs;
                }

                tokenizer.NextToken();
                var rhs = ParseExponentiation();

                lhs = new NodeBinary(lhs, rhs, op);
            }
        }

        private INode ParseExponentiation()
        {
            var lhs = ParseUnary();

            while (true)
            {
                Func<double, double, double> op = null;

                if (tokenizer.currentToken == Token.Exponentiation)
                {
                    op = (a, b) => Math.Pow(a, b);
                }

                if (op == null)
                {
                    return lhs;
                }

                tokenizer.NextToken();
                var rhs = ParseUnary();

                lhs = new NodeBinary(lhs, rhs, op);
            }
        }

        private INode ParseUnary()
        {
            while (true)
            {
                if (tokenizer.currentToken == Token.Add)
                {
                    tokenizer.NextToken();
                    continue;
                }

                if (tokenizer.currentToken == Token.Subtract)
                {
                    tokenizer.NextToken();
                    var rhs = ParseUnary();

                    return new NodeUnary(rhs, (a) => -a);
                }

                return ParseLeaf();
            }
        }

        private INode ParseLeaf()
        {
            if (tokenizer.currentToken == Token.Number)
            {
                var node = new NodeNumber(tokenizer.number);
                tokenizer.NextToken();

                return node;
            }

            if (tokenizer.currentToken == Token.OpenParens)
            {
                tokenizer.NextToken();
                var node = ParseAddSubtract();

                if (tokenizer.currentToken != Token.CloseParens)
                {
                    throw new SyntaxException("Falta cerrar paréntesis");
                }

                tokenizer.NextToken();

                return node;
            }

            if (tokenizer.currentToken == Token.Identifier)
            {
                var name = tokenizer.identifier;
                tokenizer.NextToken();

                if (tokenizer.currentToken != Token.OpenParens)
                {
                    NodeVariable node = new NodeVariable(name);

                    if (!node.isReserved())
                    {
                        variables.Add(name);
                    }

                    return node;
                }
                else
                {
                    tokenizer.NextToken();

                    var arguments = new List<INode>();

                    while (true)
                    {
                        arguments.Add(ParseAddSubtract());

                        if (tokenizer.currentToken == Token.Comma)
                        {
                            tokenizer.NextToken();
                            continue;
                        }

                        break;
                    }

                    if (tokenizer.currentToken != Token.CloseParens)
                    {
                        throw new SyntaxException("Falta cerrar paréntesis");
                    }

                    tokenizer.NextToken();

                    return new NodeFunctionCall(name, arguments.ToArray());
                }
            }

            throw new SyntaxException($"Cadena de texto inesperada: {tokenizer.currentToken}");
        }
    }
}
