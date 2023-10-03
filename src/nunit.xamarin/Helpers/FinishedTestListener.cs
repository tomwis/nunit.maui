using NUnit.Framework.Interfaces;

namespace NUnit.Runner.Helpers;

public class FinishedTestListener : ITestListener
{
    public event EventHandler Finished; 

    public void TestStarted(ITest test)
    {
        if (!test.HasChildren)
            Started?.Invoke(this, test.FullName);
    }

    public event EventHandler<string> Started;

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