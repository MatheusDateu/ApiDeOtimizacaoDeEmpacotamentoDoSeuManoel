using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ManoelsPackaging.Api.Contracts.Requests
{
    /// <summary>
    /// Represents the dimensions of an item.
    /// </summary>
    public class DimensionsDto
    {
        /// <summary>
        /// Gets or sets the height of the item in centimetres.
        /// Value must be greater than 0.
        /// </summary>
        [JsonPropertyName("altura")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Height must be greater than 0.")]
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the item in centimetres.
        /// Value must be greater than 0.
        /// </summary>
        [JsonPropertyName("largura")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Width must be greater than 0.")]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the length of the item in centimetres.
        /// Value must be greater than 0.
        /// </summary>
        [JsonPropertyName("comprimento")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Length must be greater than 0.")]
        public double Length { get; set; }
    }
}
