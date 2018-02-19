using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Standard.Data.Parsing;

namespace XmlParsing
{
    public class Document
    {
        public Node Root;

        public override string ToString()
        {
            return Root.ToString();
        }
    }

    public class Item 
    { }

    public class Content : Item
    {
        public string Text;

        public override string ToString()
        {
            return Text;
        }
    }

    public class Node : Item
    {
        public string Name;

        public IEnumerable<Item> Children;

        public override string ToString()
        {
            if (Children != null)
            {
                return string.Format("<{0}>", Name) +
                    Children.Aggregate("", (s, c) => s + c) +
                    string.Format("</{0}>", Name);
            }

            return string.Format("<{0}/>", Name);
        }
    }

    public static class XmlParser
    {
        private static CommentParser Comment = new CommentParser("<!--", "-->", "\r\n");

        private static readonly Parser<string> Identifier =
            from first in Parse.Letter.Once()
            from rest in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many()
            select new string(first.Concat(rest).ToArray());

        private static Parser<T> Tag<T>(Parser<T> content)
        {
            return 
                from lt in Parse.Char('<')
                from t in content
                from gt in Parse.Char('>').Token()
                select t;
        }

        private static readonly Parser<string> BeginTag = Tag(Identifier);

        private static Parser<string> EndTag(string name)
        {
            return Tag(
                from slash in Parse.Char('/')
                from id in Identifier
                where id == name
                select id).Named("closing tag for " + name);
        }

        private static readonly Parser<Content> Content =
            from chars in Parse.CharExcept('<').Many()
            select new Content { Text = new string(chars.ToArray()) };

        private static readonly Parser<Node> FullNode =
            from tag in BeginTag
            from nodes in Parse.Ref(() => Item).Many()
            from end in EndTag(tag)
            select new Node { Name = tag, Children = nodes };

        private static readonly Parser<Node> ShortNode = Tag(
            from id in Identifier
            from slash in Parse.Char('/')
            select new Node { Name = id });

        private static readonly Parser<Node> Node = ShortNode.Or(FullNode);

        private static readonly Parser<Item> Item =
            from leading in Comment.MultiLineComment.Many()
            from item in Node.Select(n => (Item)n).XOr(Content)
            from trailing in Comment.MultiLineComment.Many()
            select item;

        private static readonly Parser<Document> Document =
            from leading in Parse.WhiteSpace.Many()
            from doc in Node.Select(n => new Document { Root = n }).End()
            select doc;

        public static Document Eval(string input)
        {
            return Document.Parse(input);
        }
    }
}
