using System.Text.Json.Serialization;

namespace ManoelsPackaging.Api.Contracts.Responses
{
    /// <summary>
    /// Represents the allocation of products to a specific box.
    /// </summary>
    public class BoxAllocationDto
    {
        /// <summary>
        /// Gets or sets the identifier of the box used.
        /// Can be null if no suitable box was found for the products (indicated by an observation).
        /// </summary>
        [JsonPropertyName("caixa_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BoxId { get; set; }

        /// <summary>
        /// Gets or sets the list of product identifiers placed in this box.
        /// </summary>
        [JsonPropertyName("produtos")]
        public List<string> ProductIds { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets an optional observation, e.g., if products could not be fitted into any available box.
        /// </summary>
        [JsonPropertyName("observacao")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Observation { get; set; }
    }
}
