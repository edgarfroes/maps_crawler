using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using CsvHelper;
using Flurl;
using Flurl.Http;
using Microsoft.Spatial;
using Newtonsoft.Json;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Serialization;

namespace maps_crawler
{
    class Program
    {
        private static string GoogleApiKey = "AIzaSyDml6CQfRueCsbFkoeYzD4SCXEsFEBBuKc";

        class CoordinateSearch
        {
            public double Lat { get; set; }
            public double Long { get; set; }
            public int Radius { get; set; }
        }

        static List<KeyValuePair<string, string>> ras = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Plano Piloto", "plano_piloto"),
            new KeyValuePair<string, string>("Taguatinga", "taguatinga"),
            new KeyValuePair<string, string>("Brazlândia", "brazlandia"),
            new KeyValuePair<string, string>("Brazlandia", "brazlandia"),
            new KeyValuePair<string, string>("Sobradinho 2", "sobradinho_2"),
            new KeyValuePair<string, string>("Sobradinho II", "sobradinho_2"),
            new KeyValuePair<string, string>("Sobradinho", "sobradinho"),
            new KeyValuePair<string, string>("Planaltina", "planaltina"),
            new KeyValuePair<string, string>("Paranoá", "paranoa"),
            new KeyValuePair<string, string>("Paranoa", "paranoa"),
            new KeyValuePair<string, string>("Núcleo Bandeirante", "nucleo_bandeirante"),
            new KeyValuePair<string, string>("Nucleo Bandeirante", "nucleo_bandeirante"),
            new KeyValuePair<string, string>("Ceilândia", "ceilandia"),
            new KeyValuePair<string, string>("Ceilandia", "ceilandia"),
            new KeyValuePair<string, string>("Guará", "guara"),
            new KeyValuePair<string, string>("Guara", "guara"),
            new KeyValuePair<string, string>("Cruzeiro", "cruzeiro"),
            new KeyValuePair<string, string>("Samambaia", "samambaia"),
            new KeyValuePair<string, string>("Santa Maria", "santa_maria"),
            new KeyValuePair<string, string>("São Sebastião", "sao_sebastiao"),
            new KeyValuePair<string, string>("Sao Sebastiao", "sao_sebastiao"),
            new KeyValuePair<string, string>("Recanto das Emas", "recanto_das_emas"),
            new KeyValuePair<string, string>("Lago Sul", "lago_sul"),
            new KeyValuePair<string, string>("Lago Norte", "lago_norte"),
            new KeyValuePair<string, string>("Candangolândia", "candangolandia"),
            new KeyValuePair<string, string>("Candangolandia", "candangolandia"),
            new KeyValuePair<string, string>("Águas Claras", "aguas_claras"),
            new KeyValuePair<string, string>("Aguas Claras", "aguas_claras"),
            new KeyValuePair<string, string>("Riacho Fundo II", "riacho_fundo_2"),
            new KeyValuePair<string, string>("Riacho Fundo 2", "riacho_fundo_2"),
            new KeyValuePair<string, string>("Riacho Fundo", "riacho_fundo"),
            new KeyValuePair<string, string>("Sudoeste", "sudoeste"),
            new KeyValuePair<string, string>("Varjão", "varjao"),
            new KeyValuePair<string, string>("Varjao", "varjao"),
            new KeyValuePair<string, string>("Park Way", "park_way"),
            new KeyValuePair<string, string>("SCIA", "scia"),
            new KeyValuePair<string, string>("Sobradinho", "sobradinho"),
            new KeyValuePair<string, string>("Jardim Botânico", "jardim_botanico"),
            new KeyValuePair<string, string>("Jardim Botanico", "jardim_botanico"),
            new KeyValuePair<string, string>("Itapoã", "itapoa"),
            new KeyValuePair<string, string>("Itapoa", "itapoa"),
            new KeyValuePair<string, string>("SIA", "sia"),
            new KeyValuePair<string, string>("Vicente Pires", "vicente_pires"),
            new KeyValuePair<string, string>("Fercal", "fercal"),
            new KeyValuePair<string, string>("Sol Nascente", "sol_nascente"),
            new KeyValuePair<string, string>("Arniqueira", "arniqueira"),
            new KeyValuePair<string, string>("Octogonal", "octogonal"),
            new KeyValuePair<string, string>("Por do Sol", "por_do_sol"),
            new KeyValuePair<string, string>("Valparaíso de Goiás", "valparaiso_de_goias"),
            new KeyValuePair<string, string>("Valparaíso de Goias", "valparaiso_de_goias"),
            new KeyValuePair<string, string>("Cidade Ocidental", "cidade_ocidental"),
            new KeyValuePair<string, string>("Luziânia", "luziania"),
            new KeyValuePair<string, string>("Luziania", "luziania"),
            new KeyValuePair<string, string>("Formosa", "formosa"),
            new KeyValuePair<string, string>("Jardim Brasília", "jardim_brasilia"),
            new KeyValuePair<string, string>("Jardim Brasilia", "jardim_brasilia"),
            new KeyValuePair<string, string>("Gama", "gama")
        };

        class Point
        {
            public Point(double x, double y)
            {
                Latitude = x;
                Longitude = y;
            }

            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        // static bool IsInPolygon(List<Point> poly, Point point)
        // {
        //     var coef = poly.Skip(1).Select((p, i) =>
        //                                     (point.Longitude - poly[i].Longitude) * (p.Latitude - poly[i].Latitude)
        //                                   - (point.Latitude - poly[i].Latitude) * (p.Longitude - poly[i].Longitude))
        //                             .ToList();

        //     if (coef.Any(p => p == 0))
        //         return true;

        //     for (int i = 1; i < coef.Count(); i++)
        //     {
        //         if (coef[i] * coef[i - 1] < 0)
        //             return false;
        //     }
        //     return true;
        // }

        static bool IsInPolygon(List<Point> polygon, Point testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Latitude < testPoint.Latitude && polygon[j].Latitude >= testPoint.Latitude || polygon[j].Latitude < testPoint.Latitude && polygon[i].Latitude >= testPoint.Latitude)
                {
                    if (polygon[i].Longitude + (testPoint.Latitude - polygon[i].Latitude) / (polygon[j].Latitude - polygon[i].Latitude) * (polygon[j].Longitude - polygon[i].Longitude) < testPoint.Longitude)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        class FeatureCollection
        {
            public List<Feature> features { get; set; }
        }

        class Feature
        {
            public string id { get; set; }
            public FeatureProperties properties { get; set; }
            public FeatureGeometry geometry { get; set; }
        }

        class FeatureProperties
        {
            public string name { get; set; }
        }

        class FeatureGeometry
        {
            public string name { get; set; }
            public List<List<List<List<double>>>> coordinates { get; set; }
        }

        static async Task Main(string[] args)
        {
            var dataJsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "data.json"));
            var polygons = System.Text.Json.JsonSerializer.Deserialize<FeatureCollection>(dataJsonString);

