using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EPL_2024_standings
{
    public class StatsRecord
    {
        [JsonPropertyName("played")]
        public int Played { get; set; }

        [JsonPropertyName("win")]
        public int Win { get; set; }

        [JsonPropertyName("draw")]
        public int Draw { get; set; }

        [JsonPropertyName("lose")]
        public int Lose { get; set; }

        [JsonPropertyName("goals")]
        public GoalRecord Goals { get; set; } = new();
    }
}

