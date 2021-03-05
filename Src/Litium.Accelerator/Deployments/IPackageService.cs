using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Deployments
{
    /// <summary>
    ///     Installation package service
    /// </summary>
    /// <remarks>
    ///     Disclaimer: Class is still under development and can be changed without notification and with breaking changes.
    /// </remarks>
    [Service(ServiceType = typeof(IPackageService), Lifetime = DependencyLifetime.Singleton)]
    public interface IPackageService
    {
        /// <summary>
        ///     Exports to structure info.
        /// </summary>
        /// <param name="packageInfo">The package info.</param>
        /// <returns></returns>
        StructureInfo Export(PackageInfo packageInfo);

        /// <summary>
        ///     Imports the specified structure info.
        /// </summary>
        /// <param name="structureInfo">The structure info.</param>
        /// <param name="packageInfo">The package info.</param>
        void Import(StructureInfo structureInfo, PackageInfo packageInfo);
    }
}
