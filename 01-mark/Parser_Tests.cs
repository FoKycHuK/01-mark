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
            Assert.AreEqual("&quot;&gt;&lt;", MarkdownProcessor.ParseSpecialSymbols(text));
        }
        [Test]
        public void Replace_special_in_right_order()
        {
            var text = "<p> super test >>> \"<\"";
            Assert.AreEqual("&lt;p&gt; super test &gt;&gt;&gt; &quot;&lt;&quot;", MarkdownProcessor.ParseSpecialSymbols(text));
        }


        [TestCase(new string[] {"Just one line"}, 
            new string[] { "<p>Just one line</p>" }, 
            TestName = "Parse one line on paragraph")]

        [TestCase(new string[] { "Just one line", "And another one.    ", " And some spaces ", " " }, 
            new string[] { "<p>Just one line", "And another one.    ", " And some spaces ", " </p>" }, 
            TestName = "Parsing few lines on paragraph")]

        [TestCase(new string[] { "some here", "", "Text", "   ", "        ", " ", "" },
            new string[] { "<p>some here", "</p><p>", "Text", "   </p><p>", "        </p><p>", " </p><p>", "</p>" },
            TestName = "Paragraph divisions test")]

        public void ParagraphTest(string[] text, string[] ans)
        {
            var lines = MarkdownProcessor.ParseParagraphs(text);
            Assert.AreEqual(ans, lines);
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
            text = MarkdownProcessor.ParseSymbols(text, "`", "code");
            Assert.AreEqual(ans, text);
        }

        [TestCase(new string[] { "__THIS IS STRONG__" }, 
            new string[] { "<strong>THIS IS STRONG</strong>" }, 
            TestName = "Correct parsing double underlines")]

        [TestCase(new string[] { "_norm__ __THIS__IS___STRONG__ __small(cause 3 underline, not 2)___" },
            new string[] { "_norm__ <strong>THIS\\_\\_IS\\_\\_\\_STRONG</strong> __small(cause 3 underline, not 2)___" }, 
            TestName = "Correct double underlines in single underlines")]

        [TestCase(new string[] { "\\__norm__ __norm\\__" }, 
            new string[] { "\\__norm__ __norm\\__" }, 
            TestName = "Avoid tagged")]

        [TestCase(new string[] { "__ strong _ strong __ not strong _"},
            new string[] { "<strong> strong \\_ strong </strong> not strong _"},
            TestName = "Correct parse underline in double underlines")]

        public void ParsingDoubleUnderlineTest(string[] text, string[] ans)
        {
            text = MarkdownProcessor.ParseSymbols(text, "__", "strong");
            Assert.AreEqual(ans, text);
        }

        [TestCase(new string[] { "some, _SOME_ , some" }, 
            new string[] { "some, <em>SOME</em> , some" }, 
            TestName = "Correct parsing underlines")]

        [TestCase(new string[] { "bla, _bla_bla_ bla _aa___qqq_" },
            new string[] { "bla, <em>bla\\_bla</em> bla <em>aa\\_\\_\\_qqq</em>" }, 
            TestName = "Correct parsing underlines with multiunderlines")]

        [TestCase(new string[] { "bla, \\_bla_bla_ bla" }, 
            new string[] { "bla, \\_bla_bla_ bla" }, 
            TestName = "Avoid tagged underlines")]

        [TestCase(new string[] { "_simple_." }, 
            new string[] { "<em>simple</em>." }, 
            TestName = "Correct parsing with dots")]

        public void ParsingUnderlineTest(string[] text, string[] ans)
        {
            text = MarkdownProcessor.ParseSymbols(text, "_", "em");
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
            text = MarkdownProcessor.RemoveEscapeChars(text);
            Assert.AreEqual(ans, text);
        }


        [TestCase(new string[] { "__strong _strong__ simple_" }, 
            new string[] { "<strong>strong \\_strong</strong> simple_" }, 
            TestName = "Mixed underlines test")]

        [TestCase(new string[] { "_em __strong__ em_" }, 
            new string[] { "<em>em <strong>strong</strong> em</em>" }, 
            TestName = "Strong in em test")]

        [TestCase(new string[] { "__strong _strong_ strong__" }, 
            new string[] { "<strong>strong \\_strong\\_ strong</strong>" }, //да, такая вложенность не работает. имею право по условиям задачи.
            TestName = "Em in strong test")]

        public void Complex_tests(string[] text, string[] ans)
        {
            text = MarkdownProcessor.ParseSymbols(text, "__", "strong");
            text = MarkdownProcessor.ParseSymbols(text, "_", "em");
            Assert.AreEqual(ans, text);
        }
    }
}
