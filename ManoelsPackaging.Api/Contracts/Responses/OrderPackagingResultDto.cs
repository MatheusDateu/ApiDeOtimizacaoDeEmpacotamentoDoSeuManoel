using System.Text.Json.Serialization;

namespace ManoelsPackaging.Api.Contracts.Responses
{
    /// <summary>
    /// Represents the packaging result for a single order.
    /// </summary>
    public class OrderPackagingResultDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the order.
        /// </summary>
        [JsonPropertyName("pedido_id")]
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the list of boxes used for this order and their contents.
        /// </summary>
        [JsonPropertyName("caixas")]
        public List<BoxAllocationDto> Boxes { get; set; } = new List<BoxAllocationDto>();
    }
}
