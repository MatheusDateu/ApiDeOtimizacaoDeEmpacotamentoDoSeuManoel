using ManoelsPackaging.Api.Contracts.Requests;
using ManoelsPackaging.Api.Core.Entities;
using ManoelsPackaging.Api.Core.Interfaces;
using ManoelsPackaging.Api.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManoelsPackaging.Core.UnitTests
{
    [TestClass]
    public sealed class PackagingOptimisationServiceTests
    {
        private Mock<IBoxRepository> _mockBoxRepository;
        private Mock<ILogger<PackagingOptimisationService>> _mockLogger;
        private PackagingOptimisationService _service;

        /// <summary>
        /// Initializes common mock objects and the service instance before each test.
        /// This method is called before each test method is run.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _mockBoxRepository = new Mock<IBoxRepository>();
            _mockLogger = new Mock<ILogger<PackagingOptimisationService>>();
            _service = new PackagingOptimisationService(_mockBoxRepository.Object, _mockLogger.Object);
        }

        /// <summary>
        /// Tests if a single product that fits into an available box is correctly packed into one box.
        /// </summary>
        [TestMethod]
        public async Task OptimisePackagingAsync_SingleProductFitsSingleBox_ShouldPackInOneBox()
        {
            /// <summary>
            /// Arrange
            /// </summary>
            var productDetails = new ProductDetailsDto
            {
                ProductId = "P1",
                Dimensions = new DimensionsDto { Height = 10, Width = 10, Length = 10 }
            };
            var orderRequest = new OrderRequestDto
            {
                OrderId = 1,
                Products = new List<ProductDetailsDto> { productDetails }
            };
            var batchRequest = new PackagingBatchRequest { Orders = new List<OrderRequestDto> { orderRequest } };

            var availableBoxes = new List<Box>
            {
                new Box("CaixaPequena", new Dimensions(15, 15, 15))
            };
            _mockBoxRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(availableBoxes);

            /// <summary>
            /// Act
            /// </summary>
            var result = await _service.OptimisePackagingAsync(batchRequest);

            /// <summary>
            /// Assert
            /// </summary>
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Orders.Count);
            var orderResult = result.Orders.First();
            Assert.AreEqual(1, orderResult.Boxes.Count);
            var boxAllocation = orderResult.Boxes.First();
            Assert.AreEqual("CaixaPequena", boxAllocation.BoxId);
            Assert.AreEqual(1, boxAllocation.ProductIds.Count);
            Assert.AreEqual("P1", boxAllocation.ProductIds.First());
            Assert.IsNull(boxAllocation.Observation);
        }

        /// <summary>
        /// Tests if a single product that is too large for any available box is correctly marked as unpackable.
        /// </summary>
        [TestMethod]
        public async Task OptimisePackagingAsync_SingleProductTooLarge_ShouldReturnUnpackableWithObservation()
        {
            /// <summary>
            /// Arrange
            /// </summary>
            var productDetails = new ProductDetailsDto
            {
                ProductId = "P-Grande",
                Dimensions = new DimensionsDto { Height = 100, Width = 100, Length = 100 }
            };
            var orderRequest = new OrderRequestDto
            {
                OrderId = 2,
                Products = new List<ProductDetailsDto> { productDetails }
            };
            var batchRequest = new PackagingBatchRequest { Orders = new List<OrderRequestDto> { orderRequest } };

            var availableBoxes = new List<Box>
            {
                new Box("CaixaPequena", new Dimensions(10, 10, 10))
            };
            _mockBoxRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(availableBoxes);

            /// <summary>
            /// Act
            /// </summary>
            var result = await _service.OptimisePackagingAsync(batchRequest);

            /// <summary>
            /// Assert
            /// </summary>
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Orders.Count);
            var orderResult = result.Orders.First();
            Assert.AreEqual(1, orderResult.Boxes.Count);
            var boxAllocation = orderResult.Boxes.First();
            Assert.IsNull(boxAllocation.BoxId);
            Assert.IsNotNull(boxAllocation.Observation);
            Assert.AreEqual("Produto não cabe em nenhuma caixa disponível.", boxAllocation.Observation);
            Assert.AreEqual(1, boxAllocation.ProductIds.Count);
            Assert.AreEqual("P-Grande", boxAllocation.ProductIds.First());
        }

        /// <summary>
        /// Tests if an order with no products results in no boxes allocated.
        /// </summary>
        [TestMethod]
        public async Task OptimisePackagingAsync_OrderWithNoProducts_ShouldReturnNoBoxes()
        {
            /// <summary>
            /// Arrange
            /// </summary>
            var orderRequest = new OrderRequestDto
            {
                OrderId = 3,
                Products = new List<ProductDetailsDto>() /// Lista de produtos vazia
            };
            var batchRequest = new PackagingBatchRequest { Orders = new List<OrderRequestDto> { orderRequest } };

            var availableBoxes = new List<Box>
            {
                new Box("CaixaMedia", new Dimensions(30, 30, 30))
            };
            _mockBoxRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(availableBoxes);

            /// <summary>
            /// Act
            /// </summary>
            var result = await _service.OptimisePackagingAsync(batchRequest);

            /// <summary>
            /// Assert
            /// </summary>
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Orders.Count);
            var orderResult = result.Orders.First();
            Assert.AreEqual(0, orderResult.Boxes.Count); /// Espera-se nenhuma caixa
        }

        /// <summary>
        /// Tests the behavior when no box types are available from the repository.
        /// </summary>
        [TestMethod]
        public async Task OptimisePackagingAsync_NoAvailableBoxes_ShouldReturnObservationForEachProduct()
        {
            /// <summary>
            /// Arrange
            /// </summary>
            var productDetails = new ProductDetailsDto
            {
                ProductId = "P2",
                Dimensions = new DimensionsDto { Height = 5, Width = 5, Length = 5 }
            };
            var orderRequest = new OrderRequestDto
            {
                OrderId = 4,
                Products = new List<ProductDetailsDto> { productDetails }
            };
            var batchRequest = new PackagingBatchRequest { Orders = new List<OrderRequestDto> { orderRequest } };

            var availableBoxes = new List<Box>(); /// Nenhuma caixa disponível
            _mockBoxRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(availableBoxes);

            /// <summary>
            /// Act
            /// </summary>
            var result = await _service.OptimisePackagingAsync(batchRequest);

            /// <summary>
            /// Assert
            /// </summary>
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Orders.Count);
            var orderResult = result.Orders.First();
            Assert.AreEqual(1, orderResult.Boxes.Count);
            var boxAllocation = orderResult.Boxes.First();
            Assert.IsNull(boxAllocation.BoxId);
            Assert.IsNotNull(boxAllocation.Observation);
            Assert.AreEqual("Nenhum tipo de caixa disponível para empacotamento.", boxAllocation.Observation);
            Assert.IsTrue(boxAllocation.ProductIds.Contains("P2"));
        }

        /// <summary>
        /// Tests if two small products that should fit together in a small box are packed accordingly.
        /// This test depends heavily on the current packing heuristic.
        /// </summary>
        [TestMethod]
        public async Task OptimisePackagingAsync_TwoSmallProductsFitOneSmallBox_ShouldBePackedTogether()
        {
            /// <summary>
            /// Arrange
            /// </summary>
            var product1 = new ProductDetailsDto { ProductId = "P-S1", Dimensions = new DimensionsDto { Height = 5, Width = 5, Length = 5 } }; /// Volume 125
            var product2 = new ProductDetailsDto { ProductId = "P-S2", Dimensions = new DimensionsDto { Height = 4, Width = 4, Length = 4 } }; /// Volume 64
            var orderRequest = new OrderRequestDto { OrderId = 5, Products = new List<ProductDetailsDto> { product1, product2 } };
            var batchRequest = new PackagingBatchRequest { Orders = new List<OrderRequestDto> { orderRequest } };

            var availableBoxes = new List<Box>
            {
                new Box("CaixaP", new Dimensions(10, 10, 10)) /// Volume 1000. Deve caber os dois por volume.
            };
            _mockBoxRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(availableBoxes);

            /// <summary>
            /// Act
            /// </summary>
            var result = await _service.OptimisePackagingAsync(batchRequest);

            /// <summary>
            /// Assert
            /// </summary>
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Orders.Count);
            var orderResult = result.Orders.First();
            Assert.AreEqual(1, orderResult.Boxes.Count, "Should use only one box for these two small products.");
            var boxAllocation = orderResult.Boxes.First();
            Assert.AreEqual("CaixaP", boxAllocation.BoxId);
            Assert.AreEqual(2, boxAllocation.ProductIds.Count);
            CollectionAssert.AreEquivalent(new List<string> { "P-S1", "P-S2" }, boxAllocation.ProductIds.ToList());
        }
    }
}
