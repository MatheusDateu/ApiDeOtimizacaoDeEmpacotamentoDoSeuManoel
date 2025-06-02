using System.ComponentModel.DataAnnotations.Schema;

namespace ManoelsPackaging.Api.Core.Entities
{
    /// <summary>
    /// Represents a type of prefabricated box available for packaging, defined by its identifier and dimensions.
    /// Box definitions are considered immutable.
    /// </summary>
    public class Box
    {
        #region Properties
        /// <summary>
        /// Gets the identifier for the type of box (e.g., "Caixa 1").
        /// </summary>
        public string TypeIdentifier { get; init; }

        /// <summary>
        /// Gets the physical dimensions of the box.
        /// </summary>
        public Dimensions Dimensions { get; set; }
        /// <summary>
        /// Foreign key to the dimensions table.
        /// </summary>
        public int DimensionsId { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Parameterless constructor for EF Core.
        /// </summary>
        private Box()
        {
            TypeIdentifier = null!;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        /// <param name="typeIdentifier">The identifier for the box type. Cannot be null or empty.</param>
        /// <param name="dimensions">The physical dimensions of the box.</param>
        /// <exception cref="ArgumentNullException">Thrown if typeIdentifier is null.</exception>
        /// <exception cref="ArgumentException">Thrown if typeIdentifier is empty or whitespace.</exception>
        public Box(string typeIdentifier, Dimensions dimensions)
        {
            if (typeIdentifier == null)
                throw new ArgumentNullException(nameof(typeIdentifier));
            if (string.IsNullOrWhiteSpace(typeIdentifier))
                throw new ArgumentException("Box type identifier cannot be empty or whitespace.", nameof(typeIdentifier));

            TypeIdentifier = typeIdentifier;
            Dimensions = dimensions;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the calculated volume of the box.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public double Volume => Dimensions?.Volume ?? 0;
        #endregion
    }
}