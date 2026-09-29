<Query Kind="Statements">
  <Connection>
    <ID>7e8fa38e-3601-4761-a5fd-0ce3c0689212</ID>
    <NamingServiceVersion>3</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <UseMicrosoftDataSqlClient>true</UseMicrosoftDataSqlClient>
    <DisplayName>Chinook</DisplayName>
    <EncryptTraffic>true</EncryptTraffic>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook-2025</Database>
    <MapXmlToString>false</MapXmlToString>
    <DriverData>
      <SkipCertificateCheck>true</SkipCertificateCheck>
    </DriverData>
  </Connection>
</Query>

// ================ First ================
// Return the first item in a collection
Albums.First().Dump();

// First throws an error when there are no items in a collection
//	This includes if nothing is found when using a where clause
//	Error Shown: Sequence contains no elements

//Albums
//	.Where(x => x.AlbumId == 1000000)
//	.First().Dump();
	
// ================ FirstOrDefault ================
// We will NEVER, EVER use First, we will always use FirstOrDefault
// FirstOrDefault
//	Return a null for an object when nothing is found
Albums
	.Where(x => x.AlbumId == 1000000)
	.FirstOrDefault().Dump();
	
// Always know the Default for the Datatype you are querying, not all are null
// String - null
// Int32 - 0
Albums
	.Where(x => x.AlbumId == 1000000)
	.Select(x => x.ReleaseYear) //Int32
	.FirstOrDefault().Dump();


// Int32? (nullable int) - null
Customers
	.Where(x => x.CustomerId == 100000)
	.Select(x => x.SupportRepId)
	.FirstOrDefault().Dump();

// DateTime - 01-Jan-01 12:00:00 AM (Depends on the Database)
Invoices
	.Where(x => x.InvoiceId == 100000)
	.Select(x => x.InvoiceDate)
	.FirstOrDefault().Dump();
	
// DateTime? (nullable) - null
Employees
	.Where(x => x.EmployeeId == 1000000)
	.Select(x => x.HireDate)
	.FirstOrDefault().Dump();
	
// Remember that ViewModel is just an object
//	So in this case null is returned
Albums
	.Where(x => x.AlbumId == 1000000)
	.Select(x => new AlbumView
	{
		AlbumID = x.AlbumId
	})
	.FirstOrDefault().Dump();

// Not good practice ever to have two SELECT
// YOU WILL BE DOCKED MARKS UNLESS IT IS ABSOLUTELY REQUIRED
//	But selecting the field from the ViewModel still acts like it's datatype
//	If the ViewModel is never constructed (it is a null) the initializer (= 5)
//		never runs
//		The select is selecting nothing from a null value
//		it just changes the datatype to an int
Albums
	.Where(x => x.AlbumId == 1000000)
	.Select(x => new AlbumView
	{
		AlbumID = x.AlbumId
	})
	.Select(x => x.AlbumID)
	.FirstOrDefault().Dump();

// ================ Single ================
// 	This looks for one and only one item from a collection
Albums
	.Where(x => x.AlbumId == 2)
	.Single().Dump();

// Single will error out if there is more than one item returned	
Albums
	.Where(x => x.AlbumId <= 5)
	.Single().Dump();
	
// ================ SingleOrDefault ================
// 	This will also error out if there is more than one item returned
//	WE DO NOT USE THIS IN THIS CLASS
//	Normal standards are to use FirstOrDefault in place of any Single calls
Albums
	.Where(x => x.AlbumId <= 5)
	.SingleOrDefault().Dump();
	
// ================ Distinct ================
// Remove all fully duplicated records
//	Only if ALL fields are exactly the same
Albums //300 records
	.Where(x => x.ReleaseYear > 1980)
	.Select(x => new
	{
		Year = x.ReleaseYear,
		Label = x.ReleaseLabel
	})
	.OrderBy(x => x.Year)
	.ThenBy(x => x.Label)
	.Dump();
	
Albums //263 records
	.Where(x => x.ReleaseYear > 1980)
	.Select(x => new
	{
		Year = x.ReleaseYear,
		Label = x.ReleaseLabel
	})
	.OrderBy(x => x.Year)
	.ThenBy(x => x.Label)
	// Remember .Distict need to be AFTER you select your data
	.Distinct()
	.Dump();

Albums //300 records
	// Because each Album is unique 
	//	(all fields are included including the unique PK) here
	//	this distinct does nothing
	.Distinct()
	.Where(x => x.ReleaseYear > 1980)
	.Select(x => new
	{
		Year = x.ReleaseYear,
		Label = x.ReleaseLabel
	})
	.OrderBy(x => x.Year)
	.ThenBy(x => x.Label)
	.Dump();

// ================ Any/All ================
// These are boolean returns (True or False) 
// Any will return true if there is any record in the collect that
//	matches the search (most often a .Where)

// True Return example
Albums
	.Where(x => x.ReleaseYear == 1976)
	.Any().Dump();

//Can simplify if it is a basic where, just use Any()
Albums
	.Any(x => x.ReleaseYear == 1976)
	.Dump();
	
// False return example
Albums
	.Where(x => x.ReleaseYear == 1876)
	.Any().Dump();
	
// All will return true if EVERY element of a collection matches the search

// !(...) returns the opposite of what inside the brackets would return
Albums
	.All(x => !(x.ReleaseLabel.Trim() == "") || x.ReleaseLabel != null)
	.Dump();

// ================ Unions and Other Joins ================
int[] numbersA = [1,2,3,4];
int[] numbersB = [3,4,5,6];

// Union will combine the distinct elements
numbersA.Union(numbersB).Dump("Union");

// Intersect will only return the common elements from two collections
// Note: Super useful when you want to check if something already exists in another list
numbersA.Intersect(numbersB).Dump("Intersect");

// Except will return elements from the FIRST collection that are not in the SECOND collection
numbersA.Except(numbersB).Dump("Except");

// Concat is the same as SQL Union, return all element from both collections
numbersA.Concat(numbersB).Dump("Concat");

// ================ Take ================

// Take retrives a specified number of elements from the starts of a collection
Albums.Take(5).Dump();

// Take While retrieves elements from the starts of a collection as long as the condition is true
// Example: As long as the number is <= 3
numbersA.TakeWhile(x => x <= 3).Dump();
// Does not work on the Linq collection in LinqPad
// Albums.TakeWhile(x => x.ReleaseLabel != null).Dump();

// ================ Skip ================
// Skip will skip a specified number of items and return the rest
//	Can be used with Take to get specific records (Example: For paging)
Albums.Skip(30).Take(5).Dump();

// SkipWhile will not work in LinqPad
// 	the mother continues to skip items while a condition is met.

// View Model for Examples
public class AlbumView
{
	public int AlbumID { get; set; } = 5; //Remember this is the default value
}