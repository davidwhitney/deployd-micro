using NUnit.Framework;
using deployd.Features.FeatureSelection;

namespace deployd.tests.Features.FeatureSelection
{
    [TestFixture]
    public class ArgumentParserTests
    {
        private ArgumentParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new ArgumentParser();
        }

        [Test]
        public void Parse_NoArgs_HelpIsFlagged()
        {
            var config = _parser.Parse(new string[0]);

            Assert.That(config.Help, Is.True);
        }

        [TestCase("--help")]
        [TestCase("-help")]
        [TestCase("/help")]
        [TestCase("--?")]
        [TestCase("-?")]
        [TestCase("/?")]
        [TestCase("--h")]
        [TestCase("-h")]
        [TestCase("/h")]
        public void Parse_ArgumentsContainHelp_HelpIsFlagged(string suportedHelpCommands)
        {
            var config = _parser.Parse(new[] { "--junk", suportedHelpCommands });

            Assert.That(config.Help, Is.True);
        }

        [TestCase("--v")]
        [TestCase("-v")]
        [TestCase("/v")]
        [TestCase("--verbose")]
        [TestCase("-verbose")]
        [TestCase("/verbose")]
        public void Parse_ArgumentsContainVerbose_VerboseIsFlagged(string supportedVerboseCommands)
        {
            var config = _parser.Parse(new[] {"--junk", supportedVerboseCommands});

            Assert.That(config.Verbose, Is.True);
        }

        [Test]
        public void Parse_UnknownArgsPresent_UnknownArgsAreAddedAsExtras()
        {
            var config = _parser.Parse(new[] {"--help", "--somethingelse"});

            Assert.That(config.ExtraParams[0], Is.StringContaining("--somethingelse"));
        }

        [TestCase("--app")]
        [TestCase("-app")]
        [TestCase("/app")]
        public void Parse_AppPresent_MappedToAppName(string prefix)
        {
            var config = _parser.Parse(new []{prefix + "=MyApp"});

            Assert.That(config.AppName, Is.EqualTo("MyApp"));
        }

        [TestCase("--app")]
        [TestCase("-app")]
        [TestCase("/app")]
        public void Parse_AppPresentAndContainsSpaces_MappedToAppName(string prefix)
        {
            var config = _parser.Parse(new[] { prefix + "=\"My App\"" });

            Assert.That(config.AppName, Is.EqualTo("My App"));
        }

        [TestCase("--install")]
        [TestCase("-install")]
        [TestCase("/install")]
        [TestCase("--i")]
        [TestCase("-i")]
        [TestCase("/i")]
        public void Parse_ArgumentsContainInstall_InstallIsFlagged(string suportedHelpCommands)
        {
            var config = _parser.Parse(new[] { "--junk", suportedHelpCommands });

            Assert.That(config.Install, Is.True);
        }
    }
}
