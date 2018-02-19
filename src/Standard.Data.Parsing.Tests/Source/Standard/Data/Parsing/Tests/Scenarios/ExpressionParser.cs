using System;
using System.Linq.Expressions;
using Xunit;
using Standard.Data.Parsing;

namespace Standard.Data.Parsing.Tests
{
    public static class ExpressionParser
    {
        public static Expression<Func<double>> Eval(string text)
        {
            return Lambda.Parse(text);
        }

        private static Parser<ExpressionType> Operator(string op, ExpressionType opType)
        {
            return Parse.String(op).Token().Return(opType);
        }

        private static readonly Parser<ExpressionType> Add = Operator("+", ExpressionType.AddChecked);
        private static readonly Parser<ExpressionType> Subtract = Operator("-", ExpressionType.SubtractChecked);
        private static readonly Parser<ExpressionType> Multiply = Operator("*", ExpressionType.MultiplyChecked);
        private static readonly Parser<ExpressionType> Divide = Operator("/", ExpressionType.Divide);

        private static readonly Parser<Expression> Constant = Parse
			.Decimal
            .Select(x => Expression.Constant(double.Parse(x)))
            .Named("number");

        private static readonly Parser<Expression> Factor = 
            (
                from lparen in Parse.Char('(')
                from expr in Parse.Ref(() => Expr)
                from rparen in Parse.Char(')')
                select expr
            ).Named("expression")
            .XOr(Constant);

        private static readonly Parser<Expression> Operand =
            (
                (
                    from sign in Parse.Char('-')
                    from factor in Factor
                    select Expression.Negate(factor)
                ).XOr(Factor)
            ).Token();

        private static readonly Parser<Expression> Term = Parse.XChainOperator(Multiply.XOr(Divide), Operand, Expression.MakeBinary);

        private static readonly Parser<Expression> Expr = Parse.XChainOperator(Add.XOr(Subtract), Term, Expression.MakeBinary);

        private static readonly Parser<Expression<Func<double>>> Lambda = Expr
			.End()
			.Select(body => Expression.Lambda<Func<double>>(body));
    }
}