            var jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "2020-12-26 08:01:33.json"));

            var items = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, FirebaseSchool>>(jsonString);

            foreach (var item in items)
            {
                foreach (var feature in polygons.features)
                {
                    var poly = feature.geometry.coordinates
                        .SelectMany(x => x)
                        .SelectMany(x => x)
                        .Select(x => new Point(x[1], x[0]))
                        .ToList();

                    if (IsInPolygon(poly, new Point(item.Value.latitude, item.Value.longitude)))
                    {
                        item.Value.ra = feature.properties.name;
                        item.Value.ra_code = feature.id;
                        break;
                    }
                }

                if (item.Value.ra == null)
                {
                    item.Value.ra =
                        ras
                            .Where(x => item.Value.name.Contains(x.Key))
                            .Select(x => x.Key)
                            .FirstOrDefault()
                        ??
                        ras
                            .Where(x => item.Value.address.Contains(x.Key))
                            .Select(x => x.Key)
                            .FirstOrDefault()
                        ;

                    if (item.Value.ra == null)
                    {
                        item.Value.banned = true;
                        continue;
                    }
                }

                if (item.Value.ra != null && item.Value.ra_code == null)
                {
                    item.Value.ra_code = item.Value.ra.ToLower().RemoveAccents().Replace(" ", "_");
                }

                foreach (var ra in ras)
                {
                    if (item.Value.name.ToLower().Contains("de " + ra.Key.ToLower()))
                    {
                        var newString = item.Value.name.ToLower().Split("de " + ra.Key.ToLower())[0];

                        item.Value.name = item.Value.name.Substring(0, newString.Length);
                    }

                    if (item.Value.name.ToLower().Contains("do " + ra.Key))
                    {
                        var newString = item.Value.name.ToLower().Replace("de " + ra.Key, "");

                        item.Value.name = item.Value.name.Substring(0, newString.Length);
                    }

                    if (item.Value.name.StartsWith("EC "))
                    {
                        item.Value.name = item.Value.name.Replace("EC ", "Escola Classe ");
                    }

                    if (item.Value.name.StartsWith("CED "))
                    {
                        item.Value.name = item.Value.name.Replace("CED ", "Centro Educacional ");
                    }

                    item.Value.name = item.Value.name.Trim();
                }
            }

            var newDict = new Dictionary<string, FirebaseSchool>();

            var i = 1;

            foreach (var item in items.Where(x => x.Value.banned == false).ToList())
            {
                if (item.Value.name.ToLower().StartsWith("caixa escolar"))
                {
                    continue;
                }

                newDict.Add((i).ToString(), item.Value);
                i++;
            }

            string json = JsonConvert.SerializeObject(newDict, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            //write string to file
            System.IO.File.WriteAllText(Path.Combine(Environment.CurrentDirectory, $"final.json"), json);
        }


        static async Task MainOld(string[] args)
        {
            List<string> terms = new List<string>
            {
                "",
                "escola",
                "centro educacional",
                "centro de ensino",
                "CED",
                "colégio",
                "escola classe",
                "escola infantil",
                "colégio infantil",
                "maternal",
                "infantil",
                "jardim",
            };

            List<string> schools = new List<string>
            {
"CEPI Ipê Branco QNN 13, Área Especial, Ceilândia Oeste, Brasília/DF – CEP: 72225-130",
"CEPI Estrela do Cerrado QNP 28 Área Especial 02, Ceilândia, Brasília/DF – CEP: 72235-800",
"CEPI Papagaio EQNP 06/10 Área Especial, P Sul, Ceilândia, Brasília/DF – CEP: 72230-500",
"CEPI Capim Dourado QNO 10 Área Especial A, Ceilândia, Brasília/DF – CEP: 72255-001",
"Centro Interescolar de Línguas QNM 13 Área Especial, Ceilândia, Brasília/DF – CEP: 72215-130",
"Centro de Educação Profissional EQNN 14 Área Especial, Guariroba, Ceilândia, Brasília/DF – CEP: 72220-140",
"Centro de Ensino Especial 02 QNO 12 Área Especial G, Setor O, Ceilândia, Brasília/DF – CEP: 72255-207",
"Centro de Ensino Especial 01 EQNP 10/14 Área Especial, P Sul, Ceilândia, Brasília/DF – CEP: 72255-207",
"Centro de Ensino Médio 12 QNP 12 Área Especial, P Norte, Ceilândia, Brasília/DF – CEP: 72240-130",
"Centro de Ensino Médio 10 QES Área Especial 01, Ceilândia, Brasília/DF – CEP: 72265-000",
"Centro de Ensino Médio 09 EQNO 03/05 Área Especial, Setor O, Ceilândia, Brasília/DF – CEP: 72250-510",
"Centro de Ensino Médio 04 QNN 14 Área Especial, Ceilândia Sul, Brasília/DF – CEP: 72220-140",
"Centro de Ensino Médio 03 QNM 13 Área Especial, Ceilândia, Brasília/DF – CEP: 72215-130",
"Centro de Ensino Médio 02 QNM 14 Área Especial, Ceilândia, Brasília/DF – CEP: 72210-140",
"Centro Educacional Incra 09 Núcleo Rural Alexandre Gusmão, BR 070 GL 03, Incra 09, Ceilândia, Brasília/DF – CEP: 72701-991",
"Centro Educacional 16 QNQ 3 Área Especial B, Ceilândia, Brasília/DF – CEP: 72225-580",
"Centro Educacional 15 QNO 17 Conjunto B lote 1, Ceilândia, Brasília/DF – CEP: 72260-890",
"Centro Educacional 14 EQNO 11/13 AE, Ceilândia, Brasília/DF – CEP: 72255-510",
"Centro Educacional 11 EQNP 01/05 Área Especial, Ceilândia, Brasília/DF – CEP: 72240-500",
"Centro Educacional 07 QNN 13 Área Especial, Ceilândia, Brasília/DF – CEP: 72225-130",
"Centro Educacional 06 QNP 16 Área Especial, Ceilândia, Brasília/DF – CEP: 72231-600",
"CEF Prof Maria do Rosário Gondim da Silva EQNN 21/23 Área Especial, Ceilândia-Norte, Brasília/DF – CEP: 72225-230",
"CEF Boa Esperança BR 070 DF 180/190 Núcleo Rural Boa Esperança, Ceilândia, Brasília – DF – CEP: 72227-991",
"Centro de Ensino Fundamental 32 SHPS 500/700 Área Especial, Ceilândia, Brasília/DF – CEP: 72238-000",
"Centro de Ensino Fundamental 34 QNO 19 Conjunto “B” Lote 1, Setor O, Expansão, Ceilândia, Brasília/DF – CEP: 72261-062",
"Centro de Ensino Fundamental 33 QNP 08/12 Área Especial, P Sul, Ceilândia, Brasília/DF – CEP: 72231-200",
"Centro de Ensino Fundamental 35 EQNN 01/03 Área Especial, Ceilândia, Brasília/DF – CEP: 72225-520",
"Centro de Ensino Fundamental 31 QNO 17 Área Especial, Expansão do Setor O, Ceilândia, Brasília/DF – CEP: 72260-778",
"Centro de Ensino Fundamental 30 Condomínio Agrícola Privê Lucena Roriz Mod 7, Ceilândia, Brasília/DF – CEP: 72268-000",
"Centro de Ensino Fundamental 28 QNP 21 Área Especial – Setor Habitacional Sol Nascente, Brasília/DF – Cep: 72215-000",
"Centro de Ensino Fundamental 27 QNR 01 Área Especial 03, Setor R, Ceilândia, Brasília/DF – CEP: 72275-126",
"Centro de Ensino Fundamental 26 EQNO 5/7 Área Especial, Setor O, Ceilândia, Brasília/DF – CEP: 72250-500",
"Centro de Ensino Fundamental 25 QNP 09 Área Especial, Ceilândia, Brasília/DF – CEP: 72240-813",
"Centro de Ensino Fundamental 20 EQNM 08/10, Ceilândia Norte, Brasília/DF – CEP: 72210-540",
"Centro de Ensino Fundamental 19 EQNN 18/20 AE B, Ceilândia Sul, Brasília/DF – CEP: 72220-550",
"Centro de Ensino Fundamental 18 QNP 10 Área Especial, P Sul, Ceilândia, Brasília/DF – CEP: 72231-100",
"Centro de Ensino Fundamental 16 EQNM 22/24 Área Especial, Ceilândia Norte, Brasília/DF – CEP: 72210-570",
"Centro de Ensino Fundamental 14 EQNP 28/32 Área Especial, Ceilândia, Brasília/DF – CEP: 72235-808",
"Centro de Ensino Fundamental 13 EQNP 30/34 Área Especial, Ceilândia, Brasília/DF – CEP: 72236-500",
"Centro de Ensino Fundamental 12 EQNO 02/04 Área Especial, Ceilândia, Brasília/DF – CEP: 72250-530",
"Centro de Ensino Fundamental 11 EQNN 24/26 Área Especial, Ceilândia, Brasília/DF – CEP: 72220-580",
"Centro de Ensino Fundamental 10 EQNN 23/25 Área Especial, Ceilândia, Brasília/DF – CEP: 72225-590",
"Centro de Ensino Fundamental 07 EQNM 5/7 Área Especial, Ceilândia, Brasília/DF – CEP: 71215-540",
"Centro de Ensino Fundamental 04 EQNM 21/23 Área Especial, Ceilândia, Brasília/DF – CEP: 72215-580",
"Centro de Ensino Fundamental 02 EQNM 01/03 Área Especial, Ceilândia, Brasília/DF – CEP: 72250-520",
"Escola Parque Anísio Teixeira QNM 27 Módulo B Área Especial, Ceilândia Sul, Brasília/DF – CEP: 72215-272",
"Escola Classe P Norte SHSN VC 311, Rua da Cascalheira S/N, P Norte, Ceilândia, Brasília/DF – CEP: 72227-990",
"Escola Classe Lajes da Jiboia BR 060 DF 190 km 11, Núcleo Rural Lajes da Jiboia, Ceilândia, Brasília/DF – CEP: 72210-000",
"Escola Classe Jiboia BR 060 BSB 280 DF 190 Km 06, Fazenda Dois Irmãos, Ceilândia, Brasília/DF – CEP: 72227-991",
"Escola Classe Córrego das Corujas Núcleo Rural Machado, BR 070, Ceilândia, Brasília/DF – CEP: 72217-000",
"Escola Classe 68 QNR 02 lote 2, Ceilândia, Brasília/DF – CEP: 72275-250",
"Escola Classe 56 QNO 18 Conjunto I, Área especial, Ceilândia, Brasília/DF – CEP: 72260-897",
"Escola Classe 66 Córrego das Corujas, Área Especial S/N, Ceilândia, Brasília/DF – CEP: 72275-170",
"Escola Classe 65 QNR 02 Área Especial 4, Ceilândia, Brasília/DF – CEP: 72275-308",
"Escola Classe 64 EQNM 17/19 Área Especial, Ceilândia, Brasília/DF – CEP: 72215-560",
"Escola Classe 62 QNQ 1 Área Especial, Ceilândia, Brasília/DF – CEP: 72270-100",
"Escola Classe 61 QNQ 4 Área Especial, Ceilândia, Brasília/DF – CEP: 72270-400",
"Escola Classe 59 QNN 36 Área Especial 02, Ceilândia, Brasília/DF – CEP: 72220-360",
"Escola Classe 55 QNO 19 Conjunto E, Área Especial, Ceilândia, Brasília/DF – CEP: 72261-263",
"Escola Classe 52 EQNP 32/36 Área Especial, P Sul, Ceilândia, Brasília/DF – CEP: 72236-530",
"Escola Classe 50 EQNP 24/28 Área Especial, Ceilândia, Brasília/DF – CEP: 72235-520",
"Escola Classe 48 EQNP 26/30 Área Especial, P Sul, Ceilândia, Brasília/DF – CEP: 72235-540",
"Escola Classe 47 EQNP 22/26 Área Especial, Ceilândia, Brasília/DF – CEP: 72235-500",
"Escola Classe 46 EQNP 16/20 Área Especial, Ceilândia, Brasília/DF – CEP: 72231-560",
"Escola Classe 45 EQNP 12/16 Área Especial, Ceilândia, Brasília/DF – CEP: 72231-520",
"Escola Classe 43 EQNP 14/18 Área Especial, Ceilândia, Brasília/DF – CEP: 72230-145",
"Escola Classe 40 EQNP 07/11 Área Especial, P Norte, Ceilândia, Brasília/DF – CEP: 72240-540",
"Escola Classe 39 EQNP 11/15 Área Especial, P Norte, Ceilândia, Brasília/DF – CEP: 72241-520",
"Escola Classe 38 EQNP 15/19 Área Especial, Ceilândia, Brasília/DF – CEP: 72241-560",
"Escola Classe 36 EQNP 05/09 Área Especial, P Norte, Ceilândia, Brasília/DF – CEP: 72240-415",
"Escola Classe 35 EQNP 09/13 Área Especial, Setor P. Norte, Ceilândia, Brasília/DF – CEP: 72240-560",
"Escola Classe 34 EQNP 13/17 Área Especial, P Norte, Ceilândia, Brasília/DF – CEP: 72241-540",
"Escola Classe 33 QNO 13/15 Área Especial, Setor O, Ceilândia, Brasília/DF – CEP: 72255-520",
"Escola Classe 31 EQNO 9/11 Área Especial, Setor O, Ceilândia, Brasília/DF – CEP: 72252-500",
"Escola Classe 29 EQNN 19/21 Área Especial, Ceilândia, Brasília/DF – CEP: 72225-570",
"Escola Classe 28 QNN 17/19, Ceilândia, Brasília/DF – CEP: 72225-560",
"Escola Classe 27 EQNN 7/9 Área Especial, Ceilândia, Brasília/DF – CEP: 72225-550",
"Escola Classe 26 EQNN 03/05 Área Especial, Ceilândia Norte, Brasília/DF – CEP: 72225-530",
"Escola Classe 25 EQNN 22/24 Área Especial, Ceilândia Sul, Brasília/DF – CEP: 72220-570",
"Escola Classe 24 EQNN 20/22 Área Especial, Ceilândia Sul, Brasília/DF – CEP: 72220-560",
"Escola Classe 22 EQNN 06/08 Área Especial, Ceilândia Sul, Brasília/DF – CEP: 72220-530",
"Escola Classe 21 EQNN 4/6 Área Especial, Ceilândia, Brasília/DF – CEP: 72220-520",
"Escola Classe 20 EQNN 2/4 Área Especial, Ceilândia, Brasília/DF – CEP: 72220-510",
"Escola Classe 19 EQNM 07/09 Área Especial s/n° Ceilândia, Brasília/DF – CEP: 72215-550",
"Escola Classe 18 EQNM 03/05 Área Especial, Ceilândia, Brasília/DF – CEP: 72215-535",
"Escola Classe 17 EQNO 1/3, Área Especial, Setor O, Ceilândia, Brasília/DF – CEP: 72250-500",
"Escola Classe 16 EQNO 04/06 Área Especial, Setor O, Ceilândia, Brasília/DF – CEP: 72250-540",
"Escola Classe 15 EQNN 08/10, Área Especial, Ceilândia, Brasília/DF – CEP: 72220-540",
"Escola Classe 13 EQNM 24/26 Área Especial, Ceilândia, Brasília/DF – CEP: 72210-580",
"Escola Classe 12 EQNM 20/22, Área Especial, Ceilândia Norte, Brasília/DF – CEP: 72210-560",
"Escola Classe 11 EQNM 06/08 Área Especial, Ceilândia, Brasília/DF – CEP: 72210-080",
"Escola Classe 10 EQNM 02/04 Área Especial, Ceilândia Norte, Brasília/DF – CEP: 72210-510",
"Escola Classe 08 EQNM 05/07 Área Especial 01, Ceilândia Norte, Brasília/DF – CEP: 72225-540",
"Escola Classe 06 EQNM 04/06 Ceilândia Norte, Brasília/DF – CEP: 72210-520",
"Escola Classe 03 EQNM 18/20 Área Especial, Ceilândia Norte, Brasília/DF – CEP: 72210-550",
"Escola Classe 02 EQNM 19/21 Área Especial, Ceilândia Sul, Brasília/DF – CEP: 72215-220",
"Escola Classe 01 EQNM 23/25 Área Especial, Ceilândia Sul, Brasília/DF – CEP: 72215-590",
"CAIC Bernardo Sayão QNN 28 Área Especial H, I, J, K, Ceilândia Sul, Brasília/DF – CEP: 72220-280",
"CAIC Anísio Teixeira QNO 10 Área Especial, Setor O, Ceilândia, Brasília/DF – CEP: 72240-001",
"Centro de Educação Infantil 01 QNP 14 Área Especial, Ceilândia, Brasília/DF – CEP: 72231-400",
"Centro Interescolar de Línguas CIL Praça 2 Entre qd 16/18 Area especial, St. Central – Gama, Brasília – DF – 72405-165",
"Centro de Ensino Especial 01 St. Central EQ 55/56 – Gama, Brasília – DF – 72405-555",
"CEM Integrado a Educação Profissional do Gama Eq 12/16 Ae, St. Oeste – Gama, Brasília – DF – 70297-400",
"Centro De Ensino Médio 03 St. Sul EQ 5/11 – Gama, Brasília – DF – 72410-115",
"Centro De Ensino Médio 02 St. Central Edifício Phenícia – Setor Oeste, DF – 70040-020",
"Centro De Ensino Médio 01 Eq 18/21 Ae, St. Leste – Gama, Brasília – DF – 70297-400",
"Centro Educacional Gesner Teixeira Rua Das Dálias Lt 2 A 6 – Gama, DF – 72104-970",
"Centro Educacional Engenho das Lajes BR-060, Km 30 3100 – Gama, Brasília – DF – 72457-996",
"Centro Educacional Casa Grande MA 16 Chacará 01 Pte. Alta Norte, Núcleo Rural Casa Grande – Gama, DF – 72428-010",
"Centro Educacional 08 St. Sul Q 4 – Gama, Brasília – DF – 72415-209",
"Centro Educacional 07 EQ 15/17 praça 01 Lote 3 s. central – Pte. Alta Norte (Gama), Brasília – DF – 72405-155",
"Centro Educacional 06 EQ 02/07 AE S. LESTE – Setor Leste, Brasília – DF – 72450-027",
"Centro de Ensino Fundamental Tamanduá DF 180 KM. 61 PONTE ALTA – Pte. Alta Norte (Gama), Brasília – DF – 72401-970",
"Centro de Ensino Fundamental Ponte Alta Norte DF 475 KM 05 N.R.P.N – Pte. Alta Norte (Gama), Brasília – DF – 72400-000",
"Centro de Ensino Fundamental Ponte Alta do Baixo BR-290 – KM 14 – ENTRADA A ESQUERDA PONTE ALTA DOBAIXO – DF – 72400-000",
"Centro de Ensino Fundamental 15 St. Sul Q 5 – Gama, Brasília – DF – 72418-300",
"Centro de Ensino Fundamental 11 St. B Sul QSB 10, Condomínio da Qsb 02 Área Especial 5/6 Setor B Sul – St. Sul, Brasília – DF – 72410-100",
"Centro de Ensino Fundamental 10 EQ. 16/26 E 19/22, St. Oeste – Gama Oeste, Brasília – DF – 72420-167",
"Centro de Ensino Fundamental 08 St. Sul Q 2 – Pte. Alta Norte (Gama), Brasília – DF – 72405-610",
"Centro de Ensino Fundamental 05 St. Oeste Q 26 – Gama, Brasília – DF – 72420-260",
"Centro de Ensino Fundamental 04 AE Praça 3 Setor Leste, Entre Quadras 29/33 – Pte. Alta Norte (Gama), Brasília – DF – 72460-290",
"Centro de Ensino Fundamental 03 Eq 06/11, St. Leste – Gama, Brasília – DF – 72450-065",
"Centro de Ensino Fundamental 01 EQ. 01/02 AE S. NORTE – Pte. Alta Norte (Gama), Brasília – DF – 72430-150",
"Escola Classe Ponte Alta de Cima DF 290 FAZ.PONTE ALTA DE CIMA KM. 14 – Pte. Alta Norte (Gama), Brasília – DF – 72400-000",
"Escola Classe Corrégo Barreiro Km 08 – Pte. Alta Norte (Gama), Brasília – DF – 72000-000",
"Escola Classe 29 St. Sul Q 15 – Jardim Boa Vista II, Brasília – DF – 72410-970",
"Escola Classe 28 Q. A LT B S. OESTE, Gama Oeste, Brasília – DF – 72420-410",
"Escola Classe 22 EQ 33/49 AE 1 S. CENTRAL – Pte. Alta Norte (Gama), Brasília – DF – 72405-335",
"Escola Classe 21 EQ 44/45 A/E – Setor Leste – Gama Leste, Brasília – DF – 72465-445",
"Escola Classe 19 St. Leste EQ 30/49 – Gama, Brasília – DF – 72460-300",
"Escola Classe 18 QD 05 – AE – CJ D, S. SUL – Gama, Brasília – DF – 72410-304",
"Escola Classe 17 Q. 07 AE, St. Sul – Gama, Brasília – DF – 72410-070",
"Escola Classe 16 Q 06 AE S. Sul Setor Sul Quadra 6 Conjunto B C D E – Pte. Alta Norte, Gama, Brasília/DF – 72415-060",
"Escola Classe 15 Q 02 AE S. NORTE – Pte. Alta Norte (Gama), Brasília – DF – 72430-230",
"Escola Classe 14 EQ. 29/33 AE S. LESTE – Pte. Alta Norte (Gama), Brasília – DF – 72460-295",
"Escola Classe 12 Q. 01 AE S. NORTE – Pte. Alta Norte (Gama), Brasília – DF – 72430-130",
"Escola Classe 10 EQ. 10/21 AE, St. Oeste – Pte. Alta Norte (Gama), Brasília – DF – 72425-107",
"Escola Classe 09 SSU Q 3 – Gama, Brasília – DF – 72410-209",
"Escola Classe 07 Q 10 AE S. SUL – Setor Sul Q 12 – Pte. Alta Norte (Gama), Brasília – DF – 72415-100",
"Escola Classe 06 EQ 09/19 AE S. OESTE – Pte. Alta Norte (Gama), Brasília – DF – 72425-097",
"Escola Classe 03 S. LESTE EQ 10/15 – Pte. Alta Norte (Gama), Brasília – DF – 72450-107",
"Escola Classe 02 EQ 02/04 AE S. LESTE – Pte. Alta Norte (Gama), Brasília – DF – 72425-025",
"Escola Classe 01 EQ. 18/21 PR. 02, St. Leste – Gama, Brasília – DF – 72460-180",
"CAIC Carlos Castello Branco EQ. 20/23 AE Setor Oeste – Pte. Alta Norte, Gama, Brasília/DF – 72420-205",
"Centro de Educação Infantil 01 QD 09 AE setor Sul – Pte. Alta Norte (Gama), Brasília – DF – 72410-530",
"Jardim de Infância 06 EQ. 27/17 AE Setor Oeste – Pte. Alta Norte (Gama), Brasília – DF – 72420-177",
"Jardim de Infância 05 Q 10 Area Especial, St. Sul – Gama, DF – 72415-521",
"Jardim de Infância 04 SOE AE Oeste – Gama, Brasília – DF – 72420-227",
"Jardim de Infância 03 St. Leste EQ 3/5 AE Oeste – Gama, Brasília – DF – 72450-035",
"Jardim de Infância 02 EQ 31/32 – Gama, Brasília – DF – 72460-315",
"Escola Técnica do Guará Professora Teresa Ondina Maltese Guará II – Guará, Brasília – DF – 70297-400",
"Centro Interescolar de Línguas CILG, Qe 7 Ae Q, Guará I – Guará, DF – 71020-007",
"Centro de Ensino Especial Guará I QE 20 – Guará, Brasília – DF – 71015-037",
"Centro Educacional 01 da Estrutural St. Central – Guará, Brasília – DF – 70297-400",
"Centro Educacional 04 Qe 09, Ae D – Guará, Brasília – DF – 70297-400",
"Centro Educacional 03 Guará II Área Especial B – Guará, Brasília – DF – 71050-175",
"Centro de Ensino Médio 01 Qe 07 Ae M – Guará, DF – 71020-007",
"Centro Educacional 01 Entrequadras 34/36 Conjunto F, Guará II – Guará, Brasília – DF – 71065-023",
"Centro de Ensino Fundamental 03 da Estrutural Trecho 2, de – 1505 ao fim,lado ímpar lote – 1815 – a – 1825, SIA – Guará, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 02 da Estrutural Qd 02 – Ae – Conjunto 1, Scia, 2 – Guará, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 10 QE 44/46 Area especial 5, Guará II – Guará, Brasília – DF – 71070-460",
"Centro de Ensino Fundamental 08 Guará II EQ 13/15 – Guará, Brasília – DF – 71050-135",
"Centro de Ensino Fundamental 05 Eq 32/34 Lt B Ae, Guará II – Guará, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 04 Guará I QE 12 – Guará, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 02 Área Especial Q, Guará I, Brasília – DF – 71010-027",
"Centro de Ensino Fundamental 01 Guará I QI 8 – Guará, Brasília – DF – 70297-400",
"Jardim de Infância 316 Sul SQS 316 AREA ESPECIAL S/N, SHCS, Brasília – DF – 70387-110",
"Jardim de Infância 314 Sul SHCS SQS 314, Brasília – DF – 70383-000",
"Jardim de Infância 308 Sul SQS308 – Asa Sul, Brasília – DF – 70390-100",
"Jardim de Infância 305 Sul SQS 305 – Ae, SHCS, Brasília – DF – 70352-000",
"Jardim de Infância 303 Sul SQS 303 – Ae, SHCS, Brasília – DF – 70336-000",
"Jardim de Infância 208 Sul SQS208 -ae Sqs 208 -ae, SHCS, Brasília – DF – 70254-000",
"Jardim de Infância 114 Sul SHCS SQS 114, Brasília – DF – 70377-000",
"Jardim de Infância 108 Sul SQS 108 – AE, SHCS, Brasília – DF – 70347-000",
"Jardim de Infância 102 Sul SQS 102 – AE, SHCS, Brasília – DF – 70330-000",
"Centro de Ensino Especial 01 SGAS I St. de Grandes Áreas Sul 912 – Asa Sul, Brasília – DF – 70390-100",
"Centro de Ensino Especial 02 SGAS II SGAS 612 – Asa Sul, Brasília – DF – 70200-715",
"Centro de Ensino Especial de Deficientes Visuais SGAS II Quadra 612 Sul – Asa Sul, Brasília – DF – 70200-000",
"Centro Educacional do Lago Sul St. de Habitações Individuais Sul – Lago Sul, Brasília – DF – 70297-400",
"Centro Educacional Gisno SGAN 907 s/n md A, SGAN, Brasília – DF – 70790-070",
"Centro Educacional do Lago Norte SHIN CA 2 lote 24 – Lago Norte, Brasília – DF – 71503-502",
"Centro Educacional 02 Cruzeiro LT 2 – Cruzeiro Velho, Brasília – DF – 70640-570",
"Centro Ensino Médio Setor Oeste Mod D – Asa Sul, Brasília – DF – 70390-120",
"Centro Ensino Médio Setor Leste Sgas 611 612 – Conjunto e – Sn Sgas 611 612 – Conjunto e – Sn – Asa Sul, DF – 70200-715",
"Centro Ensino Médio Paulo Freire SGAN 610 – Módulo A – Asa Norte, Brasília – DF – 70860-100",
"Centro Ensino Médio Asa Norte CEAN SGAN 606 – MOD G/H, Brasília – DF – 70840-060",
"Centro Ensino Médio Elefante Branco SGAS 908, Módulos 25/26, Brasília – DF – 70390-080",
"Centro Ensino Médio Integrado – Cruzeiro SRES – Cruzeiro Velho, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental Polivalente SGAS I St. de Grandes Áreas Sul 913 Sgas 913 – Mod 57 58 – Asa Sul, Brasília – DF – 70390-130",
"Centro de Ensino Fundamental Gan SGAN 603 – Asa Norte, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental Caseb A 27 28 – Asa Sul, Brasília – DF – 70390-090",
"Centro de Ensino Fundamental 410 Norte SHCN SQN 410, Brasília – DF – 70865-000",
"Centro de Ensino Fundamental 405 Sul SHCS SQS 405 – Brasília, DF – 70297-400",
"Centro de Educação Infantil 316 Norte SQN 316 – CEF 316 Norte, SHCN, Brasília – DF – 70775-000",
"Centro de Ensino Fundamental 306 Norte Sqn 306 – Ae, Area Especial Sqn 306 – Ae, St. de Áreas Especiais Norte, DF – 70745-000",
"Centro de Ensino Fundamental 214 Sul SHCS SQS 214 – Asa Sul, Brasília – DF – 70293-000",
"Centro de Ensino Fundamental 104 Norte SHCN SQN 104, Brasília – DF – 70733-000",
"Centro de Ensino Fundamental 102 Norte Sqn 102 – Ae, SHCN, Brasília – DF – 70722-010",
"Centro de Ensino Fundamental 07 Via W 5 Norte SGAN 912 Modulo A/B – Asa Norte, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 06 SHIS QI 15 – Lago Sul, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 05 Área Especial, Shcs, 408 – Asa Sul, Brasília – DF – 70257-000",
"Centro de Ensino Fundamental 04 SHCS SQS 113 – Asa Sul, Brasília – DF – 70200-001",
"Centro de Ensino Fundamental 03 SQS 103 – AE – Asa Sul, Brasília – DF – 70342-000",
"Centro de Ensino Fundamental Athos Bulcão do Cruzeiro SHCES – Cruzeiro / Sudoeste / Octogonal, Brasília – DF – 70650-311",
"Centro de Ensino Fundamental 02 SHCS SQS 107 – Asa Sul, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 01 do Planalto Vila Planalto Acampamento Rabêlo – Brasília, DF – 70804-170",
"Centro de Ensino Fundamental 01 do Cruzeiro Área Especial F s/n Lote G – Cruzeiro Velho, Brasília – DF – 70640-001",
"Centro de Ensino Fundamental 01 Lago Norte Área Especial Quadra 4/6 – Brasília, DF – 71510-200",
"Centro de Ensino Fundamental do Varjão Asa Sul Superquadra Sul 314 BL K – Asa Sul, Brasília – DF – 70383-000",
"Escola Classe 13 Eq 07 Ae 01 – Setor Residencial Norte, Setor Residencial Norte – Planaltina, Brasília – DF – 73340-700",
"Escola Classe 11 Eq 05 Ae 01 – Setor Residencial Norte, Setor Residencial Norte – Planaltina, DF – 73340-500",
"Escola Classe 10 Estância Nova Planaltina Area Rural – Planaltina, DF – 73330-037",
"Escola Classe 09 Sgopi Cj C – Via 06 – Setor Residencial Norte, Setor Residencial Norte – Planaltina, DF – 70340-000",
"Escola Classe 07 Vila NS de Fátima Ae 01 – Planaltina, Brasília – DF – 73340-791",
"Escola Classe 06 Eq 05/06 Proj G, Setor Residencial Leste – Buritís I Q 1 Cl Conjunto – Vila Buriti, DF – 73360-500",
"Escola Classe 05 Vila Vecentina Q 17 – Planaltina, Brasília – DF – 73320-020",
"Escola Classe 04 Entrequadra 03/04 – SRL, Brasília – DF – 73350-350",
"Escola Classe 03 St. Res. Leste | Buritís I Q 1 – Vila Nossa Sra. de Fátima, Brasília – DF – 73370-100",
"Escola Classe 01 do Arapoanga Q 18B – Arapoanga Condomínio Mansões, Brasília – DF, 73370-100",
"Escola Classe 01 Av Independ 102 – VL Vicentina, LOTE 01 – Vila Vicentina, Brasília – DF – 73320-010",
"CAIC Assis Chateubriand Q 18 Via NS 02, St. Res. Leste | Buritís IV AE 04 – Planaltina, Brasília – DF – 73350-120",
"CEI 01 Estância Nova Planaltina Area Rural – Planaltina, DF – 73330-037",
"Jardim De Infância Casa Da Vivência Ae 09, St. de Áreas Especiais Norte – Planaltina, DF – 73340-710",
"Centro de Ensino Fundamental 01 SQS 106 – AE 70345000",
"Centro Educacional 01 SGAS I St. de Grandes Áreas Sul 907 sala 55, SGAS I – Brasília, DF – 70390-070",
"Escola do Parque da Cidade – Proem SGAS I SGAS 909 A 27 – Asa Sul, Brasília – DF – 70390-000",
"Escola da Natureza PARQUE DA CIDADE SARAH KUBITSCHEK – PORT 05 – Brasília, DF – 70297-400",
"Escola Meninos E Meninas Do Parque Srps – Asa Sul, Brasília – DF – 70390-090",
"CIEF SGAS I SGAS 907 – Asa Sul, Brasília – DF – 70390-070",
"CESAS – EJA Asa Sul SGAS II SGAS 602 – Asa Sul, Brasília – DF – 70200-720",
"Biblioteca Escolar e Comunitária da EQS 108/308 EQS 108/308 Professora Tatiana Eliza Nogueira, SHCS SQS 308 – Brasília, DF – 70347-000",
"Biblioteca Infantil 104/304 Asa Sul Superquadra Sul304 – Brasília, DF – 70297-400",
"CEJAEP EaD – EJA e Ed. Profissional St. de Grandes Áreas Sul 602, SGAS II, Asa Sul, Brasília – DF – 70297-400",
"Escola de Música SGA/Sul Quadra 602 Projeção D Parte A – Asa Sul, Brasília – DF – 70200-030",
"Centro Interescolar de Línguas CIL 02 SHCGN 711 Norte – Asa Norte, Brasília – DF – 70750-760",
"Centro Interescolar de Línguas CIL 01 SGAS I 907/908 Módulos 25/26 – Asa Sul, DF – 70390-075",
"Escola Parque 303/304 Norte Asa Norte SQN 303/304 – Asa Norte, Brasília – DF – 70297-400",
"Escola Parque 210/211 Norte Asa Norte SQN 210/211 – Asa Norte, Brasília – DF – 70297-400",
"Escola Parque 313/314 Sul Asa Sul EQS 313/314 – Asa Sul, Brasília – DF – 70390-110",
"Escola Parque 307/308 Sul SHCS EQS 307/308 – Asa Sul, Brasília – DF – 70200-050",
"Escola Parque 210/211 Sul SHCS EQS 210/211 – Asa Sul, Brasília – DF – 70382-400",
"CEPI Gavião – Lago Norte CEPI GAVIÃO, 2 Setor de Habitações Individuais Norte Lotes A e B – Lago Norte, Brasília – DF – 71530-250",
"Centro de Ensino Infantil 01 UnB Quadra 611 – Asa Norte, Brasília – DF – 70297-400",
"Jardim de Infância VI Comar SHIS QI 03 AE AERONAUTICA, Aeb 20 – Lago Sul, DF – 71603-000",
"Jardim de Infância 21 de Abril SHCS 708/ 908, SHCS, Brasília – DF – 70390-088",
"Jardim de Infância 01 do Cruzeiro SRES – Cruzeiro Velho, Brasília – DF – 70297-400",
"Jardim de Infância 404 Norte SQN 404 – AE – Asa Norte, Brasília – DF – 70845-000",
"Jardim de Infância 312 Norte SQN 312 CJ 05 01 – Asa Norte, Brasília – DF – 70866-000",
"Jardim de Infância 304 Norte SHCN SQN 304 – Asa Norte, Brasília – DF – 70297-400",
"Jardim de Infância 302 Norte SQN 302 – Ae, SHCN, Brasília – DF – 70723-000",
"Jardim de Infância 106 Norte SQN106 – Ae, SHCN, Brasília – DF – 70742-000",
"Escola Classe SMU QRO – AE – SMU, Brasília – DF – 70630-000",
"Escola Classe Vila Do RCG Regimento de Cavalaria e Guarda, Setores Militar Complementar – 0, Brasília – DF, 70631-000",
"Escola Classe Jardim Botânico SMDB – Lago Sul, Brasília – DF – 70297-400",
"Escola Classe Granja do Torto SHCN Superquadra Norte 113 – Asa Norte, Brasília – DF – 70763-000",
"Escola Classe Aspalha – Lago Norte Núcleo Rural Vale do Palha Cj 1, SHIN, Brasília – DF – 71540-045",
"Escola Classe 708 Norte Condomínio do Bloco D Entrada 50 Qd 707/708 – Asa Norte, Brasília – DF – 70297-400",
"Escola Classe 416 Sul SQS 416 – AE , SHCS, Brasília – DF – 70091-900",
"Escola Classe 415 Norte SHCN SQN 415 – Asa Norte, Brasília – DF – 70297-400",
"Escola Classe 413 Sul SHCS SQS 413 – Asa Sul, Brasília – DF – 70200-001",
"Escola Classe411 Norte SHCN SQN 411 – Asa Norte, Brasília – DF – 70866-530",
"Escola Classe 410 Sul Asa Sul Superquadra Sul 410 – Asa Sul, Brasília – DF – 70276-160",
"Escola Classe 405 Norte SHCN SQN 405 – Asa Norte, Brasília – DF – 70846-000",
"Escola Classe 403 Norte SHCN SQN 403 – Asa Norte, Brasília – DF – 70297-400",
"Escola Classe 316 Sul SQS 316 – s/n EC 316, Brasília – DF – 70297-400",
"Escola Classe 314 Sul SHCS SQS 314 – Asa Sul, Brasília – DF – 70380-000",
"Escola Classe 312 Norte SHCN SQN 312 – Asa Norte, Brasília – DF – 70866-000",
"Escola Classe 308 Sul SQS 308, Área especial – Brasília, DF – 70354-400",
"Escola Classe 305 Sul SHCS SQS 305 – Asa Sul, Brasília – DF – 70297-400",
"Escola Classe 304 Sul SHCS SQS 304 – Asa Sul, Brasília – DF – 70337-000",
"Escola Classe 304 Norte SHCN SQN 304 – Asa Norte, Brasília – DF – 70736-000",
"Escola Classe 302 Norte SHCN SQN 302, Brasília – DF – 70723-000",
"Escola Classe 209 Sul Área Especial, SHCS 209 Sul, DF – 70272-000",
"Escola Classe 206 Sul SHCS SQS 206 – Asa Sul, Brasília – DF – 70252-040",
"Escola Classe 204 Sul Área Especial, SHCS SQS 204 – Asa Sul, Brasília – DF – 70234-000",
"Escola Classe 115 Norte SHCN SQN 115 – Asa Norte, Brasília – DF – 70297-400",
"Escola Classe 114 Sul Sqs 114 – Ae – Sn Sqs 114 – Ae – Sn, Brasília – DF – 70377-000",
"Escola Classe 113 Norte SHCN Superquadra Norte 113 – Asa Norte, Brasília – DF – 70763-000",
"Escola Classe 111 Sul SHCS SQS 111 – Asa Sul, Brasília – DF – 70374-000",
"Escola Classe 108 Sul SHCS SQS 108 – Asa Sul, Brasília/DF – 70347-000",
"Escola Classe 106 Norte SHCN SQN 106 – Asa Norte, Brasília – DF – 70297-400",
"Escola Classe 102 Sul Área Especial, SHCS SQS 102 – Asa Sul, Brasília – DF – 70330-000",
"Escola Classe 06 do Cruzeiro Q. 805 – Cruzeiro Novo, Brasília – DF – 70297-400",
"Escola Classe 05 do Cruzeiro SHCES Qd 203 – Cruzeiro Novo, Brasília – DF – 70297-400",
"Escola Classe 04 do Cruzeiro SHCES 405/407 – Lt 1 – AE , Brasília – DF – 70650-479",
"Escola Classe 01 SHI Sul – Lago Sul SHIS QI 05, atrás do Gilberto Salomão – Lago Sul, Brasília – DF – 71635-600",
"Centro de Educação Infantil Buritizinho DF 280 KM 7/8 Sitio Nova Esperança, Brasília – DF – 72667-400",
"Escola Classe 415 Qn 417 Ae Setor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72323-540",
"Escola Classe 410 Qn 410 Ae Setor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72320-500",
"Escola Classe 403 QS 403 – AE – Samambaia Sul, Brasília – DF – 72319-570",
"Escola Classe 325 Qn 325 Ae Setor Sul, St. Sul – Samambaia Sul, Brasília – DF – 72309-700",
"Escola Classe 318 Qs 318 Ae Setor Sul, St. Sul – Samambaia Sul, Brasília – DF – 72308-530",
"Escola Classe 317 QR 317 – s/n EC – Samambaia Sul, Brasília – DF – 72307-800",
"Escola Classe 303 Qn 303 Ae Setor Sul, St. Sul – Samambaia Sul, DF – 72305-000",
"Escola Classe 121 St. Sul QS 121 – Samambaia Sul, Brasília – DF – 72301-580",
"Escola Classe 111 Qs 111 – St. Sul, Brasília – DF – 72301-550",
"Escola Classe SRIA Rua da Ceb Eptg- após SEDF. SIA Área Especial I Lote 01, DF – 71215-000",
"Escola Classe 02 da Estrutural Rod DF 087 St Central – Cidade Estrutural, DF – 71255-060",
"Escola Classe 01 da Estrutural Praça Central Da Vila, EPCL – Guará, Brasília – DF – 70297-400",
"Escola Classe 08 EQ 28/30 LOTE A, A/E, Guará II, Brasília – DF – 71065-285",
"Escola Classe 07 QE 38 ÁREA ESPECIAL Lote 12 Projeção D, Guará II – Guará, Brasília – DF – 71070-110",
"Escola Classe 06 EQ 24/26 LOE A A/E, Guará II, Brasília – DF – 71060-245",
"Escola Classe 05 Guará I QE 20 – Guará, Brasília – DF – 71015-105",
"Escola Classe 03 EQ 07 A/E Q LOTE J, Guará I, Brasília – DF – 71020-000",
"Escola Classe 02 QE 02 – AE BLOCO “A” – QE 02 – AE, Guará I, Brasília – DF – 71010-003",
"Escola Classe 01 Guará I QE 3 – Guará, Brasília – DF – 70120-000",
"CEI da Estrutural Qd 03 Ae 01 – St Norte, St. Norte – Guará, Brasília – DF – 70297-400",
"Jardim de Infância Lúcio Costa EPTG Lúcio Costa 05 – Guará, Brasília – DF – 71030-000",
"Centro Interescolar de Línguas 01 do Riacho Fundo I SHRF EQ 2/4 LT A AE – CEP: 71820421",
"Centro Interescolar de Línguas 01 3ª Av. AE 04 Praça Oficial 4/2 – Núcleo Bandeirante, Brasília – DF – CEP: 71720-592",
"CIL do Riacho Fundo II QR 2 – Candangolândia, Brasília – DF – 71725-200",
"CEM 01 do Riacho Fundo I Riacho Fundo QS 14 Lt A – Riacho Fundo I, Brasília – DF – 71825-400",
"CEM Julia Kubitschek SHCS QR 0A – Candangolândia, Brasília – DF – 71727-200",
"CEM Urso Branco CEMUB, 3ª Avenida, AE 04, Praça Oficial – Núcleo Bandeirante/DF – 71720-592",
"CED 01 do Riacho Fundo II QS18, e 2, Condomínio da Qe 12, Área Especial I – Riacho Fundo II, Brasília – DF – 71884-682",
"CED Agrourbano Ipe do Riacho Fundo II CAUB I – Riacho Fundo II, Brasília – DF – 70297-400",
"CED 02 do Riacho Fundo I Riacho Fundo I QN 7 AE 1 – Riacho Fundo I, Brasília – DF – 71800-000",
"CED Vargem Bonita Rua 1 Núcleo Hortícula, Setor de Mansões Park Way – Núcleo Bandeirante, Brasília/DF – 71750-000",
"CEF 02 do Riacho Fundo II QS 8 – Núcleo Bandeirante, Brasília – DF – 71884-330",
"CEF Telebrasilia do Riacho Fundo I CETELB, Lotes 01 e 02 – Praça Central, Riacho Fundo, Brasília – DF – 71805-701",
"CEF 01 da Candangolândia QR 2 – Candangolândia, Brasília – DF – 71725-200",
"CEF Metropolitana RUA 01 – LT 06 – PRACA DA METROPOLITANA –Núcleo Bandeirante, Brasília – DF – 71730-055",
"Centro de Ensino Fundamental 01 Sapão Sapão, Av Contorno Ae 8 Lt A – Núcleo Bandeirante, DF – 71705-000",
"EC Riacho Fundo I Df 75 – Epnb, Riacho Fundo Granja Modelo – Riacho Fundo I, DF – 71707-991",
"EC Agrovila EPTG Comb Agrourbano – Riacho Fundo II, Brasília – DF – 72307-990",
"EC 02 do Riacho Fundo II Riacho Fundo II – 1A Etapa QN 14D – Riacho Fundo II, Brasília – DF – 71881-140",
"EC 01 do Riacho Fundo II Riacho Fundo II-2A Etapa QC 4 Conj. 18 Lote 01/02 – Riacho Fundo II, Brasília – DF – 71882-168",
"EC Kanegae Epnb, Col. Agrícola Riacho Fundo – Riacho Fundo I, DF – 71707-991",
"EC 02 do Riacho Fundo I QN 05 AE 07, Riacho Fundo I, Brasília/DF – 71805-400",
"EC 01 do Riacho Fundo I QD 14 AE, Riacho Fundo I, Brasília – DF – 71820-421",
"EC 02 da Candangolândia EQR 02/03 AE – Candangolandia, Brasília – DF – 71725-250",
"EC 01 da Candangolândia EQR 05/07 AE – Candangolândia, Brasília – DF – 71725-500",
"EC Ipê Smpw Trecho 2 Q 8 Conjunto 2, 2 – Área Especial Granja do Ipê, Brasília – DF – 71701-970",
"EC 05 2 AV – EQ – BL 1400/1500 – Núcleo Bandeirante, Brasília – DF – 71715-062",
"EC 04 2ª Av – EQ – Blocos 440/540 – Núcleo Bandeirante, Brasília – DF – 71715-058",
"EC 03 3A Av Ae 06 Lt H/N – Núcleo Bandeirante, DF – 71720-588",
"CAIC Juscelino Kubitschek Park Way Trecho 2 Q 6 – Núcleo Bandeirante, Brasília – DF – 70297-400",
"CEI do Riacho Fundo II QN 14A, Área Especial 1-2 – Riacho Fundo II – 1A Etapa, Núcleo Bandeirante, Brasília – DF – 71881-110",
"CEI 01 do Riacho Fundo I Area Especial QN 7 – Riacho Fundo I, Brasília – DF – 71805-731",
"CEI da Candangolândia QR 1A – Candangolândia, Brasília – DF – 70297-400",
"CEI Segunda Avenida Entre Blocos 960/1060 – Núcleo Bandeirante, Brasília/DF – 71715-060",
"JI 01 do Riacho Fundo II QN 08 C – AE 01- RIACHO FUNDO II – Brasília – DF – 71880-130",
"Escola Classe Sussuarana DF-270 – KM 4 – CH São Francisco, Brasília, DF –71570-000",
"Escola Classe Café sem Troco DF-130 – KM 32, Brasília – DF – 71570-000",
"CEF 05 QD 25 CJ A LTS 3, 4, 18 E 19, Brasília – DF, 71572-501",
"CIL 01 Q 17 – Paranoá, Brasília – DF – 70297-400",
"CEM 01 Q 04 Cj A Ae 02 – Paranoá, Brasília – DF – 71570-401",
"CED 01 do Itapoã Via DF-250, s/n – Paranoá, Brasília – DF –",
"CED PAD/DF BR-251 – KM-07 – ESTRADA DE UNAI – DF – 70359-970",
"CED Darcy Ribeiro Q 31 Cj F Ae – Paranoá, Brasília – DF, 71573-107",
"CEF Dra Zilda Arns Qd 378 – Del Lago, Paranoá – DF, 71590-000",
"CEF Jardim II Br-251 – Df-285 Nucleo Rural Jardim Paranoá, DF, 71570-000",
"CEF Buriti Vermelho Nr Buriti Vermelho – Df-270, DF-100, Brasília – DF, 71570-000",
"CEF 04 Q 4 – Paranoá, Brasília – DF, 71570-401",
"CEF 03 Q 26 Cj G Lt 01 – Paranoá, Brasília – DF – 71572-600",
"CEF 02 Q 04 Cj A Ae 01 – Paranoá, Brasília – DF – 71570-401",
"CEF 01 Q 03 Ae 01 – Paranoá, Brasília – DF, 71570-300",
"Escola Classe 02 do Itapoã Qd 378 – Conjunto L – Ae 03 – Itapoa, DF, 71590-000",
"Escola Classe 01 do Itapoã Qd 61 – Conjunto e – Ae – Del Lago – Itapoã, DF – 71590-000",
"Escola Classe Sobradinho Dos Melos Df 250 – Km 07 Faz. Paranoa Rural Paranoá, DF –71586-100",
"Escola Classe Quebrada dos Neres Br 251 – Km 23 Rural Paranoá, DF, 70000-000",
"Escola Classe Natureza Df 250 – Km 8 Chácara Nutri – Rural, DF – 73007-994",
"Escola Classe Lamarão Df 285 – Df 120 Colônia Agrícola Lamarão Rural – Paranoá, DF – 71570-000",
"Escola Classe Itapeti BR-251 – DF-100 – KM-37 – GJ PROGRESSO – Paranoá, DF – 71570000",
"Escola Classe Cora Coralina Df 250 Km 20 – Faz. Paranoá – Sobradinho, DF – 73000-000",
"CAP Comunidade de Aprendizagem do Paranoá Paranoá, QD 3 CJ A LTS 8 A 10– Brasília – DF – 71570-305",
"Escola Classe Cariru Paranoá, Núcleo Rural Cariru – Cariru, Brasília – DF – 71570-000",
"Escola Classe Capão Seco BR-251 – DF-270 – KM 38 – BSB/UNAI – Paranoá, DF – 71570-000",
"Escola Classe Boqueirão Fazenda Sao Bento – Nr Boqueirao, DF – 71573-992",
"Escola Classe Alto Interlagos Chácara Alto Interlagos 10A, Altiplano Leste, DF, 71680-354",
"Escola Classe 06 QD 33, Área Especial, Paranoá, DF – 71573-303",
"Escola Classe 05 Q 24 – Paranoá, Brasília – DF – 71572-400",
"Escola Classe 04 Qd 14 – Conjunto F – Ae 01 – Paranoá, DF – 71571-408",
"Escola Classe 03 Q 17 Lt 08 – Paranoá, Brasília – DF – 71571-703",
"Escola Classe 02 Q 30 Cj E Lt 17 – Paranoá, Brasília – DF – 71573-025",
"Escola Classe 01 Q 26 Cj G Ae 01 – Paranoá, Brasília – DF – 71572-600",
"CAIC Santa Paulina Q. 05 Ae 01 – Paranoá, Brasília – DF – 71570-500",
"Centro de Educação Infantil 01 AE Quadra 16 Conj. E Lote 01 – Paranoá, Brasília – DF – 70297-400",
"Centro de Ensino FundamentalBonsucesso DF130 – Km 04 – NR Bonsucesso, Núcleo Rural Rajadinha – Bonsucesso, DF – CEP: 73307-990",
"Centro de Ensino Fundamental São José BR 479 – DF 250 NRr São José Rural – Planaltina, DF – CEP: 73300-000",
"Centro de Ensino Fundamental Rio Preto DF 250 – DF 320 NR Rio Preto Rural – Planaltina, DF – CEP: 73300-000",
"Centro de Ensino Fundamental 2 Av São Paulo Q 52 – Lt 02/06 – Setor tradicional – Planaltina, DF – CEP: 73330-010",
"Centro Educacional Várzeas DF-120 – DF-455 –Núcleo Rural Tabatinga – Planaltina, Brasília – DF – CEP: 73390100",
"Centro Educacional Pipiripau II BR 020 DF 345/205, S/N – Planaltina, Brasília – DF, 73301-970",
"Centro Educacional Osorio Bacchin DF 205 – Q G Lote 22 – Nr Jardim Morumbi Rural – Planaltina, DF, 73300-000",
"Escola Classe Vale Verde Quintas do Vale Verde – KM 6,5 – Chácara 133, Brasília/DF – 73333-333",
"Escola Classe Reino das Flores Chácara Sinhá Cristina – Fazenda Mestre D’Armas, Brasília/DF – 73300-000",
"Escola Classe Rajadinha DF-250 – DF-06, Brasília/DF – 73300-000",
"Escola Classe Pedra Fundamental BR-020 – DF-230 – Chácara Largo da Pedra nº 15, Brasília/DF – 73301-970",
"Centro de Educação Infantil Palmeiras BR-020 – DF-205 – KM 15, Brasília/DF – 73330-100",
"Escola Classe Monjolo BR-020 – DF-335 – Fazenda Monjolo, Brasília/DF – 73300-000",
"Escola Classe Frigorífico Industrial BR-020 – KM-10 – DF-230 – FRI BOI, Brasília/DF – 73301-070",
"Escola Classe ETA 44 BR-020 – KM 18 – CPAC/EMBRAPA, Brasília/DF – 73301-970",
"Escola Classe Estância do Pipiripau DF-345 – KM-28, Brasília/DF – 73300-000",
"Escola Classe Córrego do Meio BR-020 – KM-18 – Chácara C, Brasília/DF – 73300-000",
"Escola Classe Coperbrás DF-250/355 – Chácara 172, Brasília/DF – 73390-100",
"Escola Classe Barra Alta DF-260 – Chácara 210, Brasília/DF – 73390-100",
"Escola Classe Núcleo Rural Córrego do Atoleiro DF-345 – KM 18, Brasília – DF – 73377-003",
"CEP Saúde Entre Av Contorno e Independencia – SN – CEP: 73300000",
"Centro Educacional Taquara DF- 230 – KM 22 73300000",
"Centro Interescolar de Línguas CIL 01 Setor Educacional, Lote C, Praça do Estudante, Planaltina – DF, CEP: 73310-154.",
"Centro de Ensino Especial 01 Setor Educacional Lt 1 Setor de Educação – Planaltina, DF – 73310-150",
"Centro de Ensino Médio 02 Setor Educacional Lt J /L Setor de Educação – Planaltina, DF, 73310-150",
"CED Vale do Amanhecer AE 03 – LT 01 – VL PACHECO– Vale do Amanhecer, Brasília – DF, 73370-077",
"CED Stella dos Cherubins Guimarães Troi RUA HUGO LOBO – QD 97 – AE – Planaltina, Brasília – DF – 73330-028",
"CED Pompílio Marques Area Sub Mestre Darmas – Mod 01 – Lote 1, Mestre d’Armas – Planaltina, DF – 73380-000",
"CED Dona América Guimarães Sh Arapoanga/Condomínio Mansões Arapoanga Q 10K – Planaltina, DF – 73368-854",
"Centro Educacional Estância III Mod.1 Rua 1A nº 16, Estância Mestre D’Armas 4 – Planaltina, Brasília/DF – CEP: 73380-300",
"CEF 04 Setor de Educação – Planaltina, DF – 73310-150",
"CED 03 Setor Residencial Norte A 46 – Planaltina, Brasília – DF – 73340-025",
"CED 01 Setor Educacional Lt A, St. de Educaçâo – Planaltina, Brasília – DF – 73310-150",
"CEF Nossa Senhora de Fátima Ae 01, Vila NS de Fátima – Planaltina, DF – 73340-791",
"CEF Juscelino Kubitschek Br 020 – Mod 7 Lt 17,18,19,24,25 E 26, Condomínio Mte. D’armas – Planaltina, DF – 73300-000",
"CEF Arapoanga Rua 8I Ae Cond Arapoanga Arapoanga – Planaltina, Brasília – DF – 73370-100",
"CEF 08 EQ 3/4 Projeção H Setor Residencial Leste Planaltina-DF",
"CEF 03 Eq 10/20 Lt H, St. Res. Leste | Buritís I Vila Vicentina/ Vila Buritis, Brasília – DF – 73355-050",
"CEF 02 de Araporanga Q 3G, Mansões Arapoanga – Planaltina, Brasília – DF – 73370-100",
"CEF 01 Setor Educacional Lote M – Planaltina, Brasília – DF –73310-150",
"Escola Classe Vale do Sol SH Aprodarmas Condomínio Vale do Sol – Vila Nossa Sra. de Fátima, Brasília – DF – 73375-712",
"Escola Classe Santos Dumont Km 18, Vale do Amanhecer, Núcleo Rural Santos Dumont, Brasília – DF – 73300-000",
"Escola Classe Paraná St. Res. Leste | Buritís I Q 1 – Planaltina, Brasília – DF – 73350-150",
"Escola Classe Estância Df 345 – Km 28 Nr Pipiripau Rural – Planaltina, DF – 73300-000",
"Escola Classe Aprodarmas ",
"Escola Classe Alta- Mir Df 130 – Km 18, Bica do Der – Rural, DF – 73300-000",
"Escola Classe 16 Condomínio Estância Mte. D’armas IV Mod 6 – Planaltina, Brasília – DF – 73380-400",
"Escola Classe 14 Q 13 Cj A Lt 01, Setor Residencial Leste – Buritís I Q 1 Cl Conjunto – Planaltina, Brasília – DF – 73355-300",
"Escola Classe 15 Mod 1A Area Rural – Planaltina, DF – 73380-750",
"CEPI Azaléia 7RC5+86 Incra 08 – Brazlândia, Brasília – DF",
"Biblioteca Érico Veríssimo ",
"CEI 03 DCAG ROD DF 180 KM 6 INCRA 6 72772010",
"CED Incra 08 DCAG – BR-070 – RA IV – QD 04 – LT S/N – CEP: 72701970",
"CEPI Sagui Qd 2, Lt 2 – Brazlândia – DF –",
"CIL St. Tradicional Q 3 – Brazlândia, Brasília – DF –72720650",
"Centro de Ensino Especial B, 152,, St. Norte Eq 2/4, Brasília – DF",
"Centro de Ensino Médio 02 Q 36 AE 03, Vila São José – Brazlândia/DF – 72736-000",
"Centro de Ensino Médio 01 St. Sul AE 2 – Brazlândia, Brasília – DF – 72715620",
"Centro Educacional Irmã Regina Velanes Regis DF-430 – Brazlândia, Brasília/DF – 72701970",
"Centro Educacional 04 Df 180 – Km 27 Faz. Curralinho Rural – Brazlândia, DF – 72701-970",
"Centro Educacional 02 Ae S/N Praça Do Laço, St. Norte – Brazlândia, DF – 72705-700",
"Centro de Educacional Vendinha Df 180 Bsb, 15, BR-251 – Brazlândia, Padre Bernardo – DF – 72701-970",
"Centro de Ensino Fundamental 03 Vila São José Q 22 – Brazlândia – Brasília – DF – 72746-002",
"Centro de Ensino Fundamental 02 Q 12 Ae 5, St. Norte – Brazlândia – DF – 72710-650",
"Centro de Ensino Fundamental 01 Q 6 – Veredas, Brasília – DF – 72726-600",
"Escola Parque da Natureza PIQ 03 LT 02 ST VEREDAS – DF – 72725302",
"CAIC Professor Benedito Carlos de Oliveira AE 05, St. Tradicional – Brazlândia/DF – 72720-650",
"Centro de Educação Infantil 02 2 – Q 45 Invasão 3 Centro – Brazlândia – DF – 72755-000",
"Centro de Educação Infantil 01 Piq Q 05 Lt 01 Setor Veredas Veredas – Brazlândia/DF – 72726-100",
"Escola Classe Polo Agrícola da Torre DF-001 – EPCT 430/415 – KM 06– DF – 72700000",
"Centro de Ensino Fundamental Incra 07 Reserva G Gleba 3 – Incra 7 Rural – Brazlândia, DF – 72701-970",
"Escola Classe Incra 06 Rod Df 180 Km 06 Rural – Brazlândia – DF – 72701-970",
"Escola Classe Chapadinha Df 445 – Km 4 Rural – Brazlândia, DF, 72701-970",
"Escola Classe Bucanhão Df 415 – Km 3 Sentido Df 180 Rural – Brazlândia, DF, 72701-970",
"Escola Classe Almécegas VC-505 – Brazlândia, Brasília – DF – CEP – 72738990",
"Escola Classe 09 Vila São José Invasão 2 – Centro, DF, 72745-000",
"Escola Classe 08 Piq Q 04 Lt 01 Setor Veredas Veredas – Brazlândia – DF – 72725-400",
"Escola Classe 07 Q 38 Ae 02 – Vila São José – Brazlândia, DF – 72738-000",
"Escola Classe 06 Q 38 Ae 01- Vila São José – Brazlândia – DF – 72738-000",
"Escola Classe 05 AE 01- Setor Sul – Brazlândia – DF – 72715-610",
"Escola Classe 03 EQ 06/08 – LT A – Brazlândia DF- CEP 72710067",
"Escola Classe Incra 08 QD 18 – AE 01 – Brazlândia DF- CEP 72760180",
"Escola Classe 01 AE 03 – Setor Tradicional- Brazlândia DF- CEP 72720630",
"Escola Classe Juscelino Kubitschek Quadra 500, Área Especial 01, Trecho 01, Setor Habitacional Sol Nascente, Brasília/DF – CEP: 72243-502",
"Instituto Frederico Ozaman QNM 31 Módulo C Área Especial, Ceilândia, Brasília/DF – CEP: 72215-310",
"Instituto Paz e Vida QNP 22/26 Área Especial G, P Sul, Ceilândia, Brasília/DF – CEP: 72235-225",
"Centro Social Luterano Cantinho do Girassol QNM 30 Módulos B e C Área Especial, Ceilândia Norte, Brasília/DF – CEP: 72210-300",
"Centro Comunitário da Criança (Célula III) SHSN Chácara 84 Conjunto A1 Lote 21, Sol Nascente, Ceilândia, Brasília/DF – CEP: 72236-800",
"Centro Comunitário da Criança (Célula II) QNN 16 Lote A Área Especial, Guariroba, Ceilândia, Brasília/DF – CEP: 72220-161",
"Centro Comunitário da Criança (Célula I) QNN 31 Módulo K Área Especial, Ceilândia Norte, Brasília/DF – CEP: 72225-321",
"Centro Comunitário da Criança (Matriz) EQNP 09/13 Módulos B e D Área Especial, P Norte, Ceilândia, Brasília/DF – CEP: 72240-572",
"CEPI Jasmim QNO 12 Área Especial G, Ceilândia, Brasília/DF – CEP: 72255-207",
"CEPI Flor de Pequi QNP 15 Área Especial 01, Ceilândia, Brasília/DF – CEP: 72241-609",
"CEPI Sempre Viva QNQ 03 Lote B, Ceilândia, Brasília/DF – CEP: 72270-300",
"UNISS Granja das Oliveiras – Recanto das Emas, Brasília – DF – 72600-970",
"Unire – Unidade de Internação Granja das Oliveiras – Recanto das Emas, Brasília – DF – 70297-400",
"CIL Q 306 Área Especial – Recanto das Emas, Brasília – DF – 72621-300",
"Centro de Ensino Médio 804 s/n Área Especial 1 1 804 – Recanto das Emas, Brasília – DF – 72650-600",
"Centro de Ensino Médio 111 Q 111 AREA ESPECIAL – Recanto das Emas, Brasília – DF – 72602-314",
"Centro Educacional Myriam Ervilha Df 280 – Km 14 Rural – Samambaia Sul, DF – 72000-000",
"Centro Educacional 104 Q 104 Conj. 11A – Recanto das Emas, Brasília – DF – 72600-412",
"Centro Educacional 308 Quadra 308, Área Especial, Conjunto 12, Lote 01 – Recanto das Emas, Brasília – DF – 72622-112",
"CEPI Pinheirinho Roxo Q. 300 Lote 01 – Recanto das Emas, Brasília – DF – 72620-108",
"Jardim de Infância 603 Q 603 – Recanto das Emas, Brasília – DF – 72640-308",
"Centro de Educação Infantil 310 Q 310 qd 310 conjunto 07a – Recanto das Emas, Brasília – DF – 72622-309",
"Centro de Educação Infantil 304 304 14A – Recanto das Emas, Brasília – DF – 72621-115",
"Centro de Ensino Fundamental 802 Q 802 Núcleo Rural Monjolo Conj. 21 – Recanto das Emas, Brasília – DF – 72650-315",
"Centro de Ensino Fundamental 801 Q 801 AE Recanto das Emas – Recanto das Emas, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 405 Q 405, Recanto das Emas Lt 1 – Recanto das Emas, Brasília – DF – 72631-115",
"Centro de Ensino Fundamental 602 Qd 602 – Conjunto 01 – Lote 01, Av. Recanto das Emas – Recanto das Emas, DF – 72640-201",
"Centro de Ensino Fundamental 306 Q 306 – Recanto das Emas, Brasília – DF – 72621-308",
"Centro de Ensino Fundamental 301 Área Especial, Q 301 – Recanto das Emas, Brasília – DF – 72620-200",
"Centro de Ensino Fundamental 206 Q 206 – Recanto das Emas, Brasília – DF – 72610-518",
"Centro de Ensino Fundamental 115 Q 115 Núcleo Rural Monjolo Conj. 7C – Recanto das Emas, Brasília – DF – 72603-314",
"Centro de Ensino Fundamental 113 QUADRA 113 ÁREA ESPECIAL – Recanto das Emas, Brasília – DF – 72603-109",
"Centro de Ensino Fundamental 106 QUADRA 106 ÁREA ESPECIAL – Recanto das Emas, Brasília – DF – 72601-201",
"Centro de Ensino Fundamental 101 Qd 101 Cj 10B Lts 01 e 02 – Recanto das Emas, Brasília – DF – 72600-133",
"Escola Classe Vila Buritis DF-280 – KM 09 – SETOR HABITACIONAL AGUAS QUENTES , Brasília – DF – 72669-329",
"Escola Classe 803 Qd 803 – Lote 01 – Ae Recanto das Emas – Recanto das Emas, Brasília – DF – 72650-400",
"Escola Classe 510 Q. 511 – Recanto das Emas, Brasília – DF – 72660-200",
"Escola Classe 404 Qd 404 – Conjunto 09 – Lote 01, Av. Recanto das Emas – Recanto das Emas, Brasília – DF – 72630-400",
"Escola Classe 401 Lj 1, Av. Recanto das Emas, 8 – q 401/402 cj – Recanto das Emas, DF – 72600-000",
"Escola Classe 203 Q 203 – Recanto das Emas, Brasília – DF – 70297-400",
"Escola Classe 102 Q 102 – Recanto das Emas, Brasília – DF – 72605-020",
"Centro de Educação Infantil 307 Qr 307 Ae, St. Chácaras P Sul – Samambaia Sul, Brasília – DF – 72305-600",
"Centro de Educação Infantil 210 QN 210 – Samambaia Norte, Brasília – DF – 72316-528",
"CAIC Helena Reis Qr 409 Ae Setor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72321-100",
"CAIC Ayrton Senna Qr 117 Ae Setor Sul, St. Sul – Samambaia Sul, Brasília – DF – 72301-700",
"Centro Interescolar de Línguas CIL 01 QN 407 Conjunto G – Samambaia Sul, Brasília – DF – 72321-013",
"Centro de Ensino Especial 01 Qs 303 Ae Setor Sul, St. Sul – Samambaia Sul, Brasília – DF – 72305-500",
"Centro de Ensino Médio 414 Qr 414 Ae Setor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72320-200",
"Centro de Ensino Médio 304 QR 304 – CONJ 04 – LT 01– Samambaia Sul, DF – 72306-500",
"Centro Educacional 619 Setor Norte Ae 1 – Samambaia Sul, Brasília – DF – 72333-520",
"Centro Educacional 123 Qr 123 Ae Setor Sul, St. Sul – Samambaia Sul, DF – 72303-000",
"Centro de Ensino Fundamental 519 Qr 519 Ae Setor Sul – Samambaia Sul, Brasília – DF – 72315-300",
"Centro de Ensino Fundamental 507 Qn 507 Ae Setorsul, St. Sul – Samambaia Sul, DF – 72313-000",
"Centro de Ensino Fundamental 504 QR 504 – Samambaia Sul, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 427 Qn 427 Ae 02 Setor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72327-540",
"Centro de Ensino Fundamental 412 QR 412 – Samambaia Sul, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental 411 Qn 411 Aesetor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72321-540",
"Centro de Ensino Fundamental 407 Qr 407/409 Qn 407 Conjunto e F G, 8 – Samambaia Sul, Brasília – DF – 72321-013",
"Centro de Ensino Fundamental 404 Qs 404 Ae Setor Norte – Samambaia Sul, Brasília – DF – 72318-550",
"Centro de Ensino Fundamental 312 Qs 312 s/n – Samambaia Sul, Brasília – DF – 72308-500",
"Centro de Ensino Fundamental 120 Qs 120 Ae Setor Sul, Setor Sul – Samambaia Sul, Brasília – DF – 72304-500",
"Escola Classe 831 QR 831 s/n Lt 01 – Samambaia Norte, Brasília – DF – 72338-711",
"Escola Classe 614 Qr 614 Ae 01 Setor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72322-700",
"Escola Classe 604 Qs 604 Ae 01 Setor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72322-520",
"Escola Classe 512 Qr 512 Ae Setor Sul – Samambaia Sul, Brasília – DF – 72312-800",
"Escola Classe 511 Qn 511 – St. Sul, Brasília – DF – 72313-600",
"Escola Classe 510 Qn 510 Ae Setor Sul, St. Sul – Samambaia Sul, Brasília – DF – 72312-400",
"Escola Classe 502 QS 502 – Samambaia Sul, Brasília – DF – 72310-402",
"Escola Classe 501 QN 501 CONJUNTO 03 LOTE 01 – ÁREA ESPECIAL – Samambaia Sul, Brasília – DF – 72311-203",
"Escola Classe 431 Qs 431 Ae 01 Setor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72329-550",
"Escola Classe 425 Qs 425 Ae 02 Setor Norte, Setor Norte – Samambaia Sul, DF – 72327-520",
"Escola Classe 419 Qs 419 Ae Setor Norte, Setor Norte – Samambaia Sul, Brasília – DF – 72325-520",
"Escola Classe 108 Qs 110 – St. Sul, Brasília – DF – 72302-530",
"Centro Interescolar de Línguas CIL CL 114 – Santa Maria, Brasília – DF – 72510-230",
"Centro de Ensino Especial 01 Cl 208 Ae – Santa Maria, Brasília – DF – 72508-220",
"Centro de Ensino Médio 417 Cl 417 Lt A – Santa Maria, Brasília – DF – 72547-240",
"Centro de Ensino Médio 404 Cl 404 Lt A – Santa Maria, Brasília – DF – 72504-240",
"Centro Educacional 416 EQ 416/516 Lt A – Santa Maria, Brasília – DF – 72546-330",
"Centro Educacional 310 CL 310 – Conj. H – AE – Santa Maria/DF – 72510-230",
"CEF Santos Dumont QRC 17 – Santa Maria, Brasília – DF – 72594-223",
"CEF Sargento Lima Área Alfa Marinha, s/n – Santa Maria, Brasília – DF – 70297-400",
"CEF 418 EQ 418/518 Lt 1 – Santa Maria, Brasília – DF – 72547-330",
"CEF 403 QR 403 CL 403 Lt A – Santa Maria, Brasília – DF – 72503-700",
"CEF 316 CL 316 A – Santa Maria, Brasília – DF – 72546-606",
"CEF 213 Cl 213 Lt G – Santa Maria, Brasília – DF – 72543-220",
"CEF 209 Cl 209 Lt A – Santa Maria, Brasília/DF – 72509-220",
"CEF 201 Cl 201 Lt A1 – Santa Maria, Brasília – DF – 72501-220",
"CEF 103 Qr 103 Lt B – Santa Maria, Brasília/DF – 72503-400",
"Escola Classe 218 Eq 218/318 Lt J – Santa Maria, DF – 72547-300",
"Escola Classe 215 Cl 215 Lt A – Santa Maria, DF – 73503-255",
"Escola Classe 206 Cl 206 Lt C1 – Santa Maria, Brasília – DF – 72506-220",
"Escola Classe 203 QR 203 CL 203 – Santa Maria, Brasília – DF – 72503-201",
"Escola Classe 116 QR 116 Lt 01 – Santa Maria, Brasília/DF – 72546-413",
"Escola Classe 100 QR 100 Ae 01 – Santa Maria, Brasília – DF – 72500-000",
"Escola Classe 01 do Porto Rico Qd 17 – Lote 14c – 3 Etapa St Habitacional Porto Rico – Santa Maria/DF – 72504-003",
"CAIC Santa Maria Eq 215/315 Lt B – Santa Maria, Brasília – DF – 72545-300",
"CAIC Albert Sabin Eq 304/307 Cj D Lt 1 – Santa Maria, Brasília – DF – 72504-300",
"Jardim de Infância 116 QR 116 – Santa Maria Norte, Santa Maria – DF – 72546-408",
"Centro de Educação Infantil 416/516 EQ 416/516 Lt B – Santa Maria, Brasília – DF – 72546-330",
"Centro de Educação Infantil 210 EQ 210/310 – Santa Maria, Brasília/DF – 72509-402",
"Centro de Educação Infantil 203 Brasília, QR 203 CL 203 – Santa Maria, Brasília – DF – 72503-221",
"Instituto Dom Leolino Irmã Cecília Luvizotto Residencial Morro da Cruz nº 02 CEP: 71693-500",
"Creche Bem-me-quer Rua 48, Lote 420, Centro, São Sebastião – CEP: 71693-030",
"Centro de Educação Infantil 05 Avenida das Paineiras, Quadra 08, Lote C, Jardim Botânico III – CEP: 71681-445",
"Centro Interescolar de Línguas 01 Rua 01 Lt 101 – St. Tradicional, DF – CEP: 71691-101 (O CIL 01 de São Sebastião funcionará provisoriamenteno CEF Cerâmica São Paulo)",
"Centro de Educação Infantil 04 QD 203 CJ 3 LT 6/15 RESID OESTE, Brasília,DF – 71692-607",
"Escola Classe 05 de Sobradinho Q 09 Cj A Ae – Sobradinho, DF – 73035-091",
"Centro de Ensino Médio 01 Q. 203 – St. Res. Oeste (São Sebastião), Brasília – DF – 71692-106",
"CED São Francisco Quadra 17, Lote 100 – São Francisco, São Sebastião – DF – 71693-317",
"CED São Bartolomeu Qd2 – Conjunto 03 – Lote 04 São Bartolomeu – São Sebastião, DF – 71690-000",
"CED São José Quadra 14 – São Sebastião, Brasília – DF – 71693-043",
"CEF Nova Betânia Br-251 – Km-38, Núcleo Rural Nova Betânia, Sn – São Sebastião, DF – 71690-000",
"CEF Miguel Arcanjo Q 2 – São Sebastião, Brasília – DF – 71697-040",
"CEF Jataí Df140 – Km11 Faz Barreiros Rod Diogo Ar Barreiros – São Sebastião, DF – 71500-000",
"CEF do Bosque Área Institucional 02 Resid. Do Bosque Residencial do Bosque – São Sebastião, DF – 71691-101",
"CEF Cerâmica São Paulo Rua 01 Lt 101 – St. Tradicional, DF – 71691-101",
"CAIC Unesco Q 05 Cj A Ae – Centro, DF – 71691-047",
"Centro de Educação Infantil 03 Quadra 202, Conjunto 04, Lote 01 Residencial Oeste – São Sebastião, DF – 71692-510",
"Centro de Educação Infantil 01 Q 101 Cj 10 Resid. Oeste Setor Residencial Oeste – São Sebastião, DF – 71692-101",
"Escola Classe Vila Nova Rua 31 – 200, 200, Sao Sebastao Sao Jose – São Sebastião, DF – 71693-032",
"Escola Classe Vila do Boa Vila do Boa sem Numero São Sebastião – São Sebastião, Brasília – DF – 70297-400",
"Escola Classe São Bartolomeu Nr São Bartolomeu Km 02 São Bartolomeu – São Sebastião, DF – 71690-000",
"Escola Classe Dom Bosco Qd 05 – Conjunto A – Área Especial Centro – São Sebastião/DF – 71691-076",
"Escola Classe Cerâmica da Bênção Q 02 Ae Centro – São Sebastião, DF – 71690-000",
"Escola Classe Cachoeirinha Br 251 – Km 73, Av. Tororó – Santa Bárbara, São Sebastião – DF – 71691-970",
"Escola Classe Bela Vista Rua 01 nº 221 Bela Vista, Brasília/DF – 71694-102",
"Escola Classe Aguilhada Br 251 – Km 69 Rod. Brasilia/Unaí Rural – São Sebastião, DF – 71690-000",
"Escola Classe Agrovila Q100 Cj Q Ae 01 B.Vila Nova Bairro, Vila Nova, DF – 71693-107",
"Escola Classe 303 Qd 303 – Conjunto 01 – Lote 34 Setor Residencial Oeste – São Sebastião, DF – 71692-805",
"Escola Classe 104 Q. 104 Lt 01 – St. Res. Oeste (São Sebastião), Brasília – DF – 71692-300",
"Centro de Ensino Fundamental 09 ",
"Escola Classe 04 ",
"CAIC Julia Kubitschek de Oliveira Qr 13 Cj 03 Ae 01, Sobradinho II – Sobradinho, DF – 73062-303",
"Centro de Educação Infantil 04 Condomínio Jardim América – Sobradinho, Brasília – DF – 73062-109",
"Centro de Educação Infantil 03 CEI 03, QD 16 – AE D1 – Sobradinho, Brasília – DF – 73050-160",
"Centro de Educação Infantil 02 Área Especial, Q 3 – Sobradinho, Brasília – DF – 73030-030",
"Centro de Educação Infantil 01 Q 2 AE – Sobradinho, Brasília – DF – 73015-100",
"Centro de Ensino Especial 01 Q 6 AE – Sobradinho, Brasília – DF – 70297-400",
"Centro Educacional 03 Q 05, Q 14 Ae – Sobradinho, DF – 73030-050",
"Centro Educacional Professor Carlos Ramos Mota Epct Df 001 – Km 13 Chapada Da Contagem Nr Lago Oeste – Sobradinho, DF – 73007-990",
"Centro Educacional Fercal Df 205 Km 19 – Fercal – Sobradinho, DF – 73007-993",
"Centro Educacional 04 II ar 10 conjunto 9 lote 1 área especial – Sobradinho 2 setor oeste, Brasília – DF – 73062-100",
"Centro Educacional 03 ar 10 conjunto 9 lote 1 área especial – Sobradinho 2 setor oeste, Brasília – DF – 73062-100",
"Centro Ensino Médio 01 Quadra 04, Área Especial 04 – Sobradinho, Brasília – DF – 73025-040",
"Centro de Ensino Médio 02 Q 12 – Sobradinho, Brasília – DF – 70297-400",
"Centro de Ensino Fundamental Queima do Lençol SITIO PATRICIA – DF – 325 – KM 8/9 – Sobradinho, Brasília – DF – 73062301",
"Centro de Ensino Fundamental 08 Qr 03 Lt 04, Sobradinho II – Sobradinho, DF – 73001-970",
"Centro de Ensino Fundamental 07 AR 13 – Sobradinho, Brasília – DF – 73062-300",
"Centro de Ensino Fundamental 05 Q 10 Rua 04, Ae 4, 5 – Sobradinho, DF – 73005-600",
"Centro de Ensino Fundamental 04 Q 15 Ae 02 – Sobradinho, DF – 73045-650",
"Centro de Ensino Fundamental 03 Q 06 Ae 02 – Sobradinho, DF – 73025-660",
"Centro de Ensino Fundamental 01 Q 2 – Sobradinho, Brasília – DF – 70297-400",
"Escola Classe Lobeiral  SITIO PATRICIA – DF-326 – KM-02 – LOBEIRA–Sobradinho, Brasília – DF – 73017017",
"Escola Classe Basevi Df 001 Km 6 Vila Basevi – Nr Lago Oeste – Sobradinho, DF – 73007-991",
"Escola Classe Sonhém de Cima  DF-205 LESTE – VC 201 – KM 4 – ASSENT CONTAGEM– Sobradinho, Brasília – DF – 73001-970",
"Escola Classe Sítio das Araucárias DF 440 – NR 01 – CH 13 – KM 5,5 – GJ SANTA HELENA , Brasília – DF – 73000-000",
"Escola Classe Santa Helena DF 440 – NR 01 – CH 13 – KM 5,5 – GJ SANTA HELENA , Brasília – DF – 73000-000",
"Escola Classe Rua do Mato Df 150/Df 007 Km 12 – Sobradinho, DF – 73000-000",
"Escola Classe Ribeirão Df 250 Oeste – Km 11 Fazenda Ribeirão – Sobradinho, DF, 73000-000",
"Escola Classe Olhos D’água Br 20 – Chácara Olhos D’Água 22 Urbana – Lago Norte, DF – 73000-000",
"Escola Classe Morro do Sansão Faz – Sobradinho, DF – 73000-000Como chegar:",
"Escola Classe Engenho Velho Fercal – Sobradinho, Brasília – DF – 73150-100",
"Escola Classe Córrego do Ouro Df 330 – Faz. Córrego Do Ouro – Sobradinho, DF – 73000-000",
"Escola Classe Catingueiro Df 205 Oeste À Direita – Sobradinho, DF – 73000-000",
"Escola Classe Brochado da Rocha Br 020 – Km-13 14 Nr Corrego do Arrozal – Sobradinho, DF – 73300-000",
"Escola Classe Boa Vista Sobradinho 1ª Avenida Norte Qn 205 Conjunto 1, 6 – Sobradinho, Brasília – DF – 73007-993",
"Escola Classe Basevi Df 001 Km 6 Vila Basevi – Nr Lago Oeste – Sobradinho, DF – 73007-991",
"Escola Classe 17 SH Mansões Sobradinho Q 1 Condomínio Vila Rabêlo 1 – Sobradinho, Brasília – DF –",
"Escola Classe 16 St. de Mansões Sobradinho, Condomínio Novo St – Sobradinho, Brasília – DF, 73270-560Como chegar:",
"Escola Classe 15 Q 03 área especial 01/02 – Sobradinho, Brasília – DF – 73030-630Como chegar:",
"Escola Classe 13 Ar 05 Ae 01, Sobradinho II – Sobradinho, Brasília – DF – 73060-500",
"Escola Classe 12 St. de Indústrias Q 01 Ae – Sobradinho, Brasília – DF – 73020-112",
"Escola Classe 11 Q 11 – Sobradinho, Brasília – DF – 70297-400",
"Escola Classe 10 Q 02 Cj B/C Lt E – Sobradinho, Brasília – DF – 73015-308",
"Escola Classe 01 Q 06 Rua 05 Ae1 Sobradinho, DF – 73025-660",
"Escola Classe 02 Vicente Pires St. Hab. Vicente Pires – Vila São Sõ José, Brasília – DF – 72110-800",
"CEP – Escola Técnica Avenida Águas Claras QS 07, Lote 02/08 – Vila Areal, Brasília – DF – 70297-400",
"Centro Interescolar de Línguas QSB 2, Área Especial 3/4 – Taguatinga, Brasília – DF – 72015-520",
"Centro de Ensino Especial 01 Taguatinga Norte Ae 12 – Taguatinga, Brasília – DF – 72140-200",
"Centro de Ensino Médio Norte CEMTN, St. C Norte 02, 03 – Taguatinga, Brasília – DF – 72115-650",
"Centro de Ensino Médio 03 Qse 05 – Taguatinga, Brasília – DF – 72025-050",
"Centro Educacional 07 Eqnm 36/38 Ae, Taguatinga Norte – Taguatinga, DF – 72145-517",
"Centro Educacional 06 St. L Norte Qnl 01 – Taguatinga, Brasília – DF – 72150-508",
"Centro Educacional 05 Qnj 56 Ae 16, Taguatinga Norte – Taguatinga, DF – 72140-616",
"Centro Educacional 04 Qng 6/7, AE 20 – Taguatinga/DF – 72130-005",
"Centro Educacional 02 Centrão, Qsd 09/11 Ae, Taguatinga Sul – Taguatinga, Brasília – DF – 72015-240",
"Centro de Ensino Fundamental Vila Areal Qs 06 Bl B Cj 430 Ae – Águas Claras, DF – 72030-160",
"Centro de Ensino Fundamental 21 Área Especial 27, St. L Norte QNL 28 Via LN 30 – Taguatinga Norte, Brasília/DF – 72161-830",
"Centro de Ensino Fundamental 19 Eqnl 10/12 Ae – Taguatinga, Brasília – DF – 72155-520",
"Centro de Ensino Fundamental 17 Eqnm 38/40 Ae Lt A, Taguatinga Norte – Taguatinga, Brasília – DF – 72145-520",
"Centro de Ensino Fundamental 16 St. L Norte QNL 22 – Taguatinga, Brasília – DF – 72161-200",
"Centro de Ensino Fundamental 15 Qsa 3/5, Area especial 01 – Taguatinga, Brasília/DF – 72015-034",
"Centro de Ensino Fundamental 14 Qnb 15 Ae 02, Taguatinga Norte, Brasília/DF – 72115-150",
"Centro de Ensino Fundamental 12 Qng 39 Ae 03, Taguatinga Norte – Taguatinga, Brasília – DF – 72130-390",
"Centro de Ensino Fundamental 11 Cnd 01/05 Ae, Taguatinga Norte – Taguatinga, Brasília – DF – 72120-055",
"Centro de Ensino Fundamental 10 St. E Sul QSE 7 – Taguatinga, Brasília/DF – 72025-110",
"Centro de Ensino Fundamental 09 St. D Sul QSD – Taguatinga Sul, Brasília – DF – 72020-010",
"Centro de Ensino Fundamental 08 St. A Norte QNA 52 – Taguatinga, Brasília – DF – 72110-520",
"Centro de Ensino Fundamental 05 Qse 22 Ae 09/10, Taguatinga Sul – Taguatinga, Brasília – DF – 72025-220",
"Centro de Ensino Fundamental 04 Eqnl 05/07 Lt 01, Taguatinga Norte – Taguatinga, Brasília – DF – 72150-517",
"Centro de Ensino Fundamental 03 Taguatinga Sul QSD CL – Taguatinga, Brasília – DF – 72015-240",
"Escola Classe Colônia Agrícola Vicente Pires EC CAVP, St. Hab. Vicente Pires – Vicente Pires, Brasília – DF – 72110-800",
"Escola Classe Bilíngue Libras e Português Escrito St. H Norte – Área Especial, Brasília – DF – 72130-510",
"Escola Classe Arniqueira SHA conj. 4, Sria II Qe 38 CI AE nº 5 – Setor Habitacional – Arniqueira/DF – 71994-010",
"Escola Classe 54 Qsd 32 Ae 01/02, Taguatinga Sul – Taguatinga, Brasília – DF – 72020-320",
"Escola Classe 53 St. L Norte QNL 16 – Taguatinga, Brasília – DF – 72160-604",
"Escola Classe 52 Qnm 38 Ae 01, Taguatinga Norte – Taguatinga, DF – 72145-518",
"Escola Classe 50 Eqnl 02/04 Ae, Taguatinga Norte – Taguatinga, Brasília – DF – 72155-500",
"Escola Classe 46 Eqnl 23/21 Ae Lt 01 – Taguatinga, Brasília – DF – 72152-505",
"Escola Classe 45 Eqnm 40/42 Ae, Taguatinga Norte – Taguatinga, DF – 72146-507",
"Escola Classe 42 Eqnm 34/36 Ae 1, Taguatinga Norte – Taguatinga, Brasília – DF – 72145-507",
"Escola Classe 41 Eqnl 13/15, St. B Sul Qsb Ae Qsb 10 – Taguatinga, Brasília – DF – 72151-510",
"Escola Classe 39 Qnc 15 Ae15/17 – Taguatinga, Brasília – DF – 72115-650",
"Escola Classe 29 Qnj 18 Ae 10 – Taguatinga, Brasília – DF – 72140-180",
"Escola Classe 27 QNF 19 AE, Taguatinga Norte, Brasília/DF – 72125-690",
"Escola Classe 19 QNA 39 Lt 19, Sia AE – Taguatinga, Brasília/DF – 72110-390",
"Escola Classe 18 QND 12 AE Lt 41, Taguatinga Norte, Brasília/DF – 72120-120",
"Escola Classe 17 QSA 03/05 AE, Taguatinga Sul, Brasília/DF – 72015-034",
"Escola Classe 16 QNG 6/7 AE 15, Taguatinga Norte, Brasília/DF – 72130-005",
"Escola Classe 15 Qnd 43, Sgcv Ae, Lt 23 – Taguatinga, Brasília/DF – 72120-430",
"Escola Classe 13 Qsf 05 Ae 02, Taguatinga Sul – Taguatinga, Brasília – DF – 72025-550",
"Escola Classe 12 Qnh 06/07 Ae, Taguatinga Norte – Taguatinga, Brasília – DF – 72130-560",
"Escola Classe 11 QSE 14 AE, Taguatinga Sul, Brasília/DF – 72025-140",
"Escola Classe 10 St. D Sul QSD 18 AE 23 – Taguatinga, Brasília – DF – 72020-180",
"Escola Classe 08 Ae14, Taguatinga Norte QNG 12 – Taguatinga, Brasília – DF – 72130-120",
"Escola Classe 06 St. B Norte CNB 12 – Taguatinga, Brasília – DF – 72115-125",
"Escola Classe 01 Qsc 01 Ae 01, Taguatinga Sul – Taguatinga, Brasília – DF – 72016-010",
"CAIC Professor Walter Jose de Moura – Águas Claras Qs 07 Ae 02 Lt 04/10, Av. Águas Claras, DF – 72030-170",
"Centro de Educação Infantil Águas Claras QS 11 conjunto R AE 01, Conj. R/04 Qs 11 Conjunto R – Taguatinga, Brasília – DF – 71979-730",
"Centro de Educação Infantil 08 St. D Norte QND Condomínio – Taguatinga, Brasília – DF –72120-055",
"Centro de Educação Infantil 07 St. D Sul Qsd 32 – Taguatinga, Brasília – DF – 70297-400",
"Centro de Educação Infantil 06 Eqnl 17/19 Ae, Taguatinga Norte – Taguatinga, Brasília – DF – 72151-520",
"Centro de Educação Infantil 05 Eqnj 23/25 Ae 09 – Taguatinga, Brasília – DF – 72140-230",
"Centro de Educação Infantil 04 CNA 01/02 AE – Praça Do Di – Taguatinga, Brasília/DF – 72110-015",
"Centro de Educação Infantil 03 St. M-Norte QNM 36 – Taguatinga, Brasília – DF – 72145-632",
"Centro de Educação Infantil 02 St. D Norte QND 59 – Taguatinga, Brasília/DF – 72120-020",
"Centro de Educação Infantil 01 St. D Sul – Taguatinga, Brasília – DF – 72025-140",
            };

            var coordinates = new List<CoordinateSearch>
            {
                    new CoordinateSearch { Long = -48.149128, Lat = -15.799026, Radius = 1250 },
                    new CoordinateSearch { Long = -47.98193, Lat = -15.87037, Radius = 1250 },
                    new CoordinateSearch { Long = -47.996521, Lat = -15.8707, Radius = 1250 },
                    new CoordinateSearch { Long = -47.991028, Lat = -15.860792, Radius = 1250 },
                    new CoordinateSearch { Long = -48.000641, Lat = -15.850224, Radius = 1250 },
                    new CoordinateSearch { Long = -48.002186, Lat = -15.863434, Radius = 1250 },
                    new CoordinateSearch { Long = -48.010769, Lat = -15.852041, Radius = 1250 },
                    new CoordinateSearch { Long = -48.02021, Lat = -15.859141, Radius = 1250 },
                    new CoordinateSearch { Long = -48.008022, Lat = -15.873011, Radius = 1250 },
                    new CoordinateSearch { Long = -48.019695, Lat = -15.870535, Radius = 1250 },
                    new CoordinateSearch { Long = -48.039951, Lat = -15.853692, Radius = 1250 },
                    new CoordinateSearch { Long = -48.029823, Lat = -15.867563, Radius = 1250 },
                    new CoordinateSearch { Long = -48.056259, Lat = -15.877635, Radius = 1250 },
                    new CoordinateSearch { Long = -48.044243, Lat = -15.856829, Radius = 1250 },
                    new CoordinateSearch { Long = -48.046818, Lat = -15.869709, Radius = 1250 },
                    new CoordinateSearch { Long = -48.057976, Lat = -15.8636, Radius = 1250 },
                    new CoordinateSearch { Long = -48.072224, Lat = -15.867397, Radius = 1250 },
                    new CoordinateSearch { Long = -48.091793, Lat = -15.874332, Radius = 1250 },
                    new CoordinateSearch { Long = -48.065186, Lat = -15.874497, Radius = 1250 },
                    new CoordinateSearch { Long = -48.078918, Lat = -15.877965, Radius = 1250 },
                    new CoordinateSearch { Long = -48.091621, Lat = -15.882588, Radius = 1250 },
                    new CoordinateSearch { Long = -48.103123, Lat = -15.887046, Radius = 1250 },
                    new CoordinateSearch { Long = -48.115826, Lat = -15.891669, Radius = 1250 },
                    new CoordinateSearch { Long = -48.142948, Lat = -15.893485, Radius = 1250 },
                    new CoordinateSearch { Long = -48.124065, Lat = -15.898273, Radius = 1250 },
                    new CoordinateSearch { Long = -48.129215, Lat = -15.883909, Radius = 1250 },
                    new CoordinateSearch { Long = -48.115139, Lat = -15.876644, Radius = 1250 },
                    new CoordinateSearch { Long = -48.101749, Lat = -15.870865, Radius = 1250 },
                    new CoordinateSearch { Long = -48.088703, Lat = -15.862279, Radius = 1250 },
                    new CoordinateSearch { Long = -48.076, Lat = -15.856995, Radius = 1250 },
                    new CoordinateSearch { Long = -48.062439, Lat = -15.852701, Radius = 1250 },
                    new CoordinateSearch { Long = -48.050079, Lat = -15.847417, Radius = 1250 },
                    new CoordinateSearch { Long = -48.040638, Lat = -15.84527, Radius = 1250 },
                    new CoordinateSearch { Long = -48.031883, Lat = -15.848243, Radius = 1250 },
                    new CoordinateSearch { Long = -48.020725, Lat = -15.847582, Radius = 1250 },
                    new CoordinateSearch { Long = -48.011456, Lat = -15.845105, Radius = 1250 },
                    new CoordinateSearch { Long = -48.003044, Lat = -15.840481, Radius = 1250 },
                    new CoordinateSearch { Long = -48.007507, Lat = -15.829251, Radius = 1250 },
                    new CoordinateSearch { Long = -48.013, Lat = -15.835692, Radius = 1250 },
                    new CoordinateSearch { Long = -48.019009, Lat = -15.838004, Radius = 1250 },
                    new CoordinateSearch { Long = -48.026733, Lat = -15.840481, Radius = 1250 },
                    new CoordinateSearch { Long = -48.0336, Lat = -15.83982, Radius = 1250 },
                    new CoordinateSearch { Long = -48.038921, Lat = -15.836187, Radius = 1250 },
                    new CoordinateSearch { Long = -48.040466, Lat = -15.82793, Radius = 1250 },
                    new CoordinateSearch { Long = -48.009911, Lat = -15.808936, Radius = 1250 },
                    new CoordinateSearch { Long = -48.019524, Lat = -15.812074, Radius = 1250 },
                    new CoordinateSearch { Long = -48.030338, Lat = -15.814056, Radius = 1250 },
                    new CoordinateSearch { Long = -48.039608, Lat = -15.820663, Radius = 1250 },
                    new CoordinateSearch { Long = -48.047848, Lat = -15.813231, Radius = 1250 },
                    new CoordinateSearch { Long = -48.044758, Lat = -15.803816, Radius = 1250 },
                    new CoordinateSearch { Long = -48.035145, Lat = -15.804807, Radius = 1250 },
                    new CoordinateSearch { Long = -48.026218, Lat = -15.804477, Radius = 1250 },
                    new CoordinateSearch { Long = -48.011799, Lat = -15.801834, Radius = 1250 },
                    new CoordinateSearch { Long = -48.010941, Lat = -15.773422, Radius = 1250 },
                    new CoordinateSearch { Long = -48.008366, Lat = -15.756571, Radius = 1250 },
                    new CoordinateSearch { Long = -48.016262, Lat = -15.76384, Radius = 1250 },
                    new CoordinateSearch { Long = -48.02536, Lat = -15.769457, Radius = 1250 },
                    new CoordinateSearch { Long = -48.023987, Lat = -15.756571, Radius = 1250 },
                    new CoordinateSearch { Long = -48.033085, Lat = -15.762519, Radius = 1250 },
                    new CoordinateSearch { Long = -48.042526, Lat = -15.757562, Radius = 1250 },
                    new CoordinateSearch { Long = -48.041668, Lat = -15.769127, Radius = 1250 },
                    new CoordinateSearch { Long = -48.009052, Lat = -15.789445, Radius = 1250 },
                    new CoordinateSearch { Long = -48.017635, Lat = -15.792749, Radius = 1250 },
                    new CoordinateSearch { Long = -48.02742, Lat = -15.794566, Radius = 1250 },
                    new CoordinateSearch { Long = -48.037376, Lat = -15.795062, Radius = 1250 },
                    new CoordinateSearch { Long = -48.047333, Lat = -15.795062, Radius = 1250 },
                    new CoordinateSearch { Long = -48.057117, Lat = -15.795888, Radius = 1250 },
                    new CoordinateSearch { Long = -48.061066, Lat = -15.820333, Radius = 1250 },
                    new CoordinateSearch { Long = -48.048706, Lat = -15.82793, Radius = 1250 },
                    new CoordinateSearch { Long = -48.045959, Lat = -15.834536, Radius = 1250 },
                    new CoordinateSearch { Long = -48.046989, Lat = -15.840151, Radius = 1250 },
                    new CoordinateSearch { Long = -48.056774, Lat = -15.841472, Radius = 1250 },
                    new CoordinateSearch { Long = -48.051624, Lat = -15.832719, Radius = 1250 },
                    new CoordinateSearch { Long = -48.052654, Lat = -15.83883, Radius = 1250 },
                    new CoordinateSearch { Long = -48.056259, Lat = -15.833875, Radius = 1250 },
                    new CoordinateSearch { Long = -48.058491, Lat = -15.837839, Radius = 1250 },
                    new CoordinateSearch { Long = -48.061581, Lat = -15.835031, Radius = 1250 },
                    new CoordinateSearch { Long = -48.075657, Lat = -15.831068, Radius = 1250 },
                    new CoordinateSearch { Long = -48.056774, Lat = -15.829581, Radius = 1250 },
                    new CoordinateSearch { Long = -48.072395, Lat = -15.821819, Radius = 1250 },
                    new CoordinateSearch { Long = -48.062954, Lat = -15.81257, Radius = 1250 },
                    new CoordinateSearch { Long = -48.062954, Lat = -15.800017, Radius = 1250 },
                    new CoordinateSearch { Long = -48.073254, Lat = -15.807119, Radius = 1250 },
                    new CoordinateSearch { Long = -48.082008, Lat = -15.811083, Radius = 1250 },
                    new CoordinateSearch { Long = -48.086815, Lat = -15.827599, Radius = 1250 },
                    new CoordinateSearch { Long = -48.095055, Lat = -15.813065, Radius = 1250 },
                    new CoordinateSearch { Long = -48.104153, Lat = -15.824296, Radius = 1250 },
                    new CoordinateSearch { Long = -48.101406, Lat = -15.837674, Radius = 1250 },
                    new CoordinateSearch { Long = -48.118916, Lat = -15.842132, Radius = 1250 },
                    new CoordinateSearch { Long = -48.116341, Lat = -15.829251, Radius = 1250 },
                    new CoordinateSearch { Long = -48.123035, Lat = -15.814552, Radius = 1250 },
                    new CoordinateSearch { Long = -48.108616, Lat = -15.808936, Radius = 1250 },
                    new CoordinateSearch { Long = -48.099003, Lat = -15.800182, Radius = 1250 },
                    new CoordinateSearch { Long = -48.115482, Lat = -15.796879, Radius = 1250 },
                    new CoordinateSearch { Long = -48.130417, Lat = -15.80745, Radius = 1250 },
                    new CoordinateSearch { Long = -48.128529, Lat = -15.793245, Radius = 1250 },
                    new CoordinateSearch { Long = -48.149128, Lat = -15.799026, Radius = 1250 },
                    new CoordinateSearch { Long = -48.107414, Lat = -15.932609, Radius = 1250 },
                    new CoordinateSearch { Long = -48.096771, Lat = -15.913461, Radius = 1250 },
                    new CoordinateSearch { Long = -48.075829, Lat = -15.908838, Radius = 1250 },
                    new CoordinateSearch { Long = -48.057632, Lat = -15.904876, Radius = 1250 },
                    new CoordinateSearch { Long = -48.044243, Lat = -15.895631, Radius = 1250 },
                    new CoordinateSearch { Long = -48.058319, Lat = -15.919404, Radius = 1250 },
                    new CoordinateSearch { Long = -48.077888, Lat = -15.945484, Radius = 1250 },
                    new CoordinateSearch { Long = -48.098145, Lat = -15.96661, Radius = 1250 },
                    new CoordinateSearch { Long = -48.061066, Lat = -15.958358, Radius = 1250 },
                    new CoordinateSearch { Long = -48.033257, Lat = -15.958688, Radius = 1250 },
                    new CoordinateSearch { Long = -48.006821, Lat = -15.959679, Radius = 1250 },
                    new CoordinateSearch { Long = -48.054543, Lat = -15.972882, Radius = 1250 },
                    new CoordinateSearch { Long = -48.067589, Lat = -15.989715, Radius = 1250 },
                    new CoordinateSearch { Long = -48.078232, Lat = -16.009186, Radius = 1250 },
                    new CoordinateSearch { Long = -48.074799, Lat = -16.026676, Radius = 1250 },
                    new CoordinateSearch { Long = -48.054199, Lat = -16.012816, Radius = 1250 },
                    new CoordinateSearch { Long = -48.030853, Lat = -16.041524, Radius = 1250 },
                    new CoordinateSearch { Long = -48.019524, Lat = -16.019746, Radius = 1250 },
                    new CoordinateSearch { Long = -47.990341, Lat = -16.008196, Radius = 1250 },
                    new CoordinateSearch { Long = -47.987251, Lat = -16.035915, Radius = 1250 },
                    new CoordinateSearch { Long = -47.994118, Lat = -15.981793, Radius = 1250 },
                    new CoordinateSearch { Long = -48.009911, Lat = -16.043174, Radius = 1250 },
                    new CoordinateSearch { Long = -47.958069, Lat = -16.042514, Radius = 1250 },
                    new CoordinateSearch { Long = -47.993774, Lat = -15.939542, Radius = 1250 },
                    new CoordinateSearch { Long = -47.964935, Lat = -15.937561, Radius = 1250 },
                    new CoordinateSearch { Long = -47.941246, Lat = -15.920724, Radius = 1250 },
                    new CoordinateSearch { Long = -47.92408, Lat = -15.896952, Radius = 1250 },
                    new CoordinateSearch { Long = -47.918243, Lat = -15.864921, Radius = 1250 },
                    new CoordinateSearch { Long = -47.943993, Lat = -15.86426, Radius = 1250 },
                    new CoordinateSearch { Long = -47.969055, Lat = -15.8778, Radius = 1250 },
                    new CoordinateSearch { Long = -48.003731, Lat = -15.887046, Radius = 1250 },
                    new CoordinateSearch { Long = -48.030853, Lat = -15.886716, Radius = 1250 },
                    new CoordinateSearch { Long = -48.011627, Lat = -15.913131, Radius = 1250 },
                    new CoordinateSearch { Long = -47.984161, Lat = -15.895961, Radius = 1250 },
                    new CoordinateSearch { Long = -47.951889, Lat = -15.884404, Radius = 1250 },
                    new CoordinateSearch { Long = -47.932663, Lat = -15.880111, Radius = 1250 },
                    new CoordinateSearch { Long = -47.943993, Lat = -15.901905, Radius = 1250 },
                    new CoordinateSearch { Long = -47.962875, Lat = -15.913131, Radius = 1250 },
                    new CoordinateSearch { Long = -47.986565, Lat = -15.918743, Radius = 1250 },
                    new CoordinateSearch { Long = -48.03566, Lat = -15.937561, Radius = 1250 },
                    new CoordinateSearch { Long = -48.107414, Lat = -15.932609, Radius = 1250 },
                    new CoordinateSearch { Long = -47.903137, Lat = -15.885395, Radius = 1250 },
                    new CoordinateSearch { Long = -47.91172, Lat = -15.904876, Radius = 1250 },
                    new CoordinateSearch { Long = -47.924423, Lat = -15.923366, Radius = 1250 },
                    new CoordinateSearch { Long = -47.941933, Lat = -15.939542, Radius = 1250 },
                    new CoordinateSearch { Long = -47.973175, Lat = -15.953077, Radius = 1250 },
                    new CoordinateSearch { Long = -47.941246, Lat = -16.045154, Radius = 1250 },
                    new CoordinateSearch { Long = -47.814217, Lat = -16.046473, Radius = 1250 },
                    new CoordinateSearch { Long = -47.686501, Lat = -16.043834, Radius = 1250 },
                    new CoordinateSearch { Long = -47.903137, Lat = -15.885395, Radius = 1250 },
                    new CoordinateSearch { Long = -47.996521, Lat = -15.7721, Radius = 1250 },
                    new CoordinateSearch { Long = -47.993774, Lat = -15.796218, Radius = 1250 },
                    new CoordinateSearch { Long = -47.993431, Lat = -15.816699, Radius = 1250 },
                    new CoordinateSearch { Long = -47.987251, Lat = -15.840481, Radius = 1250 },
                    new CoordinateSearch { Long = -47.972832, Lat = -15.861288, Radius = 1250 },
                    new CoordinateSearch { Long = -47.947426, Lat = -15.853692, Radius = 1250 },
                    new CoordinateSearch { Long = -47.92717, Lat = -15.856995, Radius = 1250 },
                    new CoordinateSearch { Long = -47.939873, Lat = -15.834866, Radius = 1250 },
                    new CoordinateSearch { Long = -47.966995, Lat = -15.842132, Radius = 1250 },
                    new CoordinateSearch { Long = -47.976608, Lat = -15.819342, Radius = 1250 },
                    new CoordinateSearch { Long = -47.976608, Lat = -15.804146, Radius = 1250 },
                    new CoordinateSearch { Long = -47.978668, Lat = -15.786637, Radius = 1250 },
                    new CoordinateSearch { Long = -47.960815, Lat = -15.761527, Radius = 1250 },
                    new CoordinateSearch { Long = -47.93026, Lat = -15.765162, Radius = 1250 },
                    new CoordinateSearch { Long = -47.956352, Lat = -15.798861, Radius = 1250 },
                    new CoordinateSearch { Long = -47.957726, Lat = -15.818351, Radius = 1250 },
                    new CoordinateSearch { Long = -47.940903, Lat = -15.808771, Radius = 1250 },
                    new CoordinateSearch { Long = -47.927856, Lat = -15.792254, Radius = 1250 },
                    new CoordinateSearch { Long = -47.920303, Lat = -15.767805, Radius = 1250 },
                    new CoordinateSearch { Long = -47.910004, Lat = -15.745006, Radius = 1250 },
                    new CoordinateSearch { Long = -47.892494, Lat = -15.738728, Radius = 1250 },
                    new CoordinateSearch { Long = -47.887001, Lat = -15.763179, Radius = 1250 },
                    new CoordinateSearch { Long = -47.886658, Lat = -15.774413, Radius = 1250 },
                    new CoordinateSearch { Long = -47.886314, Lat = -15.784325, Radius = 1250 },
                    new CoordinateSearch { Long = -47.893524, Lat = -15.811744, Radius = 1250 },
                    new CoordinateSearch { Long = -47.90554, Lat = -15.820663, Radius = 1250 },
                    new CoordinateSearch { Long = -47.916527, Lat = -15.836187, Radius = 1250 },
                    new CoordinateSearch { Long = -47.935066, Lat = -15.818351, Radius = 1250 },
                    new CoordinateSearch { Long = -47.917213, Lat = -15.80778, Radius = 1250 },
                    new CoordinateSearch { Long = -47.909317, Lat = -15.795888, Radius = 1250 },
                    new CoordinateSearch { Long = -47.90554, Lat = -15.778378, Radius = 1250 },
                    new CoordinateSearch { Long = -47.900391, Lat = -15.756902, Radius = 1250 },
                    new CoordinateSearch { Long = -47.872925, Lat = -15.75591, Radius = 1250 },
                    new CoordinateSearch { Long = -47.874641, Lat = -15.777056, Radius = 1250 },
                    new CoordinateSearch { Long = -47.875328, Lat = -15.792584, Radius = 1250 },
                    new CoordinateSearch { Long = -47.879105, Lat = -15.806789, Radius = 1250 },
                    new CoordinateSearch { Long = -47.885284, Lat = -15.822975, Radius = 1250 },
                    new CoordinateSearch { Long = -47.893524, Lat = -15.834205, Radius = 1250 },
                    new CoordinateSearch { Long = -47.923737, Lat = -15.843784, Radius = 1250 },
                    new CoordinateSearch { Long = -47.911034, Lat = -15.856664, Radius = 1250 },
                    new CoordinateSearch { Long = -47.895584, Lat = -15.85138, Radius = 1250 },
                    new CoordinateSearch { Long = -47.874298, Lat = -15.840481, Radius = 1250 },
                    new CoordinateSearch { Long = -47.872238, Lat = -15.860297, Radius = 1250 },
                    new CoordinateSearch { Long = -47.849579, Lat = -15.870204, Radius = 1250 },
                    new CoordinateSearch { Long = -47.837219, Lat = -15.852041, Radius = 1250 },
                    new CoordinateSearch { Long = -47.824173, Lat = -15.837178, Radius = 1250 },
                    new CoordinateSearch { Long = -47.807693, Lat = -15.825948, Radius = 1250 },
                    new CoordinateSearch { Long = -47.801514, Lat = -15.80778, Radius = 1250 },
                    new CoordinateSearch { Long = -47.789497, Lat = -15.82793, Radius = 1250 },
                    new CoordinateSearch { Long = -47.798767, Lat = -15.844114, Radius = 1250 },
                    new CoordinateSearch { Long = -47.807007, Lat = -15.860958, Radius = 1250 },
                    new CoordinateSearch { Long = -47.81353, Lat = -15.876479, Radius = 1250 },
                    new CoordinateSearch { Long = -47.814903, Lat = -15.904546, Radius = 1250 },
                    new CoordinateSearch { Long = -47.823143, Lat = -15.926997, Radius = 1250 },
                    new CoordinateSearch { Long = -47.862968, Lat = -15.807119, Radius = 1250 },
                    new CoordinateSearch { Long = -47.835159, Lat = -15.799191, Radius = 1250 },
                    new CoordinateSearch { Long = -47.862282, Lat = -15.78895, Radius = 1250 },
                    new CoordinateSearch { Long = -47.838593, Lat = -15.749302, Radius = 1250 },
                    new CoordinateSearch { Long = -47.855072, Lat = -15.736745, Radius = 1250 },
                    new CoordinateSearch { Long = -47.876358, Lat = -15.725179, Radius = 1250 },
                    new CoordinateSearch { Long = -47.893524, Lat = -15.719891, Radius = 1250 },
                    new CoordinateSearch { Long = -47.92305, Lat = -15.753267, Radius = 1250 },
                    new CoordinateSearch { Long = -47.996521, Lat = -15.7721, Radius = 1250 },
                    new CoordinateSearch { Long = -47.853699, Lat = -15.681882, Radius = 1250 },
                    new CoordinateSearch { Long = -47.812843, Lat = -15.655106, Radius = 1250 },
                    new CoordinateSearch { Long = -47.764091, Lat = -15.647833, Radius = 1250 },
                    new CoordinateSearch { Long = -47.705727, Lat = -15.688493, Radius = 1250 },
                    new CoordinateSearch { Long = -47.814217, Lat = -15.637254, Radius = 1250 },
                    new CoordinateSearch { Long = -47.735596, Lat = -15.693781, Radius = 1250 },
                    new CoordinateSearch { Long = -47.794647, Lat = -15.682543, Radius = 1250 },
                    new CoordinateSearch { Long = -47.834473, Lat = -15.714603, Radius = 1250 },
                    new CoordinateSearch { Long = -47.80426, Lat = -15.744676, Radius = 1250 },
                    new CoordinateSearch { Long = -47.762718, Lat = -15.706341, Radius = 1250 },
                    new CoordinateSearch { Long = -47.743492, Lat = -15.747319, Radius = 1250 },
                    new CoordinateSearch { Long = -47.787094, Lat = -15.772431, Radius = 1250 },
                    new CoordinateSearch { Long = -47.777824, Lat = -15.748971, Radius = 1250 },
                    new CoordinateSearch { Long = -47.785721, Lat = -15.7982, Radius = 1250 },
                    new CoordinateSearch { Long = -47.81147, Lat = -15.804807, Radius = 1250 },
                    new CoordinateSearch { Long = -47.808037, Lat = -15.764832, Radius = 1250 },
                    new CoordinateSearch { Long = -47.835846, Lat = -15.790271, Radius = 1250 },
                    new CoordinateSearch { Long = -47.754478, Lat = -15.854352, Radius = 1250 },
                    new CoordinateSearch { Long = -47.793274, Lat = -15.867232, Radius = 1250 },
                    new CoordinateSearch { Long = -47.788124, Lat = -15.896622, Radius = 1250 },
                    new CoordinateSearch { Long = -47.785034, Lat = -15.937892, Radius = 1250 },
                    new CoordinateSearch { Long = -47.751389, Lat = -15.939212, Radius = 1250 },
                    new CoordinateSearch { Long = -47.761688, Lat = -15.906857, Radius = 1250 },
                    new CoordinateSearch { Long = -47.765121, Lat = -15.872186, Radius = 1250 },
                    new CoordinateSearch { Long = -47.627106, Lat = -15.635931, Radius = 1250 },
                    new CoordinateSearch { Long = -47.627792, Lat = -15.597907, Radius = 1250 },
                    new CoordinateSearch { Long = -47.669678, Lat = -15.598237, Radius = 1250 },
                    new CoordinateSearch { Long = -47.637405, Lat = -15.622706, Radius = 1250 },
                    new CoordinateSearch { Long = -47.685814, Lat = -15.62469, Radius = 1250 },
                    new CoordinateSearch { Long = -47.655602, Lat = -15.647503, Radius = 1250 },
                    new CoordinateSearch { Long = -47.679291, Lat = -15.60452, Radius = 1250 },
                    new CoordinateSearch { Long = -47.853699, Lat = -15.681882, Radius = 1250 },
                    new CoordinateSearch { Long = -47.338715, Lat = -15.711629, Radius = 1250 },
                    new CoordinateSearch { Long = -47.339401, Lat = -15.641552, Radius = 1250 },
                    new CoordinateSearch { Long = -47.36412, Lat = -15.666676, Radius = 1250 },
                    new CoordinateSearch { Long = -47.349701, Lat = -15.68651, Radius = 1250 },
                    new CoordinateSearch { Long = -47.360001, Lat = -15.71229, Radius = 1250 },
                    new CoordinateSearch { Long = -47.425919, Lat = -15.69907, Radius = 1250 },
                    new CoordinateSearch { Long = -47.3909, Lat = -15.647503, Radius = 1250 },
                    new CoordinateSearch { Long = -47.344208, Lat = -15.560206, Radius = 1250 },
                    new CoordinateSearch { Long = -47.296829, Lat = -15.526468, Radius = 1250 },
                    new CoordinateSearch { Long = -47.347641, Lat = -15.536391, Radius = 1250 },
                    new CoordinateSearch { Long = -47.349014, Lat = -15.498017, Radius = 1250 },
                    new CoordinateSearch { Long = -47.381287, Lat = -15.523821, Radius = 1250 },
                    new CoordinateSearch { Long = -47.365494, Lat = -15.550283, Radius = 1250 },
                    new CoordinateSearch { Long = -47.313309, Lat = -15.568144, Radius = 1250 },
                    new CoordinateSearch { Long = -47.277603, Lat = -15.538376, Radius = 1250 },
                    new CoordinateSearch { Long = -47.269363, Lat = -15.561529, Radius = 1250 },
                    new CoordinateSearch { Long = -47.397079, Lat = -15.5946, Radius = 1250 },
                    new CoordinateSearch { Long = -47.444458, Lat = -15.525806, Radius = 1250 },
                    new CoordinateSearch { Long = -47.586594, Lat = -15.503311, Radius = 1250 },
                    new CoordinateSearch { Long = -47.631226, Lat = -15.498017, Radius = 1250 },
                    new CoordinateSearch { Long = -47.69783, Lat = -15.589309, Radius = 1250 },
                    new CoordinateSearch { Long = -47.535782, Lat = -15.593277, Radius = 1250 },
                    new CoordinateSearch { Long = -47.518616, Lat = -15.532422, Radius = 1250 },
                    new CoordinateSearch { Long = -47.423172, Lat = -15.61444, Radius = 1250 },
                    new CoordinateSearch { Long = -47.465744, Lat = -15.698409, Radius = 1250 },
                    new CoordinateSearch { Long = -47.732849, Lat = -15.523821, Radius = 1250 },
                    new CoordinateSearch { Long = -47.849579, Lat = -15.518528, Radius = 1250 },
                    new CoordinateSearch { Long = -47.953949, Lat = -15.516544, Radius = 1250 },
                    new CoordinateSearch { Long = -47.989655, Lat = -15.507942, Radius = 1250 },
                    new CoordinateSearch { Long = -48.03154, Lat = -15.524483, Radius = 1250 },
                    new CoordinateSearch { Long = -47.879105, Lat = -15.602536, Radius = 1250 },
                    new CoordinateSearch { Long = -47.753448, Lat = -15.579388, Radius = 1250 },
                    new CoordinateSearch { Long = -47.860565, Lat = -15.659404, Radius = 1250 },
                    new CoordinateSearch { Long = -47.890091, Lat = -15.621053, Radius = 1250 },
                    new CoordinateSearch { Long = -47.859192, Lat = -15.701714, Radius = 1250 },
                    new CoordinateSearch { Long = -48.171616, Lat = -15.547637, Radius = 1250 },
                    new CoordinateSearch { Long = -48.208008, Lat = -15.67461, Radius = 1250 },
                    new CoordinateSearch { Long = -48.180542, Lat = -15.687171, Radius = 1250 },
                    new CoordinateSearch { Long = -48.164749, Lat = -15.652131, Radius = 1250 },
                    new CoordinateSearch { Long = -48.181229, Lat = -15.659404, Radius = 1250 },
                    new CoordinateSearch { Long = -48.194275, Lat = -15.70568, Radius = 1250 },
                    new CoordinateSearch { Long = -48.219681, Lat = -15.638907, Radius = 1250 },
                    new CoordinateSearch { Long = -48.224487, Lat = -15.700392, Radius = 1250 },
                    new CoordinateSearch { Long = -47.911377, Lat = -15.713612, Radius = 1250 },
                    new CoordinateSearch { Long = -47.903137, Lat = -15.695764, Radius = 1250 },
                    new CoordinateSearch { Long = -47.846832, Lat = -15.632294, Radius = 1250 },
                    new CoordinateSearch { Long = -47.602386, Lat = -15.546976, Radius = 1250 },
                    new CoordinateSearch { Long = -47.590714, Lat = -15.696425, Radius = 1250 },
                    new CoordinateSearch { Long = -47.487717, Lat = -15.628988, Radius = 1250 },
                    new CoordinateSearch { Long = -47.512436, Lat = -15.590632, Radius = 1250 },
                    new CoordinateSearch { Long = -47.331848, Lat = -15.603198, Radius = 1250 }
            };

            var results = new List<Result>();

            async Task searchAsync(string term, bool doCoords = false)
            {
                var url = "https://maps.googleapis.com/maps/api/place/textsearch/json"
                    .SetQueryParams(new
                    {
                        key = GoogleApiKey,
                        language = "pt-BR",
                        query = term,
                    });

                if (doCoords == true)
                {
                    url.SetQueryParam("type", "school");
                }

                string d(double v) => v.ToString().Replace(",", ".");

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

                    // Thread.Sleep(500);

                    var newResults = response.results
                        .ToList();

                    foreach (var result in newResults)
                    {
                        result.term = term;
                    }

                    results.AddRange(newResults);

                    pageToken = response.next_page_token;
                }

                if (doCoords == false)
                {
                    await appendToResultsAsync();

                    while (string.IsNullOrWhiteSpace((pageToken)) == false)
                    {
                        await appendToResultsAsync();
                    }
                }
                else
                {
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
                }
            }

            foreach (var term in terms)
            {
                await searchAsync(term, true);
            }

            foreach (var school in schools)
            {
                await searchAsync(school, false);
            }


            await WriteFiles(GoogleApiKey, results);

            Console.WriteLine($"{results.Count()} resultados no total encontrados");
            Console.WriteLine($"{results.GroupBy(x => x.place_id).Count()} resultados distintos encontrados");
        }

        private static async Task<List<AddressComponents>> GetAddressAsync(string googleApiKey, string placeId)
        {
            var url = "https://maps.googleapis.com/maps/api/place/details/json"
                .SetQueryParams(new
                {
                    key = googleApiKey,
                    language = "pt-BR",
                    placeid = placeId
                });

            Console.WriteLine("");
            Console.WriteLine("============================================================");
            Console.WriteLine(Url.Decode(url.ToString(), false));
            Console.WriteLine("============================================================");
            Console.WriteLine("");

            var response = await url.GetJsonAsync<DetailsResult>();

            // Thread.Sleep(500);

            return response.result.address_components;
        }

        private static async Task WriteFiles(String googleApiKey, List<Result> results)
        {
            results = results
                .Where(x => x != null)
                .GroupBy(x => x.place_id)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var fileName = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using (var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, $"{fileName}.csv")))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(results.OrderBy(x => x.place_id).ToList());
                }
            }

            var expandoDic = new Dictionary<string, object>();

            foreach (var item in results)
            {
                try
                {
                    item.address_components = await GetAddressAsync(googleApiKey, item.place_id);

                    var ra = item.address_components?.FirstOrDefault(x => x.types.Contains("administrative_area_level_4"))?.long_name;

                    expandoDic[item.place_id] = new FirebaseSchool
                    {
                        name = item.name,
                        business_status = item.business_status,
                        address = item.formatted_address,
                        latitude = item.geometry.location.lat,
                        longitude = item.geometry.location.lng,
                        user_ratings_total = item.user_ratings_total,
                        rating = item.rating,
                        ra = ra
                    };
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            string json = JsonConvert.SerializeObject(expandoDic.ToExpando(), Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            //write string to file
            System.IO.File.WriteAllText(Path.Combine(Environment.CurrentDirectory, $"{fileName}.json"), json);
        }


        class Response
        {
            public string next_page_token { get; set; }
            public List<Result> results { get; set; }
        }

        class Result
        {
            public string term { get; set; }
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
            public ResultGeometry geometry { get; set; }
            public List<AddressComponents> address_components { get; set; }
        }

        class ResultGeometry
        {
            public ResultGeometryLocation location { get; set; }
        }

        class ResultGeometryLocation
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        class FirebaseSchool
        {
            public string name { get; set; }
            public bool banned { get; set; }
            public string business_status { get; set; }
            public double rating { get; set; }
            public double user_ratings_total { get; set; }
            public string address { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string ra { get; set; }
            public string ra_code { get; set; }
        }

        class DetailsResult
        {
            public DetailsDetailsResult result { get; set; }
        }

        class DetailsDetailsResult
        {
            public List<AddressComponents> address_components { get; set; }
        }

        class AddressComponents
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public List<string> types { get; set; }
        }
    }

    public static class Extensions
    {
        public static string RemoveAccents(this string text)
        {
            return new string(text
                .Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }

        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;
            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }
                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }
            return expando;
        }
    }
}
