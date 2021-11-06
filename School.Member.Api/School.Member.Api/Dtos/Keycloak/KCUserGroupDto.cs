using System;
using System.Text.Json.Serialization;

public record KCUserGroupDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}