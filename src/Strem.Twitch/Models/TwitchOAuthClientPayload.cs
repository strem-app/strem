﻿using System.Text.Json.Serialization;

namespace Strem.Twitch.Models;

public class TwitchOAuthClientPayload
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    public string Scope { get; set; }
    public string State { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
}