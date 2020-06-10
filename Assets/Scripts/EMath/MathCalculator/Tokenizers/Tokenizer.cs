using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Assets.Scripts.EMath.MathCalculator.Tokenizers
{
    class Tokenizer
    {
        public Token currentToken { get; private set; }
        public double number { get; private set; }
        public string identifier { get; private set; }

        private readonly TextReader reader;
        private readonly Queue<char> extra;
        private char currentChar;

        public Tokenizer(TextReader reader)
        {
            this.reader = reader;
            this.extra = new Queue<char>();

            NextChar();
            NextToken();
        }

        public void NextToken()
        {
            while (char.IsWhiteSpace(currentChar))
            {
                NextChar();
            }

            switch (currentChar)
            {
                case '\0':
                    currentToken = Token.EOF;
                    return;
                case '+':
                    NextChar();
                    currentToken = Token.Add;
                    return;
                case '-':
                    NextChar();
                    currentToken = Token.Subtract;
                    return;
                case '*':
                    NextChar();
                    currentToken = Token.Multiply;
                    return;
                case '/':
                    NextChar();
                    currentToken = Token.Divide;
                    return;
                case '^':
                    NextChar();
                    currentToken = Token.Exponentiation;
                    return;
                case '(':
                    NextChar();
                    currentToken = Token.OpenParens;
                    return;
                case ')':
                    NextChar();
                    currentToken = Token.CloseParens;
                    return;
                case ',':
                    NextChar();
                    currentToken = Token.Comma;
                    return;
            }

            if (char.IsDigit(currentChar) || currentChar == '.')
            {
                var sb = new StringBuilder();
                bool haveDecimalPoint = false;

                while (true)
                {
                    if (isNumerical(haveDecimalPoint))
                    {
                        sb.Append(currentChar);
                        haveDecimalPoint = currentChar == '.';
                        NextChar();
                    }
                    else if (isName())
                    {
                        extra.Enqueue(currentChar);
                        currentChar = '*';
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                number = double.Parse(sb.ToString(), CultureInfo.InvariantCulture);
                currentToken = Token.Number;

                return;
            }

            if (isName())
            {
                var sb = new StringBuilder();

                while (isAlphaNumeric())
                {
                    sb.Append(currentChar);
                    NextChar();
                }

                identifier = sb.ToString();
                currentToken = Token.Identifier;
            }
        }

        private bool isNumerical(bool haveDecimalPoint)
        {
            return char.IsDigit(currentChar) || (!haveDecimalPoint && currentChar == '.');
        }

        private bool isName()
        {
            return char.IsLetter(currentChar) || currentChar == '_';
        }

        private bool isAlphaNumeric()
        {
            return char.IsLetterOrDigit(currentChar) || currentChar == '_';
        }

        private void NextChar()
        {
            currentChar = ReadChar();
        }

        private char ReadChar()
        {
            if (extra.Count > 0)
            {
                return extra.Dequeue();
            }

            int ch = reader.Read();
            return ch < 0 ? '\0' : (char)ch;
        }
    }
}
