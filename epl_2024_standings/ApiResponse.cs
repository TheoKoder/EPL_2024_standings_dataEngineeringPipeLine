using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPL_2024_standings
{
    public class ApiResponse
    {

        [JsonPropertyName("data")]
        public List<StandingsItem>? Data { get; set; }

        public ApiResponse() { }
    }
}

