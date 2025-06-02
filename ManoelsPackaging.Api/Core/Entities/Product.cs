namespace ManoelsPackaging.Api.Core.Entities
{
    /// <summary>
    /// Represents a product to be packaged, including its identifier and dimensions.
    /// Product details are considered immutable once created.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets the unique identifier of the product.
        /// </summary>
        public string ProductId { get; }

        /// <summary>
        /// Gets the physical dimensions of the product.
        /// </summary>
        public Dimensions Dimensions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="productId">The unique identifier for the product. Cannot be null or empty.</param>
        /// <param name="dimensions">The physical dimensions of the product.</param>
        /// <exception cref="ArgumentNullException">Thrown if productId is null.</exception>
        /// <exception cref="ArgumentException">Thrown if productId is empty or whitespace.</exception>
        public Product(string productId, Dimensions dimensions)
        {
            if (productId == null)
                throw new ArgumentNullException(nameof(productId));
            if (string.IsNullOrWhiteSpace(productId))
                throw new ArgumentException("Product ID cannot be empty or whitespace.", nameof(productId));

            ProductId = productId;
            Dimensions = dimensions; // Dimensions struct is already validated
        }
    }
}
