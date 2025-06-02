using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ManoelsPackaging.Api.Contracts.Requests
{
    /// <summary>
    /// Represents the details of a product within an order.
    /// </summary>
    public class ProductDetailsDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product.
        /// This field is required.
        /// </summary>
        [JsonPropertyName("produto_id")]
        [Required(ErrorMessage = "Product ID is required.")]
        [MinLength(1, ErrorMessage = "Product ID cannot be empty.")]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the dimensions of the product.
        /// This field is required.
        /// </summary>
        [JsonPropertyName("dimensoes")]
        [Required(ErrorMessage = "Product dimensions are required.")]
        public DimensionsDto Dimensions { get; set; } = new(); // Initialise to prevent null if not provided, validation will catch it
    }
}
