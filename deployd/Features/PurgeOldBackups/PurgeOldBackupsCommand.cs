namespace deployd.Features.PurgeOldBackups
{
    public class PurgeOldBackupsCommand : IFeatureCommand
    {
        private readonly IApplication _app;

        public PurgeOldBackupsCommand(IApplication app)
        {
            _app = app;
        }

        public void Execute()
        {
            _app.PruneBackups();
        }
    }
}
