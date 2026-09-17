using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPL_2024_standings
{

    public class StandingsItem
    {
        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("team")]
        public TeamInfo? Team { get; set; }

        [JsonPropertyName("played")]
        public int Played { get; set; }

        [JsonPropertyName("won")]
        public int Won { get; set; }

        [JsonPropertyName("drawn")]
        public int Drawn { get; set; }

        [JsonPropertyName("lost")]
        public int Lost { get; set; }

        [JsonPropertyName("goalsFor")]
        public int GoalsFor { get; set; }

        [JsonPropertyName("goalsAgainst")]
        public int GoalsAgainst { get; set; }

        [JsonPropertyName("goalDiff")]
        public int GoalDiff { get; set; }

        [JsonPropertyName("points")]
        public int Points { get; set; }

        [JsonPropertyName("form")]
        public string? Form { get; set; }
    }
}

    

