using System.ComponentModel.DataAnnotations.Schema;

namespace ManoelsPackaging.Api.Core.Entities
{
    /// <summary>
    /// Represents the physical dimensions of an item (height, width, length) as an entity.
    /// </summary>
    public class Dimensions
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets the height in centimetres.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets the width in centimetres.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets the length in centimetres.
        /// </summary>
        public double Length { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Parameterless constructor for EF Core.
        /// </summary>
        public Dimensions() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dimensions"/> struct.
        /// </summary>
        /// <param name="height">The height in centimetres. Must be a positive value.</param>
        /// <param name="width">The width in centimetres. Must be a positive value.</param>
        /// <param name="length">The length in centimetres. Must be a positive value.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if any dimension is not positive.</exception>
        public Dimensions(double height, double width, double length)
        {
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");

            Height = height;
            Width = width;
            Length = length;
        }
        #endregion

        #region Methods
        [NotMapped]
        /// <summary>
        /// Gets the calculated volume of the dimensions (Height * Width * Length).
        /// </summary>
        public double Volume => Height * Width * Length;
        #endregion
    }
}