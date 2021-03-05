using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Deployments
{
    /// <summary>
    ///     Structure package part.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    ///     Disclaimer: Class is still under development and can be changed without notification and with breaking changes.
    /// </remarks>
    [Service(ServiceType = typeof(IStructurePackage<>), Lifetime = DependencyLifetime.Singleton)]
    public interface IStructurePackage<T>
    {
        /// <summary>
        ///     Exports the specified package info.
        /// </summary>
        /// <param name="packageInfo">The package info.</param>
        /// <returns></returns>
        T Export(PackageInfo packageInfo);

        /// <summary>
        ///     Imports the specified structure info.
        /// </summary>
        /// <param name="structureInfo">The structure info.</param>
        /// <param name="packageInfo">The package info.</param>
        void Import(StructureInfo structureInfo, PackageInfo packageInfo);

        /// <summary>
        ///     Prepares the import.
        /// </summary>
        /// <param name="structureInfo">The structure info.</param>
        /// <param name="packageInfo">The package info.</param>
        void PrepareImport(StructureInfo structureInfo, PackageInfo packageInfo);
    }
}
