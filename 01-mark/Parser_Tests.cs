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
        //TODO. Регионы - зло. Их неудобно разворачивать. Регионами пользуются чтобы прятать плохой код
        #region Replace special symbols tests
        [Test]
        public void Simple_replace_special()
        {
            var text = "\"><";
            Assert.AreEqual("&quot;&gt;&lt;", ParserToHTML.ParseSpecialSymbols(text));
        }
        [Test]
        public void Complex_replace_special()
        {
            var text = "<p> super test >>> \"<\"";
            Assert.AreEqual("&lt;p&gt; super test &gt;&gt;&gt; &quot;&lt;&quot;", ParserToHTML.ParseSpecialSymbols(text));
        }
        #endregion


        #region Correct parsing lines tests
        [Test]
        public void Simple_parsing_lines()
        {
            var text = "Just one line";
            Assert.AreEqual(new string[] { "<p>Just one line</p>" }, ParserToHTML.ParseLines(text));
        }
        [Test]
        public void Complex_parsing_lines()
        {
            var text = "Just one line\nAnd another one.    \n And some spaces \n ";
            Assert.AreEqual(new string[] { "<p>Just one line", "And another one.    ", " And some spaces ", " </p>" }, ParserToHTML.ParseLines(text));
        }
        [Test]
        public void Paragraph_divisions()
        {
            var text = "some here\n\nText\n   \n        \n \n";
            Assert.AreEqual(new string[] { "<p>some here</p>", "<p>", "Text</p>", "   <p></p>", "        <p></p>", " <p>", "</p>" }, ParserToHTML.ParseLines(text));
        }
        #endregion


        #region Correct parsing backticks
        void ParsingBackticksTest(string[] text, string[] ans)
        {
            ParserToHTML.ParseBackticks(text);
            //TODO. Не перепутаны ли местами аргументы - где Actual, где Expected? Чтобы не путаться, можно использовать синтаксис Assert.That(..., Is.EqaulTo(...))
            Assert.AreEqual(text, ans);
        }

        //TODO. Посмотреть на Testcase. Мб будет удобнее использовать [TestCase("something", "somethingElse", TestName="читаемое название тест кейса")]?
        [Test]
        public void Correct_insert_codes()
        {
            ParsingBackticksTest(new string[] { "uncoded, `coded`" }, new string[] { "uncoded, <code>coded</code>" });
        }

        [Test]
        public void Correct_handle_one_backtick()
        {
            ParsingBackticksTest(new string[] { "uncoded, `uncoded too" }, new string[] { "uncoded, `uncoded too" });
        }
        [Test]
        public void Correct_handle_many_lines()
        {
            ParsingBackticksTest(new string[] { "uncoded, `uncoded too", "and this uncoded too`" }, new string[] { "uncoded, `uncoded too", "and this uncoded too`" });
        }
        [Test]
        public void Correct_handle_many_lines_and_some_backticks()
        {
            ParsingBackticksTest(new string[] { "`uncoded", "this `coded`" }, new string[] { "`uncoded", "this <code>coded</code>" });
        }
        [Test]
        public void Correct_handle_underlines()
        {
            ParsingBackticksTest(new string[] { "_uncoded_, `_coded_`" }, new string[] { "_uncoded_, <code>\\_coded\\_</code>" });
        }
        [Test]
        public void Correct_handle_many_underlines()
        {
            ParsingBackticksTest(new string[] { "`_this_is_it____`" }, new string[] { "<code>\\_this\\_is\\_it\\_\\_\\_\\_</code>" });
        }
        #endregion


        #region Correct parsing double underlines
        void ParsingDoubleUnderlineTest(string[] text, string[] ans)
        {
            ParserToHTML.ParseDoubleUnderlines(text);
            Assert.AreEqual(text, ans);
        }
        [Test]
        public void Simple_double_underlines()
        {
            ParsingDoubleUnderlineTest(new string[] { "__THIS IS STRONG__" }, new string[] { "<strong>THIS IS STRONG</strong>" });
        }
        [Test]
        public void Complex_double_underlines()
        {
            ParsingDoubleUnderlineTest(new string[] { "_norm__ __THIS__IS______STRONG__ __small(cause 3 underline, not 2)___" }, 
                new string[] { "_norm__ <strong>THIS__IS______STRONG</strong> __small(cause 3 underline, not 2)___" });
        }
        [Test]
        public void Avoid_tagged()
        {
            ParsingDoubleUnderlineTest(new string[] { "\\__norm__ __norm\\__" }, new string[] { "\\__norm__ __norm\\__" });
        }
        #endregion


        #region Correct parsing underlines
        void ParsingUnderlineTest(string[] text, string[] ans)
        {
            ParserToHTML.ParseUnderlines(text);
            Assert.AreEqual(text, ans);
        }
        [Test]
        public void Simple_underlines()
        {
            ParsingUnderlineTest(new string[] { "some, _SOME_ , some" }, new string[] { "some, <em>SOME</em> , some" });
        }
        [Test]
        public void Complex_underlines()
        {
            ParsingUnderlineTest(new string[] { "bla, _bla_bla_ bla _aa_____qqq_" }, 
                new string[] { "bla, <em>bla_bla</em> bla <em>aa_____qqq</em>" });
        }
        [Test]
        public void Avoid_tagged_underlines()
        {
            ParsingUnderlineTest(new string[] { "bla, \\_bla_bla_ bla" }, new string[] { "bla, \\_bla_bla_ bla" });
        }
        [Test]
        public void Must_to_parse_underline_with_dots()
        {
            ParsingUnderlineTest(new string[] { "_simple_." }, new string[] { "<em>simple</em>." });
        }
        #endregion


        #region Correct removing escape symbol (\)
        void RemovingEscapeTest(string[] text, string[] ans)
        {
            ParserToHTML.RemoveEscapeChars(text);
            Assert.AreEqual(text, ans);
        }
        [Test]
        public void Remove_single_symbols()
        {
            RemovingEscapeTest(new string[] { "\\ something, \\_removing it all" }, new string[] { " something, _removing it all" });
        }
        [Test]
        public void Remove_single_symbols_in_dif_lines()
        {
            RemovingEscapeTest(new string[] { "begin\\", "\\end" }, new string[] { "begin", "end" });
        }
        [Test]
        public void Pass_double_symbols()
        {
            RemovingEscapeTest(new string[] { "some\\\\thing" }, new string[] { "some\\thing" });
        }
        [Test]
        public void Pass_double_symbols_double()
        {
            RemovingEscapeTest(new string[] { "some\\\\\\\\thing" }, new string[] { "some\\\\thing" });
        }
        [Test]
        public void Remove_single_after_double()
        {
            RemovingEscapeTest(new string[] { "some\\\\\\thing" }, new string[] { "some\\thing" });
        }
        #endregion
    }
}
