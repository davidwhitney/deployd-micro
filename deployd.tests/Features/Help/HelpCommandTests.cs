using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using deployd.Features.FeatureSelection;
using deployd.Features.Help;

namespace deployd.tests.Features.Help
{
    [TestFixture]
    public class HelpCommandTests
    {
        [Test]
        public void Unfinished()
        {
            var config = new InstanceConfiguration();
            var cmd = new HelpCommand(config);

        }
    }
}
