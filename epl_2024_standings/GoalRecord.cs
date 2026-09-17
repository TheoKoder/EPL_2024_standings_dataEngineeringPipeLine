using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPL_2024_standings
{ 
	public class GoalRecord
	{
		[JsonPropertyName("for")]
		public int For {  get; set; }

		[JsonPropertyName("against")]
		public int Against { get; set; }
	}
}

