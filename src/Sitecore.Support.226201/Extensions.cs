﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sitecore.XConnect.Diagnostics.Telemetry;
using Sitecore.XConnect.Schema;
using Sitecore.XConnect.Serialization;
using Sitecore.XConnect.Service;
using Sitecore.XConnect.Web.Infrastructure;
using Sitecore.XConnect.Web.Infrastructure.Operations;
using Sitecore.XConnect.Web.Infrastructure.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Sitecore.Support.XConnect.Web
{
  internal static class Extensions
  {
    private static FileSystemWatcher _modelFolderWatcher;

    public static void UseWebPerformanceCounters(this IServiceCollection services)
    {
      string path = HostingEnvironment.MapPath("~/App_Data/Diagnostics");
      string instanceName = HostingEnvironment.SiteName;
      services.TryAdd(ServiceDescriptor.Singleton<ICountersInstance>((Func<IServiceProvider, ICountersInstance>)(provider => (ICountersInstance)new CountersInstance(instanceName, path))));
      services.TryAdd(ServiceDescriptor.Singleton(typeof(ICountersInstance<>), typeof(CountersInstance<>)));
    }

    public static void UseXConnectService(this IServiceCollection services)
    {
      services.AddSingleton(typeof(IXdbOperationResultFormatter), (object)XdbOperationFormatterRegistry.CreateDefault());
      services.Add(ServiceDescriptor.Scoped(typeof(XdbCollectionService), typeof(XdbCollectionService)));
      services.Add(ServiceDescriptor.Scoped(typeof(OperationResponseMapper), typeof(OperationResponseMapper)));
      FileBasedXdbModelResolver xdbModelResolver = new FileBasedXdbModelResolver(HostingEnvironment.MapPath("~/App_Data/Models"));
      xdbModelResolver.TryAdd(XConnectCoreModel.Instance);
      xdbModelResolver.LoadModels();
      services.Add(ServiceDescriptor.Singleton(typeof(IXdbModelResolver), (object)xdbModelResolver));
      _modelFolderWatcher = new FileSystemWatcher(HostingEnvironment.MapPath("~/App_Data/Models"));
      _modelFolderWatcher.Changed += (FileSystemEventHandler)((sender, args) => HostingEnvironment.InitiateShutdown());
      _modelFolderWatcher.Renamed += (RenamedEventHandler)((sender, args) => HostingEnvironment.InitiateShutdown());
      _modelFolderWatcher.Created += (FileSystemEventHandler)((sender, args) => HostingEnvironment.InitiateShutdown());
      _modelFolderWatcher.Deleted += (FileSystemEventHandler)((sender, args) => HostingEnvironment.InitiateShutdown());
      _modelFolderWatcher.EnableRaisingEvents = true;
      XdbRuntimeModel runtimeModel = new XdbRuntimeModel(xdbModelResolver.KnownModels.ToArray<XdbModel>());
      services.Add(ServiceDescriptor.Singleton<XdbEdmModel>((Func<IServiceProvider, XdbEdmModel>)(p => XdbEdmModel.Create((XdbModel)runtimeModel, p.GetService<IMediaTypeFormatterConfiguration>()))));
      services.Add(ServiceDescriptor.Singleton<XdbModel>((Func<IServiceProvider, XdbModel>)(p => p.GetService<XdbEdmModel>().Model)));
      services.Add(ServiceDescriptor.Singleton<ExpandOptionsParser>((Func<IServiceProvider, ExpandOptionsParser>)(p => new ExpandOptionsParser(p.GetService<XdbEdmModel>()))));
    }
  }
}
