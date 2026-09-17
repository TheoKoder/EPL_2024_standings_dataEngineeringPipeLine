using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPL_2024_standings
{

    public class ResponseItem
    {

        [JsonPropertyName("league")]
        public LeagueData League { get; set; } = new();

    }

}