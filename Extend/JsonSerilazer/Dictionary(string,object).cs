using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebGal.Extend.Json;

public class DictionaryStringObjectConverter : JsonConverter<Dictionary<string, object>>
{
	public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var result = new Dictionary<string, object>();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				return result;

			var propName = reader.GetString() ?? throw new JsonException();
			reader.Read();
			object value;
			switch (reader.TokenType)
			{
				case JsonTokenType.Number:
					if (reader.TryGetInt32(out int l))
						value = l;
					else if (reader.TryGetInt64(out long ll))
						value = ll;
					else
						value = (float)reader.GetDouble();
					break;
				case JsonTokenType.String:
					value = reader.GetString() ?? string.Empty;
					break;
				default:
					throw new JsonException();
			}
			result.Add(propName, value);
		}
		throw new JsonException();
	}

	public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		foreach (var kvp in value)
		{
			writer.WritePropertyName(kvp.Key);
			if (kvp.Value is int l)
				writer.WriteNumberValue(l);
			else if (kvp.Value is long ll)
				writer.WriteNumberValue(ll);
			else if (kvp.Value is double d)
				writer.WriteNumberValue(d);
			else
				writer.WriteStringValue(kvp.Value.ToString());
		}

		writer.WriteEndObject();
	}
}