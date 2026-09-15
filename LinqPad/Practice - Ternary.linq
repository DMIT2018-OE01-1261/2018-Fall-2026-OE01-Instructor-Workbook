<Query Kind="Statements">
  <Connection>
    <ID>6be9b05b-daed-40aa-8405-7854658efede</ID>
    <NamingServiceVersion>3</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <UseMicrosoftDataSqlClient>true</UseMicrosoftDataSqlClient>
    <EncryptTraffic>true</EncryptTraffic>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Contoso</Database>
    <DisplayName>Contoso</DisplayName>
    <MapXmlToString>false</MapXmlToString>
    <DriverData>
      <SkipCertificateCheck>true</SkipCertificateCheck>
    </DriverData>
  </Connection>
</Query>

// ======== In Class Examples ========
// Question 1
// Question: "How would you filter the Employees table to retrieve those with a base rate of less than$30, and return the results as an anonymous data set that includes their full name, department, and  whether they require a salary review, ordered by last name?" 
Employees
	// Since we do not select the last name the order by
	// must be before the select
	.OrderBy(x => x.LastName)
	.Select(x => new
	{
		FullName = x.FirstName + " " + x.LastName,
		Department = x.DepartmentName,
		IncomeCategory = x.BaseRate < 30 ? "Required Review" : "No Review Required"
	})
	.Dump();
	
// Question 2
// Question: "How would you filter the Products table to retrieve items in the 'Music, Movies, and Audio Books' category, and return the results as an anonymous data set that includes the product name, color, and whether color processing is needed, ordered by style name?"
// Note: Not black and white means additional Colour processing may be needed
Products
	.Where(x => x.ProductSubcategory.ProductCategory.ProductCategoryName == "Music, Movies and Audio Books")
	.OrderBy(x => x.StyleName)
	.Select(x => new
	{
		ProductName = x.ProductName,
		Colour = x.ColorName,
		// When using AND or OR in a ternary you should have brackets (it is good practice)
		ColourProcessNeeded = (x.ColorName == "Black" || x.ColorName == "White")  ? "No" : "Yes"
	})
	.Dump();