
using ManoelsPackaging.Api.Contracts.Requests;
using ManoelsPackaging.Api.Contracts.Responses;
using ManoelsPackaging.Api.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ManoelsPackaging.Api.Controllers
{
    /// <summary>
    /// Handles requests related to calculating packaging solutions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PackagingController : ControllerBase
    {
        #region Properties
        private readonly IPackagingOptimisationService _packagingOptimisationService;
        #endregion

        #region Constructors
        public PackagingController(IPackagingOptimisationService packagingOptimisationService)
        {
            _packagingOptimisationService = packagingOptimisationService ?? throw new ArgumentNullException(nameof(packagingOptimisationService));
        }
        #endregion

        #region Endpoints

        /// <summary>
        /// Processes a batch of orders to determine the optimal packaging.
        /// </summary>
        /// <param name="batchRequest">The request containing a list of orders and their products.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains the packaging solution for each order, or an error if the request is invalid.</returns>
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculatePackaging([FromBody] PackagingBatchRequest batchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PackagingBatchResponse response = await _packagingOptimisationService.OptimisePackagingAsync(batchRequest);

            return Ok(response);
        }
        #endregion
    }
}