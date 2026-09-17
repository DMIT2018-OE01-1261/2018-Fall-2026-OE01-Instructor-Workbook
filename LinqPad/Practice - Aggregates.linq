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
Customers
	.Where(x => x.TotalChildren > 0)
	.Count().Dump();
	
//Question 3
Products
	.Select(x => new
	{
		Name = x.ProductName,
		// Because SUM cannot be used on nothing we have to COUNT first to check if there are records
		TotalOnHand = x.Inventories.Count() == 0 ? 0 : x.Inventories.Sum(i => i.OnHandQuantity)
	})
	.Dump();
	
//Question 4
Promotions
	.Select(x => new
	{
		PromotionID = x.PromotionID,
		PromotionName = x.PromotionName,
		//Since our DiscountAmount is a nullable (has the ?)
		//We do not have to precheck if there are records
		TotalDiscountGiven = x.InvoiceLines.Sum(il => il.DiscountAmount)
	})
	.ToList().Dump();
	
//Question 6
Stores
	.Select(x => new
	{
		StoreID = x.StoreID,
		Name = x.StoreName,
		OldestInvoice = x.Invoices.Any()
			? x.Invoices.Min(i => i.DateKey).ToShortDateString()
			: "N/A"
	})
	.OrderBy(x => x.Name)
	.ToList()
	.Dump();