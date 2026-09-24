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

//Question 1
ProductSubcategories
	.GroupBy(x => new { x.ProductCategory.ProductCategoryName })

	.Select(x => new 
	{
		CategoryName = x.Key.ProductCategoryName,
		ProductSubcategories = x.Select(x => new
		{
			SubCategoryName = x.ProductSubcategoryName
		}).OrderBy(x => x.SubCategoryName)
	})
	.OrderBy(x => x.CategoryName)
	.ToList()
	.Dump();
	
//Question 1 - No Group By
ProductCategories
	.Select(x => new 
	{
		CategoryName = x.ProductCategoryName,
		ProductSubcategories = x.ProductSubcategories.Select(s => new 
		{
			SubCategoryName = s.ProductSubcategoryName
		}).OrderBy(s => s.SubCategoryName)
	})
	.OrderBy(x => x.CategoryName)
	.ToList()
	.Dump();