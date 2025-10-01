namespace Repositories.Attributes
{
    /// <summary>
    /// Marks a repository class for automatic registration in the RepositoryFactory.
    /// The factory will automatically discover and register repositories with this attribute.
    /// </summary>
    /// <example>
    /// [AutoRegisterRepository(typeof(IAuthRepository))]
    /// public class AuthRepository : BaseRepository&lt;LoginDto&gt;, IAuthRepository
    /// {
    ///     // Implementation
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoRegisterRepositoryAttribute : Attribute
    {
        /// <summary>
        /// The interface type that this repository implements (e.g., IAuthRepository)
        /// </summary>
        public Type InterfaceType { get; }

        public AutoRegisterRepositoryAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}
