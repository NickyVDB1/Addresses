using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestJordy.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/fetch-adressen", async ([FromQuery] string postcode) =>
{
    string initialUrl = $"https://api.basisregisters.vlaanderen.be/v2/adressen?Postcode={postcode}";
    List<Adressen> adressenList = new List<Adressen>();
    using (HttpClient client = new HttpClient())
    {

       var apiKey =  builder.Configuration.GetValue<string>("APIKey");
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);

        string currentUrl = initialUrl;

        var maxAttempts = 1;
        var attempts = 0;

        while (!string.IsNullOrEmpty(currentUrl) && attempts < maxAttempts)
        {
            HttpResponseMessage response = await client.GetAsync(currentUrl);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject responseObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);

            JArray adressenArray = (JArray)responseObject["adressen"];

            foreach (var adres in adressenArray)
            {

                var adressen = new Adressen
                {
                    _type = adres["_type"]?.ToString(),
                    identificator = new Identificator
                    {
                        id = adres["identificator"]?["id"]?.ToString(),
                        naamruimte = adres["identificator"]?["naamruimte"]?.ToString(),
                        objectId = adres["identificator"]?["objectId"]?.ToString(),
                        versieId = adres["identificator"]?["versieId"]?.ToString()
                    },
                    detail = adres["detail"]?.ToString(),
                    huisnummer = adres["huisnummer"]?.ToString(),
                    volledigAdres = new VolledigAdres
                    {
                        geografischeNaam = new GeografischeNaam
                        {
                            spelling = adres["volledigAdres"]?["geografischeNaam"]?["spelling"]?.ToString(),
                            taal = adres["volledigAdres"]?["geografischeNaam"]?["taal"]?.ToString()
                        }
                    },
                    adresStatus = adres["adresStatus"]?.ToString()
                };

                adressenList.Add(adressen);
            }
            currentUrl = responseObject["volgende"]?.ToString();

            attempts++;
        }
    }
    Console.WriteLine("Rows: " + adressenList.Count);
    return Results.Ok(adressenList);
});

app.Run();