using ManoelsPackaging.Api.Core.Entities;
using ManoelsPackaging.Api.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManoelsPackaging.Api.Infrastructure.Persistence.Repositories
{
    public class SqlBoxRepository : IBoxRepository
    {
        #region Properties
        private readonly AppDbContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBoxRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public SqlBoxRepository(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retrieves all available box types with their dimensions.
        /// </summary>
        /// <returns>A read-only list of all Box entities including their Dimensions.</returns>
        public async Task<IReadOnlyList<Box>> GetAllAsync()
        {
            return await _context.Boxes
                                 .Include(b => b.Dimensions)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific box type by its identifier, including its dimensions.
        /// </summary>
        /// <param name="typeIdentifier">The type identifier of the box.</param>
        /// <returns>The Box entity if found, including its Dimensions; otherwise, null.</returns>
        public async Task<Box?> GetByIdentifierAsync(string typeIdentifier)
        {
            return await _context.Boxes
                                 .Include(b => b.Dimensions)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(b => b.TypeIdentifier == typeIdentifier);
        }
        #endregion
    }
}