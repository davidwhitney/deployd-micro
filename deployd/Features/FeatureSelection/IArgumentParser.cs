using System.Collections.Generic;

namespace deployd.Features.FeatureSelection
{
    public interface IArgumentParser
    {
        InstanceConfiguration Parse(IEnumerable<string> args);
    }
}