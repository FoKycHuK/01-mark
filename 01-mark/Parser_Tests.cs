﻿using System;
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


        #region Correct parsing backticks
        [Test]
        public void Correct_insert_codes()
        {
            var text = new string[1];
            text[0] = "uncoded, 'coded'";
            ParserToHTML.ParseBackticks(text);
            Assert.AreEqual(new string[] {"uncoded, <code>coded</code>"}, text);
        }
        [Test]
        public void Correct_handle_one_backtick()
        {
            var text = new string[1];
            text[0] = "uncoded, 'uncoded too";
            ParserToHTML.ParseBackticks(text);
            Assert.AreEqual(new string[] { "uncoded, 'uncoded too" }, text);
        }
        [Test]
        public void Correct_handle_many_lines()
        {
            var text = new string[2];
            text[0] = "uncoded, 'uncoded too";
            text[1] = "and this uncoded too'";
            ParserToHTML.ParseBackticks(text);
            Assert.AreEqual(new string[] { "uncoded, 'uncoded too", "and this uncoded too'" }, text);
        }
        [Test]
        public void Correct_handle_many_lines_and_some_backticks()
        {
            var text = new string[2];
            text[0] = "'uncoded";
            text[1] = "this 'coded'";
            ParserToHTML.ParseBackticks(text);
            Assert.AreEqual(new string[] { "'uncoded", "this <code>coded</code>" }, text);
        }
        [Test]
        public void Correct_handle_underlines()
        {
            var text = new string[1];
            text[0] = "_uncoded_, '_coded_'";
            ParserToHTML.ParseBackticks(text);
            Assert.AreEqual(new string[] { @"_uncoded_, <code>\_coded\_</code>" }, text);
        }
        [Test]
        public void Correct_handle_many_underlines()
        {
            var text = new string[1];
            text[0] = "'_this_is_it____'";
            ParserToHTML.ParseBackticks(text);
            Assert.AreEqual(new string[] { @"<code>\_this\_is\_it\_\_\_\_</code>" }, text);
        }
        #endregion
    }
}
