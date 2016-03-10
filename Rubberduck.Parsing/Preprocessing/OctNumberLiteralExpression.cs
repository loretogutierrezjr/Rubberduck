﻿using System;

namespace Rubberduck.Parsing.Preprocessing
{
    public sealed class OctNumberLiteralExpression : Expression
    {
        private readonly IExpression _tokenText;

        public OctNumberLiteralExpression(IExpression tokenText)
        {
            _tokenText = tokenText;
        }

        public override IValue Evaluate()
        {
            string literal = _tokenText.Evaluate().AsString;
            literal = literal.Replace("&O", "").Replace("&", "");
            var number = (decimal)Convert.ToInt32(literal, 8);
            return new DecimalValue(number);
        }
    }
}
