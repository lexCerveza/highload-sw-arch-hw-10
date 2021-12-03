using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Elasticsearch.Net;
using Nest;

const string citiesIndexName = "cities";

var connectionPool = new SingleNodeConnectionPool(new Uri("http://elasticsearch:9200"));
var elasticClient = new ElasticClient(
    new ConnectionSettings(connectionPool)
        .DisableDirectStreaming()
        .ThrowExceptions()
        .EnableDebugMode()
        .DefaultMappingFor<CityInfo>(m => m.IndexName(citiesIndexName)));

var csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    NewLine = Environment.NewLine,
    HasHeaderRecord = false
};

if (elasticClient.Indices.Exists(citiesIndexName).Exists)
{
    elasticClient.Indices.Delete(citiesIndexName);
}

elasticClient.Indices.Create(
    citiesIndexName,
    c => c.Map<CityInfo>(m => m
        .Properties(ps => ps
            .Completion(s => s.Name(n => n.name))
            .Completion(s => s.Name(n => n.country))
            .Completion(s => s.Name(n => n.subcountry))
            .Number(s => s.Name(n => n.geoNameId).Type(NumberType.Long)))));

using var streamReader = new StreamReader("cities.csv");
using var csvReader = new CsvReader(streamReader, csvReaderConfig);

var cities = csvReader.GetRecords<CityInfo>().ToList();

elasticClient.IndexMany<CityInfo>(cities);

Console.WriteLine("Data seeded successfully");

public record CityInfo(string name, string country, string subcountry, long geoNameId);