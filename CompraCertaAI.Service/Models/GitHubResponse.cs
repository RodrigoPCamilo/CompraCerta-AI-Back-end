using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CompraCertaAI.Service.Models
{
    public class GitHubResponse
    {
        [JsonPropertyName("choices")]
        public List<GitHubChoice> Choices { get; set; } = new();
    }

    public class GitHubChoice
    {
        [JsonPropertyName("message")]
        public GitHubMessage Message { get; set; } = new();
    }

    public class GitHubMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}