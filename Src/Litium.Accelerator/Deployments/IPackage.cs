using Litium.Studio.Plugins.Deployments;

namespace Litium.Accelerator.Deployments
{
    /// <summary>
    ///     Package
    /// </summary>
    /// <remarks>
    ///     Disclaimer: Class is still under development and can be changed without notification and with breaking changes.
    /// </remarks>
    public interface IPackage : IDeploymentPackage
    {
        /// <summary>
        ///     Creates the web site.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns></returns>
        PackageInfo CreatePackageInfo(StructureInfo structureInfo, string name, string domainName);

        /// <summary>
        ///     Gets the structure info.
        /// </summary>
        /// <returns> </returns>
        StructureInfo GetStructureInfo();

        /// <summary>
        /// Get package storage path
        /// </summary>
        /// <returns></returns>
        string GetPackageStoragedPath();

        /// <summary>
        ///     Persists the structure information.
        /// </summary>
        /// <param name="structureInfo">The structure information.</param>
        void PersistStructureInfo(StructureInfo structureInfo);

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        string Type { get; set; }
    }
}
