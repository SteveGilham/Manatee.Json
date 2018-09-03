﻿using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class PatternPropertiesKeyword : Dictionary<string, JsonSchema>, IJsonSchemaKeyword
	{
		public string Name => "patternProperties";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			// need to determine how to break out the work between properties, patternProperties, and additionalProperties
			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			foreach (var kvp in json.Object)
			{
				this[kvp.Key] = serializer.Deserialize<JsonSchema>(kvp.Value);
			}
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.ToDictionary(kvp => kvp.Key,
			                         kvp => serializer.Serialize<JsonSchema>(kvp.Value))
				.ToJson();
		}
	}
}