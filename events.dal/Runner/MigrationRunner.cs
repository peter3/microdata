using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using System.Reflection;

public static class FluentMigrationRunner
{
    public class MigrationOptions : IMigrationProcessorOptions
    {
        public bool PreviewOnly { get; set; }
        public string ProviderSwitches { get; set; }
        public int Timeout { get; set; }
    }

    public static void MigrateToLatest(string connectionString)
    {
        // var announcer = new NullAnnouncer();
        var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));
        var assembly = Assembly.GetExecutingAssembly();

        var migrationContext = new RunnerContext(announcer)
        {
            Namespace = "events.dal.Models"
        };

        var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
        var factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2014ProcessorFactory();

        using (var processor = factory.Create(connectionString, announcer, options))
        {
            var runner = new MigrationRunner(assembly, migrationContext, processor);
            runner.MigrateUp(true);
        }
    }
}