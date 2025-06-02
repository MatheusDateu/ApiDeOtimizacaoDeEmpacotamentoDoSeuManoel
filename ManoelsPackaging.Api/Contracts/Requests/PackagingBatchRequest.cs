using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ManoelsPackaging.Api.Contracts.Requests
{
    /// <summary>
    /// Represents a batch of orders to be processed for packaging.
    /// This is the root object for the packaging request.
    /// </summary>
    public class PackagingBatchRequest
    {
        /// <summary>
        /// Gets or sets the list of orders to be processed.
        /// Must contain at least one order.
        /// </summary>
        [JsonPropertyName("pedidos")]
        [Required(ErrorMessage = "Orders list is required.")]
        [MinLength(1, ErrorMessage = "Request must contain at least one order.")]
        public List<OrderRequestDto> Orders { get; set; } = new List<OrderRequestDto>();
    }
}
