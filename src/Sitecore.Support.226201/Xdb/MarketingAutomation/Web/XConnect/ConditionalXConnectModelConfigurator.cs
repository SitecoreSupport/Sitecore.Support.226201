using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Framework.Conditions;
using Sitecore.Support.XConnect.Web;
using Sitecore.XConnect.DependencyInjection.Abstractions;
using Sitecore.XConnect.Web.Infrastructure;

namespace Sitecore.Support.Xdb.MarketingAutomation.Web.XConnect
{
  public class ConditionalXConnectModelConfigurator : IXConnectServicesConfiguration
  {
    public void ConfigureServices(IServiceCollection services)
    {
      Condition.Requires<IServiceCollection>(services, "services").IsNotNull<IServiceCollection>();

      if (services.All(x=>x.ServiceType !=typeof(XdbEdmModel)))
      {
        services.UseXConnectModel();
      }
    }
  }
}
