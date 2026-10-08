# Creates the DAL (database context) and Entities (table classes) folders
# in the OLTPSystem project from your database.
#
# Run from the solution folder (the folder with OLTP.slnx):
#     .\scaffold.ps1
# Default SQL Server instance instead of SQL Server Express:
#     .\scaffold.ps1 -Server "."
#
# Run it again with -Force if the database changes.

param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$Database = "OLTP-DMIT2018",
    [string]$Context = "OLTPContext",
    [switch]$Force
)

# Installs the EF Core command-line tool listed in dotnet-tools.json
dotnet tool restore

$connection = "Server=$Server;Database=$Database;Trusted_Connection=True;TrustServerCertificate=True"

$arguments = @(
    "ef", "dbcontext", "scaffold", $connection,
    "Microsoft.EntityFrameworkCore.SqlServer",
    "--project", "OLTPSystem",
    "--context", $Context,
    "--context-dir", "DAL",
    "--context-namespace", "OLTPSystem.DAL",
    "--output-dir", "Entities",
    "--namespace", "OLTPSystem.Entities",
    # Keep the database's names (CustomerID, not CustomerId), same as LINQPad
    "--use-database-names",
    "--data-annotations",
    "--no-onconfiguring"
)
if ($Force) { $arguments += "--force" }

dotnet @arguments
