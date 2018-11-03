using Sitecore.Support.XConnect.Web;

namespace Sitecore.Support.XConnect.Web
{
  using Microsoft.Extensions.DependencyInjection;
  using Sitecore.XConnect.DependencyInjection.Abstractions;
  using System;

  [CLSCompliant(false)]
  public class XConnectServicesConfiguration : IXConnectServicesConfiguration
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.UseXConnectService();
    }
  }
}