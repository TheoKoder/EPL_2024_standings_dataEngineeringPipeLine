using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPL_2024_standings
{
    public class LeagueData
    {
        [JsonPropertyName("standings")]
        public List<List<StandingsItem>> Standings { get; set; } = new();
    }
}

