namespace Repositories.Factory
{
    /// <summary>
    /// Factory interface for creating repository instances.
    /// Eliminates the need to manually define repository properties in UnitOfWork.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Creates or retrieves a repository instance of the specified type.
        /// Repositories are cached per UnitOfWork instance.
        /// </summary>
        /// <typeparam name="TRepository">The repository interface type</typeparam>
        /// <returns>Repository instance</returns>
        TRepository GetRepository<TRepository>() where TRepository : class;

        /// <summary>
        /// Checks if a repository can be created for the specified type.
        /// </summary>
        /// <typeparam name="TRepository">The repository interface type</typeparam>
        /// <returns>True if repository can be created</returns>
        bool CanCreate<TRepository>() where TRepository : class;
    }
}
