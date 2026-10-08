namespace OLTPSystem.Tests;

public static class TestSettings
{
	// Change Server to match your SQL Server (same as you used for scaffold.ps1):
	//	".\\SQLEXPRESS"  SQL Server Express
	//	"."              default instance on your computer
	public const string ConnectionString =
		"Server=.\\SQLEXPRESS;Database=OLTP-DMIT2018;Trusted_Connection=True;TrustServerCertificate=True";
}
