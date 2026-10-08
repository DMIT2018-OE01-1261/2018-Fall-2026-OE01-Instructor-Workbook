using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OLTPSystem.DAL;

namespace OLTPSystem.Tests;

// Every test class inherits from TestBase. (Do not modify)
//
//	- Gives each test its own database context (Context)
//	- Starts a transaction before each test and rolls it back after,
//	  so Add, Edit and Delete tests never change your real data
//	- Gives you Dump() to see a value, like LINQPad's .Dump()
public abstract class TestBase : IDisposable
{
	protected readonly OLTPContext Context;
	private readonly IDbContextTransaction _transaction;
	private readonly ITestOutputHelper _output;

	protected TestBase(ITestOutputHelper output)
	{
		_output = output;

		var options = new DbContextOptionsBuilder<OLTPContext>()
			.UseSqlServer(TestSettings.ConnectionString)
			.Options;

		Context = new OLTPContext(options);
		_transaction = Context.Database.BeginTransaction();
	}

	// Shows a value in the test's output, like LINQPad's .Dump().
	//	In Test Explorer, click the test: the data shows under "Standard Output".
	//		Dump(result.Value, "Customers");
	protected void Dump(object? value, string title = "")
	{
		if (!string.IsNullOrWhiteSpace(title))
			_output.WriteLine($"===== {title} =====");
		_output.WriteLine(JsonSerializer.Serialize(value, DumpOptions));
	}

	private static readonly JsonSerializerOptions DumpOptions = new() { WriteIndented = true };

	// Runs after every test. The transaction is never committed,
	//	so disposing it rolls back every change the test made.
	public void Dispose()
	{
		_transaction.Dispose();
		Context.Dispose();
	}
}
