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

//================ Group By ===============
// Group by is only used when absolutely needed, otherwise use other method to find the same results
// Group by is resource intensive and can slow down your queries

// Group By is needed when we are trying to organize data or find info all within one table

// If I wanted to show Album data by the Artist, Group By isn't the best solution
// But if I wanted to show Album data by the ReleaseYear, Group By becomes useful

Albums
	.GroupBy(x => x.ReleaseYear)
	.Dump();
	
Albums
	.GroupBy(x => x.ReleaseYear)
	.Select(x => new
	{
		//Key refers to the field or fields that are used in the Group By
		Year = x.Key,
		//The actual group data is just referenced with the variable (in this case x)
		Albums = x.ToList()
	}).Dump();

// We can also group by multiple fields
// 	To do this we make what we are grouping by an anonymous dataset
//	use the new keyword
Albums
	.GroupBy(x => new { x.ReleaseYear, x.ReleaseLabel })
	.Select(x => new
	{
		// Reference each key in the Group By
		Year = x.Key.ReleaseYear,
		Label = x.Key.ReleaseLabel == null ? "Unknown" : x.Key.ReleaseLabel,
		Count = x.Count(),
		// Since the data is grouped now we can select information for it
		Albums = x.Select(a => new
		{
			Title = a.Title,
			Artist = a.Artist.Name
		}).ToList()
	})
	.OrderBy(x => x.Year)
	.ThenBy(x => x.Label)
	.Dump();
	
// Example when NOT to use Group By
// 	Show the invoice Total for each Customer
//	In this case we can use the navigational properties to avoid grouping
//	We need to start at the correct table

Customers
	.Select(x => new
	{
		Name = x.FirstName + " " + x.LastName,
		TotalInvoiceAmount = x.Invoices.Sum(i => i.Total)
	})
	.OrderBy(x => x.Name)
	.ToList().Dump();
	
//Same results using a Group By
//	This will be slower with thousands or millions of records
Invoices
	.GroupBy(x => new { FullName = x.Customer.FirstName + " " + x.Customer.LastName })
	.Select(x => new
	{
		//Even if there is just 1 field, when using anonymous datasets you want to
		//	reference it by the name
		Name = x.Key.FullName,
		TotalInvoiceAmount = x.Sum(i => i.Total)
	})
	.ToList().Dump();
	