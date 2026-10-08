# OLTP Solution

This solution replaces the LINQPad service files (like `ServiceExample.linq`).
We will keep adding to it all term until it becomes our Blazor web app.

| Project | What it is | When we get to Blazor |
|---|---|---|
| **OLTPSystem** | Services (BLL), view models, database context (DAL) and entities | Moves over unchanged. The Blazor app references it. |
| **OLTPSystem.Tests** | xUnit tests for the services (replaces `Main()`) | Stays behind |

## Where things went from LINQPad

| LINQPad | Now |
|---|---|
| Part 1: `Main()` with Pass/Fail checks | `OLTPSystem.Tests` - one test per check, run from Test Explorer |
| `verbose` and `.Dump()` | `Dump(value, "Title")` inside a test (see below) |
| Part 2: `CodeBehind` class | Not needed yet: tests call the service directly. It comes back as the Blazor page's `.razor.cs` file. |
| Part 3: `CustomerService`, `LookupService` | `OLTPSystem/BLL/` |
| Part 4: View Models | `OLTPSystem/ViewModels/` (one file per class) |
| `GetErrorMessages` | `OLTPSystem/Helpers/ResultHelpers.cs` |
| LINQPad's database connection (`this`) | `OLTPContext` in `OLTPSystem/DAL/` |

## First-time setup

1. **Restore the database** `OLTP-DMIT2018` if you haven't already.
2. **Open** `OLTP.slnx` in Visual Studio.
3. **Check your connection** in `OLTPSystem.Tests/TestSettings.cs`.
   It is set for SQL Server Express (`.\SQLEXPRESS`). If you use the default
   instance, change it to `Server=.`.
4. **Run the tests**: Test menu > Test Explorer > Run All (or `dotnet test` in a terminal).
   `Setup_DatabaseConnection_CanConnect` should pass. If it does, you're ready.

## Running and reading tests

- **Run All** runs every test. A green tick passed; a red X failed.
- Click a failed test to see why, e.g. `Expected: 14  Actual: 12`.
- Right-click a test > **Run** to run just that one.
- Right-click a test > **Debug** to stop on breakpoints in your service code.
- To see data (like LINQPad's `.Dump()`), add `Dump(result.Value, "Customers");`
  to the test, run it, and click the test. The data shows under **Standard Output**
  in the Test Detail Summary pane.

## Writing a service method (the pattern)

1. Add the view model to `ViewModels/`.
2. Add the method to the service in `BLL/`. It always returns a `Result<T>`:
   - **Return right away** for missing parameters: `result.AddError(...)` then `return result;`
   - **Collect** the other rule errors, then `if (result.IsFailure) return result;`
   - Happy path: `return result.WithValue(value);`
3. Add tests: at least one test that expects each error, and one that expects success.

Adding a new service? Create the class in `BLL/`, add a matching test class that
inherits `TestBase`, and add an `AddScoped` line in `OLTPExtension.cs`.

**Add, edit and delete tests are safe**: every test runs in a database transaction
that is rolled back when the test ends, so your data never changes.

## If the database changes

The `DAL` and `Entities` folders were generated from the database. To regenerate them,
run this in a terminal in the solution folder:

```powershell
.\scaffold.ps1 -Force
```

Default SQL Server instance: `.\scaffold.ps1 -Server "." -Force`.
If PowerShell refuses to run the script, run this once first:
`Set-ExecutionPolicy -Scope CurrentUser RemoteSigned`
