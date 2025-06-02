using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ManoelsPackaging.Api.Contracts.Requests
{
    /// <summary>
    /// Represents a single order containing multiple products.
    /// </summary>
    public class OrderRequestDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order.
        /// </summary>
        [JsonPropertyName("pedido_id")]
        // Assuming OrderId can be any integer. Add [Range] if there are specific constraints.
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the list of products included in the order.
        /// Must contain at least one product.
        /// </summary>
        [JsonPropertyName("produtos")]
        [Required(ErrorMessage = "Products list is required.")]
        [MinLength(1, ErrorMessage = "Order must contain at least one product.")]
        public List<ProductDetailsDto> Products { get; set; } = new List<ProductDetailsDto>();
    }
}
