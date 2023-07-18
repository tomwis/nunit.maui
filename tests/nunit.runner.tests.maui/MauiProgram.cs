using Microsoft.Extensions.Logging;
using NUnit.Runner.Services;
using NUnit.Runner.Tests;

namespace nunit.runner.tests.maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // This will load all tests within the current project
        var nunit = new NUnit.Runner.App();

        // If you want to add tests in another assembly
        nunit.AddTestAssembly(typeof(MauiProgram).Assembly);
        // Or, if you want to add tests with an extra test options dictionary
        //nunit.AddTestAssembly(typeof(MyTests).Assembly, new Dictionary<string, object>());

        // Available options for testing
        nunit.Options = new TestOptions
        {
            // If True, the tests will run automatically when the app starts
            // otherwise you must run them manually.
            AutoRun = true,

            // If True, the application will terminate automatically after running the tests.
            //TerminateAfterExecution = true,

            // Information about the tcp listener host and port.
            // For now, send result as XML to the listening server.
            //TcpWriterParameters = new TcpWriterInfo("192.168.0.108", 13000),

            // Creates a NUnit Xml result file on the host file system using PCLStorage library.
            // CreateXmlResultFile = true,

            // Choose a different path for the xml result file
            // ResultFilePath = Path.Combine(Environment.ExternalStorageDirectory.Path, Environment.DirectoryDownloads, "Nunit", "Results.xml")
        };
        
        var builder = MauiApp.CreateBuilder().UseMauiApp(_ => nunit);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}