using ManoelsPackaging.Api.Core.Entities;

namespace ManoelsPackaging.Api.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for accessing Box entity data.
    /// </summary>
    public interface IBoxRepository
    {
        /// <summary>
        /// Retrieves all available box types with their dimensions.
        /// </summary>
        /// <returns>A read-only list of all Box entities including their Dimensions.</returns>
        Task<IReadOnlyList<Box>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific box type by its identifier, including its dimensions.
        /// </summary>
        /// <param name="typeIdentifier">The type identifier of the box.</param>
        /// <returns>The Box entity if found; otherwise, null.</returns>
        Task<Box?> GetByIdentifierAsync(string typeIdentifier);
    }
}
