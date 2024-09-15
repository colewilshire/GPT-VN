public static class JsonSchemaDefinitions
{
    public static readonly string CharacterDescriptionsSchema = @"
    {
      ""type"": ""object"",
      ""properties"": {
        ""CharacterDescriptions"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""object"",
            ""properties"": {
              ""Name"": { ""type"": ""string"" },
              ""BodyType"": { ""type"": ""string"" },
              ""Hair"": { ""type"": ""string"" },
              ""Outfit"": { ""type"": ""string"" },
              ""Accessory"": { ""type"": ""string"" },
              ""Eyes"": { ""type"": ""string"" }
            },
            ""required"": [""Name"", ""BodyType"", ""Hair"", ""Outfit"", ""Accessory"", ""Eyes""],
            ""additionalProperties"": false
          }
        }
      },
      ""required"": [""CharacterDescriptions""],
      ""additionalProperties"": false
    }
    ";

    public static readonly string DialogueSchema = @"
    {
      ""type"": ""object"",
      ""properties"": {
        ""DialogueLines"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""object"",
            ""properties"": {
              ""CharacterName"": { ""type"": ""string"" },
              ""DialogueText"": { ""type"": ""string"" },
              ""Mood"": { ""type"": ""string"" },
              ""BackgroundDescription"": { ""type"": ""string"" }
            },
            ""required"": [""CharacterName"", ""DialogueText"", ""Mood"", ""BackgroundDescription""],
            ""additionalProperties"": false
          }
        }
      },
      ""required"": [""DialogueLines""],
      ""additionalProperties"": false
    }
    ";

    public static readonly string ChoiceSchema = @"
    {
      ""type"": ""object"",
      ""properties"": {
        ""Choices"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""object"",
            ""properties"": {
              ""CharacterName"": { ""type"": ""string"" },
              ""DialogueText"": { ""type"": ""string"" },
              ""Mood"": { ""type"": ""string"" },
              ""BackgroundDescription"": { ""type"": ""string"" }
            },
            ""required"": [""CharacterName"", ""DialogueText"", ""Mood"", ""BackgroundDescription""],
            ""additionalProperties"": false
          }
        }
      },
      ""required"": [""Choices""],
      ""additionalProperties"": false
    }
    ";
}
