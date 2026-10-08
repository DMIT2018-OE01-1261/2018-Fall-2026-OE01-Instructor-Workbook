<Query Kind="Program">
  <NuGetReference>xunit</NuGetReference>
  <NuGetReference>xunit.runner.utility</NuGetReference>
  <Namespace>Xunit</Namespace>
</Query>

// This script can be #load-ed into other script for xunit test support. *V=1.8*
// You can modify the code to customize the xunit test runner behavior.

void Main()
{
	RunTests();
	return;
}

#region private::Demo Tests

// Note: Regions prefixed with 'private::' are ignored when the script is #loaded into another script.

[Fact] void Test_ShouldSucceed() => Assert.True (1 + 1 == 2);
[Fact] void Test_ShouldFail()    => Assert.True (1 + 1 == 3);

public class TestClass   // You can also define tests in a (public) class.
{
	[Theory]
	[InlineData (3)]
	[InlineData (5)]
	[InlineData (6)]
	void MyFirstTheory (int value) => Assert.True (IsOdd (value));
}

public static bool IsOdd (int value) => value % 2 == 1;

#endregion

/// <summary>Runs an xunit test with the specified method name and displays the results. This method is editable in the 'xunit' script.</summary>
static TestResultSummary[] RunTest (string methodName, bool quietly = false, bool reportFailuresOnly = false)
	=> RunTests (quietly, reportFailuresOnly, c => c.TestMethod.Method.Name == methodName);

/// <summary>Runs all xunit tests and displays the results. To run a single test, call RunTest() instead. This method is editable in the 'xunit' script.</summary>
static TestResultSummary[] RunTests (bool quietly = false, bool reportFailuresOnly = false, Func<Xunit.Abstractions.ITestCase, bool> filter = null)
{
	using var runner = Xunit.Runners.AssemblyRunner.WithoutAppDomain (typeof (UserQuery).Assembly.Location);
	if (filter != null) runner.TestCaseFilter = filter;

	int totalTests = 0, completedTests = 0, failures = 0;
	runner.OnDiscoveryComplete = info => totalTests = info.TestCasesToRun;

	var tests = new TestResultSummary [0];
#if CMD
	object dc = new object();
#else
	var dc = new DumpContainer (Util.WithHeading (tests, "Test Results"));
	if (!quietly) dc.Dump ("", collapseTo: 1, repeatHeadersAt:0);
#endif

	runner.OnTestFailed = info => AddTestResult (info);
	runner.OnTestPassed = info => AddTestResult (info);

	using var done = new ManualResetEventSlim();
	runner.OnExecutionComplete = info =>
	{
		if (!quietly) $"Completed {info.TotalTests} tests in {Math.Round (info.ExecutionTime, 3)}s ({info.TestsFailed} failed)".Dump("");
		done.Set();
	};
	runner.Start();
	done.Wait();
#if CMD
	if (!quietly) tests.Dump ("Test Results");
#endif
#if LINQPAD9
	if (!quietly && Util.AI.IsEnabled && tests.Where (t => t.Failed()) is var failedTests && failedTests.Any())
	{
		string aiInstruction = "The following xunit tests failed:\n\n" +
			string.Join ("\n\n", failedTests.Select (f =>
				$"{f.TypeName}.{f.MethodName}{f.Case}: {f.FailureInfo.ExceptionType}\n   {f.FailureInfo.ExceptionMessage.Replace ("\n", "\n   ")} "));

		int? firstLine = failedTests.SelectMany (t => t.GetStackTrace()).Min (f => (int?)f.line);

		Util.Highlight (Util.AI.NewInlineAgentLink ("Send failed test details to AI Agent", aiInstruction, firstLine - 1)).Dump();
	}
#endif
	return tests;

	void AddTestResult (Xunit.Runners.TestInfo testInfo)
	{
		var summary = new TestResultSummary (testInfo);
		lock (dc)
		{
			completedTests++;
			if (summary.Failed()) failures++;

			if (!reportFailuresOnly || summary.Failed())
				tests = tests
					.Append (summary)
					.OrderBy (t => t.Succeeded())
					.ThenBy (t => t.TypeName)
					.ThenBy (t => t.MethodName)
					.ThenBy (t => t.Case)
					.ToArray();

			#if !CMD
						dc.Content = Util.WithHeading (tests, $"Test Results - {completedTests} of {totalTests} ({failures} failures)");
			#endif
		}
	}
}

public class TestResultSummary
{
	Xunit.Runners.TestInfo _testInfo;
	public TestResultSummary (Xunit.Runners.TestInfo testInfo) => _testInfo = testInfo;

	public bool Succeeded() => _testInfo is Xunit.Runners.TestPassedInfo;
	public bool Failed() => _testInfo is Xunit.Runners.TestFailedInfo;

	public string TypeName => _testInfo.TypeName;
	public string MethodName => _testInfo.MethodName;

	public string Case => _testInfo.TestDisplayName.Substring (
		_testInfo.TestDisplayName.StartsWith (TypeName + "." + MethodName) ? TypeName.Length + 1 + MethodName.Length : 0);

	public decimal? Seconds => (_testInfo as Xunit.Runners.TestExecutedInfo)?.ExecutionTime;

	public object Status =>
		_testInfo is Xunit.Runners.TestPassedInfo ? Util.WithStyle ("Succeeded", "color:green") :
		_testInfo is Xunit.Runners.TestFailedInfo ? Util.WithStyle ("Failed", "color:red") :
		"";

	public Xunit.Runners.TestFailedInfo FailureInfo => _testInfo as Xunit.Runners.TestFailedInfo;

	public object Location => 
		Util.VerticalRun (GetStackTrace().Select (frame => Util.HorizontalRun (true, frame.name, new LINQPad.Hyperlinq (frame.line - 1, 0, $"line {frame.line}"))));

	public IEnumerable<(string name, int line)> GetStackTrace() =>
		from match in Regex.Matches (FailureInfo?.ExceptionStackTrace ?? "", @"(at .+?)\s+in\s+.+?LINQPadQuery:line\s+(\d+)")
		let line = int.Parse (match.Groups [2].Value)
		select (match.Groups [1].Value, line);
}			