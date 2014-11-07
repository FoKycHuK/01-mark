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
        [Test]
        public void Replace_special_test()
        {
            var text = "\"><";
            Assert.AreEqual("&quot;&gt;&lt;", ParserToHTML.ParseSpecialSymbols(text));
        }
        [Test]
        public void Replace_special_in_right_order()
        {
            var text = "<p> super test >>> \"<\"";
            Assert.AreEqual("&lt;p&gt; super test &gt;&gt;&gt; &quot;&lt;&quot;", ParserToHTML.ParseSpecialSymbols(text));
        }


        [Test]
        public void Parsing_one_line()
        {
            var text = "Just one line";
            Assert.AreEqual(new string[] { "<p>Just one line</p>" }, ParserToHTML.ParseLines(text));
        }
        [Test]
        public void Parsing_few_lines()
        {
            var text = "Just one line\nAnd another one.    \n And some spaces \n ";
            Assert.AreEqual(new string[] { "<p>Just one line", "And another one.    ", " And some spaces ", " </p>" }, ParserToHTML.ParseLines(text));
        }
        [Test]
        public void Paragraph_divisions()
        {
            var text = "some here\n\nText\n   \n        \n \n";
            Assert.AreEqual(new string[] { "<p>some here", "</p><p>", "Text", "   </p><p>", "        </p><p>", " </p><p>", "</p>" }, ParserToHTML.ParseLines(text));
        }

        [TestCase(new string[] { "uncoded, `coded`" },
            new string[] { "uncoded, <code>coded</code>" },
            TestName = "Correct insert codes")]

        [TestCase(new string[] { "uncoded, `uncoded too" },
            new string[] { "uncoded, `uncoded too" },
            TestName = "Correct handle one backtick")]

        [TestCase(new string[] { "`uncoded", "this `coded`" },
            new string[] { "`uncoded", "this <code>coded</code>" },
            TestName = "Correct handle many lines and some backticks")]

        [TestCase(new string[] { "uncoded, `uncoded too", "and this uncoded too`" },
            new string[] { "uncoded, `uncoded too", "and this uncoded too`" },
            TestName = "Correct handle many lines")]

        [TestCase(new string[] { "_uncoded_, `_coded_`" },
            new string[] { "_uncoded_, <code>\\_coded\\_</code>" },
            TestName = "Correct handle underlines")]

        [TestCase(new string[] { "`_this_is_it____`" },
            new string[] { "<code>\\_this\\_is\\_it\\_\\_\\_\\_</code>" },
            TestName = "Correct handle many underlines")]

        public void ParsingBackticksTest(string[] text, string[] ans)
        {
            ParserToHTML.ParseBackticks(text);
            Assert.AreEqual(ans, text);
        }

        [TestCase(new string[] { "__THIS IS STRONG__" }, 
            new string[] { "<strong>THIS IS STRONG</strong>" }, 
            TestName = "Correct parsing double underlines")]

        [TestCase(new string[] { "_norm__ __THIS__IS______STRONG__ __small(cause 3 underline, not 2)___" },
            new string[] { "_norm__ <strong>THIS__IS______STRONG</strong> __small(cause 3 underline, not 2)___" }, 
            TestName = "Correct double underlines in single underlines")]

        [TestCase(new string[] { "\\__norm__ __norm\\__" }, 
            new string[] { "\\__norm__ __norm\\__" }, 
            TestName = "Avoid tagged")]

        public void ParsingDoubleUnderlineTest(string[] text, string[] ans)
        {
            ParserToHTML.ParseUnderlines(text, "__", "strong");
            Assert.AreEqual(ans, text);
        }

        [TestCase(new string[] { "some, _SOME_ , some" }, 
            new string[] { "some, <em>SOME</em> , some" }, 
            TestName = "Correct parsing underlines")]

        [TestCase(new string[] { "bla, _bla_bla_ bla _aa_____qqq_" },
            new string[] { "bla, <em>bla_bla</em> bla <em>aa_____qqq</em>" }, 
            TestName = "Correct parsing underlines with multiunderlines")]

        [TestCase(new string[] { "bla, \\_bla_bla_ bla" }, 
            new string[] { "bla, \\_bla_bla_ bla" }, 
            TestName = "Avoid tagged underlines")]

        [TestCase(new string[] { "_simple_." }, 
            new string[] { "<em>simple</em>." }, 
            TestName = "Correct parsing with dots")]

        public void ParsingUnderlineTest(string[] text, string[] ans)
        {
            ParserToHTML.ParseUnderlines(text, "_", "em");
            Assert.AreEqual(ans, text);
        }

        [TestCase(new string[] { "\\ something, \\_removing it all" }, 
            new string[] { " something, _removing it all" }, 
            TestName = "Correct removing single escape")]

        [TestCase(new string[] { "begin\\", "\\end" }, 
            new string[] { "begin", "end" }, 
            TestName = "Removing in diff lines")]

        [TestCase(new string[] { "some\\\\thing" }, 
            new string[] { "some\\thing" }, 
            TestName = "Correct pass double escape")]

        [TestCase(new string[] { "some\\\\\\\\thing" }, 
            new string[] { "some\\\\thing" }, 
            TestName = "Correct pass double escape double")]

        [TestCase(new string[] { "some\\\\\\thing" }, 
            new string[] { "some\\thing" }, 
            TestName = "Correct Remove single after double")]

        public void RemovingEscapeTest(string[] text, string[] ans)
        {
            ParserToHTML.RemoveEscapeChars(text);
            Assert.AreEqual(ans, text);
        }
    }
}
