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
// A datatype with a ? at the end is nullable (not required)
//	this means the data could be null in that field
//	Best practice is to get the underlying value from the data with .Value
// DateOnly is (year, month, day)
Employees
	.Where(x => x.HireDate.Value >= new DateOnly(2022, 1, 1))
	.OrderBy(x => x.LastName)
	.Dump();
	
// Question 3
// Ordering should ALWAYS be after you filter your data to optimize your queries
//	I WILL DOCK MARKS FOR THIS
Customers
	.Where(x => x.YearlyIncome > 60000 && x.YearlyIncome < 61000)
	.OrderBy(x => x.EmailAddress)
	.Select(x => x.EmailAddress)
	.Dump();
	
// =========== Take Home Practice =========
// Question 4
Stores
	.Where(x => x.StoreName.Contains("No."))
	.OrderBy(x => x.SellingAreaSize)
	.ThenByDescending(x => x.EmployeeCount)
	.ThenBy(x => x.StoreName)
	.Dump();