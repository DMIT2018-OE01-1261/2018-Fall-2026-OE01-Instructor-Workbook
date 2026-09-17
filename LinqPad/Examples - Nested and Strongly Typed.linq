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

//Create two separate questions for Artists and Albums
Artists 	
	.OrderBy(x => x.Name) 	
	.Select(x => new ArtistView
	{ 	
		Name = x.Name,
		// ALWAYS MAKING SURE TO USE THE NAVIGATION PROPERTY
		Albums = x.Albums
					.OrderBy(a => a.Title)
					.Select(a => new AlbumView
					{
						Album = a.Title,
						Label = a.ReleaseLabel,
						Year = a.ReleaseYear,
						Tracks = a.Tracks
							.Select(t => new TrackView
							{
								TrackID = t.TrackId,
								Name = t.Name,
								LengthSeconds = t.Milliseconds/1000
							})
							.ToList()
					}).ToList()
	})
	.Dump();

// Strongly Typed Classes
public class ArtistView
{
	public string Name {get; set;}
	public List<AlbumView> Albums {get; set;}
}

public class AlbumView
{
	public string Album {get; set;}
	public string Label {get; set;}
	public int Year {get; set;}
	public List<TrackView> Tracks {get; set;}
}

public class TrackView
{
	public int TrackID {get; set;}
	public string Name {get; set;}
	public int LengthSeconds {get; set;}
}