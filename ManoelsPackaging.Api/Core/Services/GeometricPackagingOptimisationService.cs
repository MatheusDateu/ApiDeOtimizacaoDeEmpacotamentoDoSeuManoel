using ManoelsPackaging.Api.Contracts.Requests;
using ManoelsPackaging.Api.Contracts.Responses;
using ManoelsPackaging.Api.Core.Entities;
using ManoelsPackaging.Api.Core.Interfaces;

namespace ManoelsPackaging.Api.Core.Services
{
    #region Spatial Models
    /// <summary>
    /// Represents a 3D bounding box / free spatial zone inside a container.
    /// </summary>
    public class SpatialSpace
    {
        #region Properties
        /// <summary>
        /// Gets or sets the X coordinate origin of the space.
        /// </summary>
        public double OriginX { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate origin of the space.
        /// </summary>
        public double OriginY { get; set; }

        /// <summary>
        /// Gets or sets the Z coordinate origin of the space.
        /// </summary>
        public double OriginZ { get; set; }

        /// <summary>
        /// Gets or sets the width dimension of the space.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height dimension of the space.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the length dimension of the space.
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// Gets the total volume of this spatial zone.
        /// </summary>
        public double Volume => Width * Height * Length;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialSpace"/> class.
        /// </summary>
        /// <param name="originX">The X coordinate origin.</param>
        /// <param name="originY">The Y coordinate origin.</param>
        /// <param name="originZ">The Z coordinate origin.</param>
        /// <param name="width">The width dimension.</param>
        /// <param name="height">The height dimension.</param>
        /// <param name="length">The length dimension.</param>
        public SpatialSpace(double originX, double originY, double originZ, double width, double height, double length)
        {
            OriginX = originX;
            OriginY = originY;
            OriginZ = originZ;
            Width = width;
            Height = height;
            Length = length;
        }
        #endregion
    }

    /// <summary>
    /// Represents a product successfully positioned inside a box with specific 3D coordinates.
    /// </summary>
    public class PlacedProduct
    {
        #region Properties
        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the original product entity.
        /// </summary>
        public Product OriginalProduct { get; set; } = null!;

        /// <summary>
        /// Gets or sets the coordinate X position.
        /// </summary>
        public double CoordinateX { get; set; }

        /// <summary>
        /// Gets or sets the coordinate Y position.
        /// </summary>
        public double CoordinateY { get; set; }

        /// <summary>
        /// Gets or sets the coordinate Z position.
        /// </summary>
        public double CoordinateZ { get; set; }

        /// <summary>
        /// Gets or sets the width of the product in its current orientation.
        /// </summary>
        public double OrientedWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the product in its current orientation.
        /// </summary>
        public double OrientedHeight { get; set; }

        /// <summary>
        /// Gets or sets the length of the product in its current orientation.
        /// </summary>
        public double OrientedLength { get; set; }
        #endregion
    }

    /// <summary>
    /// Represents an orientation candidate for 3D placement.
    /// </summary>
    public class ProductOrientation
    {
        #region Properties
        /// <summary>
        /// Gets or sets the oriented height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the oriented width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the oriented length.
        /// </summary>
        public double Length { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOrientation"/> class.
        /// </summary>
        /// <param name="height">The height dimension.</param>
        /// <param name="width">The width dimension.</param>
        /// <param name="length">The length dimension.</param>
        public ProductOrientation(double height, double width, double length)
        {
            Height = height;
            Width = width;
            Length = length;
        }
        #endregion
    }

