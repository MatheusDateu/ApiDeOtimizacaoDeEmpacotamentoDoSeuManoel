using ManoelsPackaging.Api.Contracts.Requests;
using ManoelsPackaging.Api.Contracts.Responses;

namespace ManoelsPackaging.Api.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for the packaging optimisation service.
    /// </summary>
    public interface IPackagingOptimisationService
    {
        /// <summary>
        /// Processes a batch of orders and determines the optimal packaging solution for each.
        /// </summary>
        /// <param name="batchRequest">The batch of orders to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the packaging solutions.</returns>
        Task<PackagingBatchResponse> OptimisePackagingAsync(PackagingBatchRequest batchRequest);
    }
}