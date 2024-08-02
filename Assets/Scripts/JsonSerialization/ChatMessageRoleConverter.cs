using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAI_API.Chat;

public class ChatMessageRoleConverter : JsonConverter<ChatMessageRole>
{
    public override ChatMessageRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString();
        return ChatMessageRole.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, ChatMessageRole value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}