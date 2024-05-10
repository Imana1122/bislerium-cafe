using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{
    /// <summary>
    /// Represents a factory for creating instances of a DbContext.
    /// </summary>
    /// <typeparam name="T">The type of DbContext to create.</typeparam>
    public interface IDbContextFactory<T> where T : DbContext
    {
        /// <summary>
        /// Creates a new instance of the specified DbContext type.
        /// </summary>
        /// <returns>A new instance of the specified DbContext type.</returns>
        T CreateDbContext();
    }
}
