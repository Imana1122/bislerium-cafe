using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.DataAccess
{
    /// <summary>
    /// Represents a factory for creating instances of a DbContext.
    /// </summary>
    /// <typeparam name="T">The type of DbContext to create.</typeparam>
    public class DbContextFactory<T> : IDbContextFactory<T> where T : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextFactory{T}"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for creating instances of services.</param>
        public DbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a new instance of the specified DbContext type.
        /// </summary>
        /// <returns>A new instance of the specified DbContext type.</returns>
        public T CreateDbContext()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