    /// <summary>
    /// Engine responsible for 3D spatial packing simulation using 3D Guillotine algorithm.
    /// </summary>
    public class Spatial3DPackingEngine
    {
        #region Methods
        /// <summary>
        /// Simulates geometric packing of products into a candidate box type.
        /// </summary>
        /// <param name="targetBox">The candidate box target.</param>
        /// <param name="availableProducts">Products available to pack.</param>
        /// <param name="packedProducts">Out parameter with successfully packed products.</param>
        /// <returns>True if at least one product was packed; otherwise false.</returns>
        public bool TryPackBox(Box targetBox, List<Product> availableProducts, out List<PlacedProduct> packedProducts)
        {
            packedProducts = new List<PlacedProduct>();
            var freeSpaces = new List<SpatialSpace>
            {
                new SpatialSpace(0, 0, 0, targetBox.Dimensions.Width, targetBox.Dimensions.Height, targetBox.Dimensions.Length)
            };

            var remainingProducts = new List<Product>(availableProducts);

            foreach (var product in remainingProducts)
            {
                SpatialSpace? selectedSpace = null;
                ProductOrientation? selectedOrientation = null;
                int selectedSpaceIndex = -1;

                for (int i = 0; i < freeSpaces.Count; i++)
                {
                    var currentSpace = freeSpaces[i];
                    var possibleOrientations = GetProductOrientations(product);

                    foreach (var orientation in possibleOrientations)
                    {
                        if (orientation.Width <= currentSpace.Width &&
                            orientation.Height <= currentSpace.Height &&
                            orientation.Length <= currentSpace.Length)
                        {
                            selectedSpace = currentSpace;
                            selectedOrientation = orientation;
                            selectedSpaceIndex = i;
                            break;
                        }
                    }

                    if (selectedSpace != null)
                    {
                        break;
                    }
                }

                if (selectedSpace != null && selectedOrientation != null && selectedSpaceIndex >= 0)
                {
                    var placedProduct = new PlacedProduct
                    {
                        ProductId = product.ProductId,
                        OriginalProduct = product,
                        CoordinateX = selectedSpace.OriginX,
                        CoordinateY = selectedSpace.OriginY,
                        CoordinateZ = selectedSpace.OriginZ,
                        OrientedWidth = selectedOrientation.Width,
                        OrientedHeight = selectedOrientation.Height,
                        OrientedLength = selectedOrientation.Length
                    };

                    packedProducts.Add(placedProduct);
                    freeSpaces.RemoveAt(selectedSpaceIndex);

                    double rightWidth = selectedSpace.Width - selectedOrientation.Width;
                    if (rightWidth > 0)
                    {
                        freeSpaces.Add(new SpatialSpace(
                            selectedSpace.OriginX + selectedOrientation.Width,
                            selectedSpace.OriginY,
                            selectedSpace.OriginZ,
                            rightWidth,
                            selectedSpace.Height,
                            selectedOrientation.Length));
                    }

                    double topHeight = selectedSpace.Height - selectedOrientation.Height;
                    if (topHeight > 0)
                    {
                        freeSpaces.Add(new SpatialSpace(
                            selectedSpace.OriginX,
                            selectedSpace.OriginY + selectedOrientation.Height,
                            selectedSpace.OriginZ,
                            selectedSpace.Width,
                            topHeight,
                            selectedOrientation.Length));
                    }

                    double frontLength = selectedSpace.Length - selectedOrientation.Length;
                    if (frontLength > 0)
                    {
                        freeSpaces.Add(new SpatialSpace(
                            selectedSpace.OriginX,
                            selectedSpace.OriginY,
                            selectedSpace.OriginZ + selectedOrientation.Length,
                            selectedSpace.Width,
                            selectedSpace.Height,
                            frontLength));
                    }

                    freeSpaces = freeSpaces.OrderBy(s => s.Volume).ToList();
                }
            }

            return packedProducts.Any();
        }

        /// <summary>
        /// Gets all 6 3D spatial rotations of a product.
        /// </summary>
        /// <param name="product">The target product.</param>
        /// <returns>List of 6 spatial orientations.</returns>
        private List<ProductOrientation> GetProductOrientations(Product product)
        {
            double h = product.Dimensions.Height;
            double w = product.Dimensions.Width;
            double l = product.Dimensions.Length;

            return new List<ProductOrientation>
            {
                new ProductOrientation(h, w, l),
                new ProductOrientation(h, l, w),
                new ProductOrientation(w, h, l),
                new ProductOrientation(w, l, h),
                new ProductOrientation(l, h, w),
                new ProductOrientation(l, w, h)
            };
        }
        #endregion
    }
    #endregion // Spatial Models

