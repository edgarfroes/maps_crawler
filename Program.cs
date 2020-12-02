using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Flurl;
using Flurl.Http;

namespace maps_crawler
{
    class Program
    {
        class CoordinateSearch
        {
            public double Lat { get; set; }
            public double Long { get; set; }
            public int Radius { get; set; }
        }

        static async Task Main(string[] args)
        {
            const string googleApiKey = "AIzaSyBE6dZDuo7LhKY5AaWyMGmW42-p7XXCYHc";

            var coordinates = new List<CoordinateSearch>
            {
                // Ceilândia
                new CoordinateSearch
                {
                    Lat = -15.812206,
                    Long = -48.028733,
                    Radius = 5500
                },
                // Taguatinga norte
                new CoordinateSearch
                {
                    Lat = -15.805474,
                    Long = -48.105222,
                    Radius = 4000
                },
                // Taguatinga centro
                new CoordinateSearch
                {
                    Lat = -15.834132,
                    Long = -48.058509,
                    Radius = 1000
                },
                // Taguatinga sul
                new CoordinateSearch
                {
                    Lat = -15.852973,
                    Long = -48.053883,
                    Radius = 2500
                },
                // Águas Claras
                new CoordinateSearch
                {
                    Lat = -15.839393,
                    Long = -48.027081,
                    Radius = 1000
                },
            };

            // var lat0 = -15.830275;
            // var lon0 = -48.049121;

            // GeographyPoint addCoordinate(double dx, double dy)
            // {
            //     const double r = (180 / (Math.PI / 180));

            //     var lat = lat0 + r * (dy / 6378137);
            //     var lon = lon0 + r * (dx / 6378137) / Math.Cos(lat0);

            //     return GeographyPoint.Create(lat, lon);
            // }

            var url = "https://maps.googleapis.com/maps/api/place/textsearch/json"
                .SetQueryParams(new
                {
                    key = googleApiKey,
                    language = "pt-BR",
                    type = "school",
                    // query = "escola",
                });

            string d(double v) => v.ToString().Replace(",", ".");

            var results = new List<Result>();

            void writeCsv()
            {
                var fileName = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                using (var writer = new StreamWriter($"{Environment.CurrentDirectory}/{fileName}.csv"))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(results.OrderBy(x => x.place_id).ToList());
                    }
                }
            }

            string pageToken = null;

            async Task appendToResultsAsync()
            {
                url.SetQueryParam("pagetoken", pageToken);

                Console.WriteLine("");
                Console.WriteLine("============================================================");
                Console.WriteLine(Url.Decode(url.ToString(), false));
                Console.WriteLine("============================================================");
                Console.WriteLine("");

                var response = await url.GetJsonAsync<Response>();

                var newResults = response.results
                    .Where(x => results.Any(y => y.place_id == x.place_id) == false)
                    .ToList();

                results.AddRange(newResults);

                pageToken = response.next_page_token;
            }

            foreach (var coordinate in coordinates)
            {
                url.SetQueryParam("location", $"{d(coordinate.Lat)},{d(coordinate.Long)}");
                url.SetQueryParam("radius", coordinate.Radius);

                await appendToResultsAsync();

                while (string.IsNullOrWhiteSpace((pageToken)) == false)
                {
                    await appendToResultsAsync();
                }
            }

            writeCsv();

            Console.WriteLine($"{results.Count} resultados encontrados");
        }

        class Response
        {
            public string next_page_token { get; set; }
            public List<Result> results { get; set; }
        }

        class Result
        {
            public string place_id { get; set; }
            public string name { get; set; }
            public bool permanently_closed { get; set; }
            public string business_status { get; set; }
            public string reference { get; set; }
            public List<string> types { private get; set; }
            public string found_types => string.Join(",", types);
            public string formatted_address { get; set; }
            public double rating { get; set; }
            public double user_ratings_total { get; set; }
            public string google_url => $"https://www.google.com/maps/place/?q=place_id:{place_id}";
        }
    }
}
