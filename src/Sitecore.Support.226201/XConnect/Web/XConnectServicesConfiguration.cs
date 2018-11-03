using System;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.XConnect.DependencyInjection.Abstractions;

namespace Sitecore.Support.XConnect.Web
{
  [CLSCompliant(false)]
  public class XConnectServicesConfiguration : IXConnectServicesConfiguration
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.UseXConnectService();
    }
  }
}