    #region Service Implementation
    /// <summary>
    /// Implements advanced 3D spatial geometric optimization for order packaging.
    /// </summary>
    public class GeometricPackagingOptimisationService : IPackagingOptimisationService
    {
        #region Properties
        private readonly IBoxRepository _boxRepository;
        private readonly ILogger<GeometricPackagingOptimisationService> _logger;
        private readonly Spatial3DPackingEngine _packingEngine;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometricPackagingOptimisationService"/> class.
        /// </summary>
        /// <param name="boxRepository">The repository for accessing box data.</param>
        /// <param name="logger">The logger for logging service activity.</param>
        public GeometricPackagingOptimisationService(IBoxRepository boxRepository, ILogger<GeometricPackagingOptimisationService> logger)
        {
            _boxRepository = boxRepository;
            _logger = logger;
            _packingEngine = new Spatial3DPackingEngine();
        }
        #endregion

        #region Services
        /// <summary>
        /// Processes a batch of orders and determines the optimal 3D packaging solution for each.
        /// </summary>
        /// <param name="batchRequest">The batch of orders to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the packaging solutions.</returns>
        public async Task<PackagingBatchResponse> OptimisePackagingAsync(PackagingBatchRequest batchRequest)
        {
            _logger.LogInformation("Geometric packaging optimisation process started for {OrderCount} orders.", batchRequest.Orders?.Count ?? 0);

            IReadOnlyList<Box> availableBoxesFromDb = await _boxRepository.GetAllAsync();
            if (availableBoxesFromDb == null || !availableBoxesFromDb.Any())
            {
                _logger.LogWarning("No available box types found in the repository. Cannot proceed with packaging.");
                return CreateResponseForNoBoxesAvailable(batchRequest);
            }

            var packagingResults = new List<OrderPackagingResultDto>();
            if (batchRequest.Orders != null)
            {
                foreach (var orderRequest in batchRequest.Orders)
                {
                    _logger.LogInformation("Processing order ID: {OrderId}", orderRequest.OrderId);
                    List<BoxAllocationDto> allocatedBoxesForOrder = ProcessSingleOrder(orderRequest, availableBoxesFromDb);
                    packagingResults.Add(new OrderPackagingResultDto
                    {
                        OrderId = orderRequest.OrderId,
                        Boxes = allocatedBoxesForOrder
                    });
                }
            }

            _logger.LogInformation("Geometric packaging optimisation process completed.");
            return new PackagingBatchResponse { Orders = packagingResults };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a collection of <see cref="ProductDetailsDto"/> objects to a list of domain <see cref="Product"/> entities.
        /// </summary>
        /// <param name="productDtos">The collection of DTOs to convert.</param>
        /// <returns>A list of domain products.</returns>
        private List<Product> ConvertToDomainProducts(IEnumerable<ProductDetailsDto> productDtos)
        {
            var domainProducts = new List<Product>();
            if (productDtos != null)
            {
                foreach (var dto in productDtos)
                {
                    domainProducts.Add(new Product(dto.ProductId, new Core.Entities.Dimensions(dto.Dimensions.Height, dto.Dimensions.Width, dto.Dimensions.Length)));
                }
            }
            return domainProducts;
        }

        /// <summary>
        /// Processes a single order request using geometric 3D packing simulation.
        /// </summary>
        /// <param name="orderRequest">The order to process.</param>
        /// <param name="availableBoxTypes">The list of available box types.</param>
        /// <returns>A list of box allocations for the given order.</returns>
        private List<BoxAllocationDto> ProcessSingleOrder(OrderRequestDto orderRequest, IReadOnlyList<Box> availableBoxTypes)
        {
            var finalAllocationsForOrder = new List<BoxAllocationDto>();
            List<Product> productsToPack = ConvertToDomainProducts(orderRequest.Products)
                                            .OrderByDescending(p => p.Dimensions.Volume)
                                            .ToList();
            List<Product> unpackableProducts = new List<Product>();

            while (productsToPack.Any())
            {
                Product productToStartPotentialBoxWith = productsToPack.First();

                Box? bestBoxChoiceForThisRound = null;
                List<PlacedProduct> bestPlacedProductSetForThisRound = new List<PlacedProduct>();
                int maxItemsPackedInCandidateBox = 0;
                double maxVolumePackedInCandidateBox = 0;

                foreach (var candidateBoxType in availableBoxTypes.OrderBy(b => b.Dimensions.Volume))
                {
                    if (_packingEngine.TryPackBox(candidateBoxType, productsToPack, out List<PlacedProduct> packedCandidateList))
                    {
                        if (packedCandidateList.Any(p => p.ProductId == productToStartPotentialBoxWith.ProductId))
                        {
                            int itemsCount = packedCandidateList.Count;
                            double volumeSum = packedCandidateList.Sum(p => p.OriginalProduct.Dimensions.Volume);

                            if (itemsCount > maxItemsPackedInCandidateBox ||
                               (itemsCount == maxItemsPackedInCandidateBox && volumeSum > maxVolumePackedInCandidateBox))
                            {
                                maxItemsPackedInCandidateBox = itemsCount;
                                maxVolumePackedInCandidateBox = volumeSum;
                                bestPlacedProductSetForThisRound = packedCandidateList;
                                bestBoxChoiceForThisRound = candidateBoxType;
                            }
                        }
                    }
                }

                if (bestBoxChoiceForThisRound != null && bestPlacedProductSetForThisRound.Any())
                {
                    finalAllocationsForOrder.Add(new BoxAllocationDto
                    {
                        BoxId = bestBoxChoiceForThisRound.TypeIdentifier,
                        ProductIds = bestPlacedProductSetForThisRound.Select(p => p.ProductId).ToList()
                    });

                    foreach (var packedProduct in bestPlacedProductSetForThisRound)
                    {
                        productsToPack.RemoveAll(p => p.ProductId == packedProduct.ProductId);
                    }
                }
                else
                {
                    unpackableProducts.Add(productToStartPotentialBoxWith);
                    productsToPack.Remove(productToStartPotentialBoxWith);
                }
            }

            if (unpackableProducts.Any())
            {
                finalAllocationsForOrder.Add(new BoxAllocationDto
                {
                    BoxId = null,
                    ProductIds = unpackableProducts.Select(p => p.ProductId).ToList(),
                    Observation = unpackableProducts.Count == 1 ? "Produto não cabe em nenhuma caixa disponível." : "Um ou mais produtos não couberam em nenhuma caixa disponível."
                });
            }
            return finalAllocationsForOrder;
        }

        /// <summary>
        /// Creates a response when no boxes are available for packaging.
        /// </summary>
        /// <param name="batchRequest">The original batch request.</param>
        /// <returns>A response indicating no boxes are available.</returns>
        private PackagingBatchResponse CreateResponseForNoBoxesAvailable(PackagingBatchRequest batchRequest)
        {
            var results = new List<OrderPackagingResultDto>();
            if (batchRequest.Orders != null)
            {
                foreach (var orderRequest in batchRequest.Orders)
                {
                    results.Add(new OrderPackagingResultDto
                    {
                        OrderId = orderRequest.OrderId,
                        Boxes = new List<BoxAllocationDto>
                        {
                            new BoxAllocationDto
                            {
                                ProductIds = orderRequest.Products?.Select(p => p.ProductId).ToList() ?? new List<string>(),
                                Observation = "Nenhum tipo de caixa disponível para empacotamento."
                            }
                        }
                    });
                }
            }
            return new PackagingBatchResponse { Orders = results };
        }
        #endregion
    }
    #endregion
}