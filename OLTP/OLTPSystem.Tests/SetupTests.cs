namespace OLTPSystem.Tests;

public class SetupTests : TestBase
{
	public SetupTests(ITestOutputHelper output) : base(output) { }

	// Sanity check: confirms the tests run and the database connection works.
	//	If this test fails, fix TestSettings.cs before anything else.
	[Fact]
	public void Setup_DatabaseConnection_CanConnect()
	{
		Assert.True(Context.Database.CanConnect(),
			"Cannot connect to the database. Check the connection string in TestSettings.cs.");
	}
}
