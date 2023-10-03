using NUnit.Framework.Interfaces;

namespace NUnit.Runner.Helpers;

public class FinishedTestListener : ITestListener
{
    public event EventHandler Finished; 

    public void TestStarted(ITest test)
    {
    }

    public void TestFinished(ITestResult result)
    {
        if (!result.HasChildren)
            Finished?.Invoke(this, EventArgs.Empty);
    }

    public void TestOutput(TestOutput output)
    {
    }

    public void SendMessage(TestMessage message)
    {
    }
}