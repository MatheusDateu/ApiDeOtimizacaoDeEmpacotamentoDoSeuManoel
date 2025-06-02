using ManoelsPackaging.Api.Contracts.Requests;
using ManoelsPackaging.Api.Contracts.Responses;
using ManoelsPackaging.Api.Core.Entities;
using ManoelsPackaging.Api.Core.Interfaces;

namespace ManoelsPackaging.Api.Core.Services
{
    /// <summary>
    /// Implements the logic for optimising packaging solutions.
    /// </summary>
    public class PackagingOptimisationService : IPackagingOptimisationService
    {
        #region Properties
        private readonly IBoxRepository _boxRepository;
        private readonly ILogger<PackagingOptimisationService> _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PackagingOptimisationService"/> class.
        /// </summary>
        /// <param name="boxRepository">The repository for accessing box data.</param>
        /// <param name="logger">The logger for logging service activity.</param>
        public PackagingOptimisationService(IBoxRepository boxRepository, ILogger<PackagingOptimisationService> logger)
        {
            _boxRepository = boxRepository;
            _logger = logger;
        }
        #endregion

        #region Services
        /// <summary>
        /// Processes a batch of orders and determines the optimal packaging solution for each.
        /// </summary>
        /// <param name="batchRequest">The batch of orders to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the packaging solutions.</returns>
        public async Task<PackagingBatchResponse> OptimisePackagingAsync(PackagingBatchRequest batchRequest)
        {
            _logger.LogInformation("Packaging optimisation process started for {OrderCount} orders.", batchRequest.Orders?.Count ?? 0);

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

            _logger.LogInformation("Packaging optimisation process completed.");
            return new PackagingBatchResponse { Orders = packagingResults };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a collection of <see cref="ProductDetailsDto"/> objects to a list of domain <see cref="Product"/>
        /// entities.
        /// </summary>
        /// <param name="productDtos">The collection of <see cref="ProductDetailsDto"/> objects to convert. Can be null.</param>
        /// <returns>A list of <see cref="Product"/> entities. Returns an empty list if <paramref name="productDtos"/> is null.</returns>
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
        /// private method to process a single order request and allocate boxes for its products.
        /// This version attempts to find a best box by simulating packing for each suitable box type.
        /// </summary>
        /// <param name="orderRequest">The order to process.</param>
        /// <param name="availableBoxTypes">The list of available box types from the database.</param>
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
                List<Product> bestProductSetForThisRound = new List<Product>();
                int maxItemsPackedInCandidateBox = 0;
                double maxVolumePackedInCandidateBox = 0;

                foreach (var candidateBoxType in availableBoxTypes.OrderBy(b => b.Dimensions.Volume))
                {
                    if (CanProductFitInBox(productToStartPotentialBoxWith, candidateBoxType))
                    {
                        List<Product> currentTryProductsInBox = new List<Product> { productToStartPotentialBoxWith };
                        double candidateBoxVolumeCapacity = candidateBoxType.Dimensions.Volume;
                        double currentVolumeInThisTryBox = productToStartPotentialBoxWith.Dimensions.Volume;

                        List<Product> otherProductsToConsider = productsToPack
                            .Where(p => p.ProductId != productToStartPotentialBoxWith.ProductId)
                            .OrderBy(p => p.Dimensions.Volume)
                            .ToList();

                        foreach (var nextProductToTry in otherProductsToConsider)
                        {
                            if (CanProductFitInBox(nextProductToTry, candidateBoxType) &&
                                (currentVolumeInThisTryBox + nextProductToTry.Dimensions.Volume <= candidateBoxVolumeCapacity))
                            {
                                currentTryProductsInBox.Add(nextProductToTry);
                                currentVolumeInThisTryBox += nextProductToTry.Dimensions.Volume;
                            }
                        }

                        if (currentTryProductsInBox.Count > maxItemsPackedInCandidateBox)
                        {
                            maxItemsPackedInCandidateBox = currentTryProductsInBox.Count;
                            maxVolumePackedInCandidateBox = currentVolumeInThisTryBox;
                            bestProductSetForThisRound = new List<Product>(currentTryProductsInBox);
                            bestBoxChoiceForThisRound = candidateBoxType;
                        }
                        else if (currentTryProductsInBox.Count == maxItemsPackedInCandidateBox && currentVolumeInThisTryBox > maxVolumePackedInCandidateBox)
                        {
                            maxVolumePackedInCandidateBox = currentVolumeInThisTryBox;
                            bestProductSetForThisRound = new List<Product>(currentTryProductsInBox);
                            bestBoxChoiceForThisRound = candidateBoxType;
                        }
                    }
                }

                if (bestBoxChoiceForThisRound != null && bestProductSetForThisRound.Any())
                {
                    finalAllocationsForOrder.Add(new BoxAllocationDto
                    {
                        BoxId = bestBoxChoiceForThisRound.TypeIdentifier,
                        ProductIds = bestProductSetForThisRound.Select(p => p.ProductId).ToList()
                    });

                    foreach (var packedProduct in bestProductSetForThisRound)
                    {
                        productsToPack.Remove(packedProduct);
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
        /// Checks if a single product can fit into a given box type by testing 6 orientations.
        /// </summary>
        /// <param name="product">The product to check.</param>
        /// <param name="box">The box to check against.</param>
        /// <returns>True if the product fits in any orientation, otherwise false.</returns>
        private bool CanProductFitInBox(Product product, Box box)
        {
            double ph = product.Dimensions.Height;
            double pw = product.Dimensions.Width;
            double pl = product.Dimensions.Length;

            double bh = box.Dimensions.Height;
            double bw = box.Dimensions.Width;
            double bl = box.Dimensions.Length;

            /// <summary>
            /// Test all 6 permutations of product dimensions against fixed box dimensions.
            /// (h,w,l) vs (H,W,L)
            /// </summary>
            if ((ph <= bh && pw <= bw && pl <= bl) || (ph <= bh && pl <= bw && pw <= bl) ||
                (pw <= bh && ph <= bw && pl <= bl) || (pw <= bh && pl <= bw && ph <= bl) ||
                (pl <= bh && ph <= bw && pw <= bl) || (pl <= bh && pw <= bw && ph <= bl))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// <see langword="private"/> method to create a response when no boxes are available for packaging.
        /// </summary>
        /// <param name="batchRequest">The original batch request.</param>
        /// <returns>A <see cref="PackagingBatchResponse"/> indicating no boxes are available.</returns>
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
}