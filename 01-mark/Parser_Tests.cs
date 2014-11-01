using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace _01_mark
{
    [TestFixture]
    class Parser_Tests
    {
        #region Replace special symbols tests
        [Test]
        public void Simple_replace_special()
        {
            string text;
            text = "\"><";
            Assert.AreEqual("&quot;&gt;&lt;", ParserToHTML.ParseSpecialSymbols(text));
        }
        [Test]
        public void Complex_replace_special()
        {
            string text;
            text = "<p> super test >>> \"<\"";
            Assert.AreEqual("&lt;p&gt; super test &gt;&gt;&gt; &quot;&lt;&quot;", ParserToHTML.ParseSpecialSymbols(text));
        }
        #endregion
        #region Correct parsing lines tests
        [Test]
        public void Simple_parsing_lines()
        {
            string text;
            text = "Just one line";
            Assert.AreEqual(new string[] { "<p>Just one line</p>" }, ParserToHTML.ParseLines(text));
        }
        [Test]
        public void Complex_parsing_lines()
        {
            string text;
            text = "Just one line\nAnd another one.    \n And some spaces \n ";
            Assert.AreEqual(new string[] { "<p>Just one line", "And another one.    ", " And some spaces ", " </p>" }, ParserToHTML.ParseLines(text));
        }
        [Test]
        public void Paragraph_divisions()
        {
            string text;
            text = "some here\n\nText\n   \n        \n \n";
            Assert.AreEqual(new string[] { "<p>some here</p>", "<p>", "Text</p>", "   <p></p>", "        <p></p>", " <p>", "</p>" }, ParserToHTML.ParseLines(text));
        }
        #endregion
    }
}
