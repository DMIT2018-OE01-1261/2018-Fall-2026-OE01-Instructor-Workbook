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

Albums
	.OrderBy(x => x.Title)
	.Select( x => new
	{
		Title = x.Title,
		Artist = x.Artist.Name,
		TrackCount = x.Tracks.Count(),
		AlbumLength = x.Tracks.Sum(x => x.Milliseconds) / 1000,
		MinTrackLength = x.Tracks.Min(x => x.Milliseconds) / 1000,
		MaxTrackLength = x.Tracks.Max(x => x.Milliseconds) / 1000,
		AverageTrackLength = x.Tracks.Average(x => x.Milliseconds) / 1000,
		
	})
	.Dump();