using System.Text.Json.Serialization;

namespace ManoelsPackaging.Api.Contracts.Responses
{
    /// <summary>
    /// Represents the response for a batch of packaging requests, detailing the outcome for each order.
    /// </summary>
    public class PackagingBatchResponse
    {
        /// <summary>
        /// Gets or sets the list of packaging results for each processed order.
        /// </summary>
        [JsonPropertyName("pedidos")]
        public List<OrderPackagingResultDto> Orders { get; set; } = new List<OrderPackagingResultDto>();
    }
}
