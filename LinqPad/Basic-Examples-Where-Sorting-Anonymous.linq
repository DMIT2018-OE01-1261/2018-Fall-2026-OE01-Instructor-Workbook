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

//============== WHERE EXAMPLES =================
int[] numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9];

// Evaluate each item in the numbers collection to test if % 2 == 0
// The right side of the lamba expression for Where, when it is true the value is returned
// when false the value is discarded
var evenNumber = numbers.Where(n => n % 2 == 0);

// We have to for C# Statements in LinqPad .Dump() the results to the results window
numbers.Dump();
evenNumber.Dump();

// To use our connection we reference the data by the table names
Albums.Dump();

// the letter a is just how we choose to reference albums, we could make this bob
// or any other letter we want
Albums.Where(a => a.ReleaseYear == 1999)
	.Select(a => a.Title)
	.Dump();

//============ SORTING EXAMPLES ============

// && AND, || OR
// Make sure that the album release year is 1990 or greater AND 1999 or less
Albums.Where(x => x.ReleaseYear >= 1990 && x.ReleaseYear <= 1999)
	.OrderBy(x => x.ReleaseYear)
	.ThenBy(x => x.Title)
	.ThenByDescending(x => x.ReleaseLabel)
	.Dump();
	
//========== NAVIGATIONAL PROPERTIES ===========

// KEY POINTS: Navigate to a parent there is ONE record
// Navigate to children there are Many records

// Since a parent only has one record, 
// we can see all the fields/properties of the parent record
//	Example: record.Parent.parentField
Albums.Where(a => a.Artist.Name == "Deep Purple")
	.Dump();
	
// Since there are many child records, we get a collection only
// (a collection of 0 or more records - can be 0 if the record has no children)
// When it is a collect we cannot see the individual fields/properties of the records
// 	for example: Albums.Where(a => a.Tracks.Name == "Coverdale").Dump();
// 	will not work, because album.Tracks returns as an entityset (a collection)

//============ Anonymous Datasets ==================

// If we want certain fields we can return an anonymouse collection of data
//	To do this we use the new keyword to say we want to define (shape) the data to a new type
//	We are not shaping it into a class or defined type, so it is anonymous <> or untitled
Albums.Where(x => x.AlbumId < 6)
	.Select(x => new
	{
		Title = x.Title,
		Year = x.ReleaseYear,
		Label = x.ReleaseLabel
	}).Dump();
	
// Once you have renamed a field in your shape, you must refer to it
// by that name.
// Keep is mind after each method the data has a new shape.
Albums.Where(x => x.AlbumId < 6)
	.Select(x => new
	{
		Title = x.Title,
		Year = x.ReleaseYear,
		Label = x.ReleaseLabel
	})
	.Select(y => new 
	{
		Bob = y.Title,
		Fred = y.Year,
		Jenny = y.Label
	})
	.OrderBy(x => x.Fred)
	.Dump();
	
// We can also use those navigational properties for parent records to shape data
// from multiple tables
Albums.Where(x => x.AlbumId < 6)
	.Select(x => new
	{
		Title = x.Title,
		Year = x.ReleaseYear,
		Artist = x.Artist.Name
	})
	.Dump();
	
// Navigation in any type (anonymous or otherwise) to a child record will return a collection
//	We canot select a single field/property from the child record collection
//	but we can select the whole collection
Albums.Where(x => x.AlbumId < 6)
	.Select(x => new
	{
		Title = x.Title,
		Year = x.ReleaseYear,
		Artist = x.Artist.Name,
		Tracks = x.Tracks
	})
	.Dump();
