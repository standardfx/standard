using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Standard.Data.Parsing;

namespace Standard.Data.Parsing.Tests
{
	public static class AssemblerParser
	{
		public static Parser<string> Identifier = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

		public static Parser<T> AsmToken<T>(Parser<T> parser)
		{
			var whitespace = Parse.WhiteSpace.Except(Parse.LineEnd);
			return
				from leading in whitespace.Many()
				from item in parser
				from trailing in whitespace.Many()
				select item;
		}

		public static Parser<Tuple<string, string[]>> Instruction =
			from instructionName in AsmToken(Parse.LetterOrDigit.Many().Text())
			from operands in AsmToken(Identifier).XDelimitedBy(Parse.Char(','))
			select Tuple.Create(instructionName, operands.ToArray());

		public static CommentParser Comment = new CommentParser { Single = ";", NewLine = Environment.NewLine };

		public static Parser<string> LabelId =
			Parse.Identifier(Parse.Letter.Or(Parse.Chars("._?")), Parse.LetterOrDigit.Or(Parse.Chars("_@#$~.?")));

		public static Parser<string> Label =
			from labelName in AsmToken(LabelId)
			from colon in AsmToken(Parse.Char(':'))
			select labelName;

		public static readonly Parser<IEnumerable<AssemblerLine>> Assembler = (
			from label in Label.Optional()
			from instruction in Instruction.Optional()
			from comment in AsmToken(Comment.SingleLineComment).Optional()
			from lineTerminator in Parse.LineTerminator
			select new AssemblerLine(
				label.GetOrDefault(),
				instruction.IsEmpty ? null : instruction.Get().Item1,
				instruction.IsEmpty ? null : instruction.Get().Item2,
				comment.GetOrDefault())
			).XMany().End();
	}

	public class AssemblerLine
	{
		public readonly string Label;
		public readonly string InstructionName;
		public readonly string[] Operands;
		public readonly string Comment;

		public AssemblerLine(string label, string instructionName, string[] operands, string comment)
		{
			Label = label;
			InstructionName = instructionName;
			Operands = operands;
			Comment = comment;
		}

		protected bool Equals(AssemblerLine other)
		{
			return ToString() == other.ToString();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;

			return Equals((AssemblerLine)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Label != null ? Label.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (InstructionName != null ? InstructionName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Operands != null ? Operands.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Comment != null ? Comment.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override string ToString()
		{
			return string.Join(" ",
				Label == null ? string.Empty : (Label + ":"),
				InstructionName == null ? string.Empty : InstructionName + string.Join(", ", Operands),
				Comment == null ? string.Empty : ";" + Comment);
		}
	}
}
