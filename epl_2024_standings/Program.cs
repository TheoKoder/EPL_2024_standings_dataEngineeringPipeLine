using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.Data.SqlClient;

namespace EPL_2024_standings
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Env.Load();

                string apiKey = Environment.GetEnvironmentVariable("RAPIDAPI_KEY") ?? string.Empty;
                string dbConnStr = Environment.GetEnvironmentVariable("SQL_CONN_STRING") ?? string.Empty;

                if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(dbConnStr))
                {
                    Console.WriteLine("[Failure] - RAPIDAPI_KEY or SQL_CONN_STRING is missing in .env!");
                    return;
                }

                // Season parameter matching API requirement
                string seasons = "2025-26";
                string baseUrl = $"https://premier-league18.p.rapidapi.com/standings?seasons={seasons}";

                Console.WriteLine($"Fetching Premier League standings data for season {seasons}...");
                List<StandingRecord> standings = await FetchStandingsAsync(baseUrl, apiKey);

                Console.WriteLine($"Loading {standings.Count} Team records into DB...");

                if (standings.Count > 0)
                {
                    await UpsertStandingsToDbAsync(standings, dbConnStr);
                    Console.WriteLine("[Success] - ETL process completed successfully!");
                }
                else
                {
                    Console.WriteLine("[Notice] - Skipping DB load because 0 records were returned.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task<List<StandingRecord>> FetchStandingsAsync(string baseUrl, string apiKey)
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}"),
                Headers =
                {
                    { "x-rapidapi-key", apiKey.Trim() },
                    { "x-rapidapi-host", "premier-league18.p.rapidapi.com" }
                }
            };

            using var response = await client.SendAsync(request);
            string json = await response.Content.ReadAsStringAsync();
           
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API Error {(int)response.StatusCode}]: {json}");
                response.EnsureSuccessStatusCode();
            }

            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResults = JsonSerializer.Deserialize<ApiResponse>(json, opts);

            var standingsList = new List<StandingRecord>();

            if (apiResults?.Data != null && apiResults.Data.Count > 0)
            {
                foreach (var item in apiResults.Data)
                {
                    standingsList.Add(new StandingRecord
                    {
                        Position = item.Position,
                        TeamId = item.Team?.Id ?? 0,
                        TeamName = item.Team?.Name ?? "Unknown",
                        Played = item.Played,
                        Won = item.Won,
                        Draw = item.Drawn,
                        Lost = item.Lost,
                        GoalsFor = item.GoalsFor,
                        GoalsAgainst = item.GoalsAgainst,
                        GoalDiff = item.GoalDiff,
                        Points = item.Points,
                        Form = item.Form ?? string.Empty
                    });
                }
            }
            else
            {
                Console.WriteLine("[Warning] [Warning] API returned an empty or unparseable data array.");
            }

            return standingsList;
        }

        private static async Task UpsertStandingsToDbAsync(List<StandingRecord> standings, string connectionStr)
        {
            using var connection = new SqlConnection(connectionStr);
            await connection.OpenAsync();

            string mergeQuery = @"
            MERGE premierleagueTeams AS Target
            USING (SELECT  @Position AS Position, @TeamId AS TeamId, 
                          @TeamName AS TeamName, @Played AS Played, @Won AS Won, 
                          @Draw AS Draw, @Lost AS Lost, @GoalsFor AS GoalsFor, 
                          @GoalsAgainst AS GoalsAgainst, @GoalDiff AS GoalDiff, 
                          @Points AS Points, @Form AS Form) AS Source
            ON Target.TeamId = Source.TeamId
            WHEN MATCHED THEN
                UPDATE SET 
                    Position     = Source.Position,
                    TeamName     = Source.TeamName,
                    Played       = Source.Played,
                    Won          = Source.Won,
                    Draw         = Source.Draw,
                    Lost         = Source.Lost,
                    GoalsFor     = Source.GoalsFor,
                    GoalsAgainst = Source.GoalsAgainst,
                    GoalDiff     = Source.GoalDiff,
                    Points       = Source.Points,
                    Form         = Source.Form
            WHEN NOT MATCHED THEN
                INSERT ( Position, TeamId, TeamName, Played, Won, Draw, Lost, GoalsFor, GoalsAgainst, GoalDiff, Points, Form)
                VALUES ( Source.Position, Source.TeamId, Source.TeamName, 
                        Source.Played, Source.Won, Source.Draw, Source.Lost, 
                        Source.GoalsFor, Source.GoalsAgainst, Source.GoalDiff, Source.Points, Source.Form);";

            foreach (var row in standings)
            {
                using var command = new SqlCommand(mergeQuery, connection);
                command.Parameters.AddWithValue("@Position", row.Position);
                command.Parameters.AddWithValue("@TeamId", row.TeamId);
                command.Parameters.AddWithValue("@TeamName", row.TeamName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Played", row.Played);
                command.Parameters.AddWithValue("@Won", row.Won);
                command.Parameters.AddWithValue("@Draw", row.Draw);
                command.Parameters.AddWithValue("@Lost", row.Lost);
                command.Parameters.AddWithValue("@GoalsFor", row.GoalsFor);
                command.Parameters.AddWithValue("@GoalsAgainst", row.GoalsAgainst);
                command.Parameters.AddWithValue("@GoalDiff", row.GoalDiff);
                command.Parameters.AddWithValue("@Points", row.Points);
                command.Parameters.AddWithValue("@Form", row.Form ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}