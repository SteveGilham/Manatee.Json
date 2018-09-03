﻿using System;
using System.Reflection;
using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SchemaSerializer : IPrioritizedSerializer
	{
		public int Priority => int.MinValue;

		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return typeof(JsonSchema).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
		}
		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var schema = (JsonSchema)(object) obj;
			return schema.ToJson(serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = JsonSchemaFactory.FromJson(json);
			return (T) (object) value;
		}
	}
}