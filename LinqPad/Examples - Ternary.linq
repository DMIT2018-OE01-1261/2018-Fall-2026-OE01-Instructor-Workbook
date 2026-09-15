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

//List all albums by release label. Any album with no label should be indicated as Unknown.
Albums
	.Select(x => new
	{
		Title = x.Title,
		Label = x.ReleaseLabel == null ? "Unknown" : x.ReleaseLabel,
		Artist = x.Artist.Name,
		Year = x.ReleaseYear 
	})
	.Dump();
	
// List all albums showing the Title, Artist name, Year, and decade of release
//	decades will be:
	// before 1970 - Oldies
	// 1970 - 1979 - 70s
	// 1980 - 1989 - 80s
	// 1990 - 1999 - 90s
	// 2000 and newer - Modern
// Order by the decade
	// Note: Order by Year to have Oldies show up first.
	//	Since Oldies is a string field O will be ordered after the numbers and M
	//	Use Year to make sure it is first, cause Year is numeric

Albums
	.Select(x => new
	{
		Title = x.Title,
		Artist = x.Artist.Name,
		Year = x.ReleaseYear,
		Decade = x.ReleaseYear < 1970 ? "Oldies" :
					x.ReleaseYear < 1980 ? "70s" :
					x.ReleaseYear < 1990 ? "80s" :
					x.ReleaseYear < 2000 ? "90s" : "Modern"
	})
	.OrderBy(x => x.Year)
	.Dump();