using System;
using System.Collections.Generic;
using log4net;

namespace deployd.Features.FeatureSelection
{
    public class CommandCollection : List<IFeatureCommand>
    {
        private readonly ILog _log;

        public CommandCollection(ILog log)
        {
            _log = log;
        }

        public void RunAll()
        {
            try
            {
                foreach (var command in this)
                {
                    command.Execute();
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
                Environment.Exit(-1);
            }
        }
    }
}