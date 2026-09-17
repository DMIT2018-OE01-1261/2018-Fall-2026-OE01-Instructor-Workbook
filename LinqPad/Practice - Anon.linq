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

// Question 3
// Req: Audio Category, Pink, and Recording Pen or Bluetooth Headphones
Products
	.Where(x => x.ProductSubcategory.ProductCategory.ProductCategoryName == "Audio"
			&& x.ColorName == "Pink"
			&& (x.ProductSubcategory.ProductSubcategoryName == "Recording Pen" 
				|| x.ProductSubcategory.ProductSubcategoryName == "Bluetooth Headphones"))
	.Select(x => new
	{
		CategoryName = x.ProductSubcategory.ProductCategory.ProductCategoryName,
		SubcategoryName = x.ProductSubcategory.ProductSubcategoryName,
		ProductName = x.ProductName
	})	
	.Dump();
	
// Question 4
Invoices
	.Where(x => x.Customer.Geography.ContinentName == "Europe")
	.Select(x => new
	{
		InvoiceNo = x.InvoiceID,
		InvoiceDate = x.DateKey.Date.ToString(),
		CustomerName = $"{x.Customer.FirstName} {x.Customer.LastName}",
		City = x.Customer.Geography.CityName,
		Country = x.Customer.Geography.RegionCountryName
	})
	.OrderBy(x => x.City)
	.Dump();