using System.Reflection;
using Litium.Owin.InversionOfControl;

namespace Litium.Accelerator.Deployments
{
	internal class IocInstaller : IComponentInstaller
	{
		public void Install(IIoCContainer container, Assembly[] assemblies)
		{
			container.For<IPackageService>().RegisterAsSingleton();
			//container.For<IPackage>().AsPlugin().RegisterAsSingleton();
			container.For(typeof (IStructurePackage<>)).RegisterAsSingleton();
		}
	}
}
