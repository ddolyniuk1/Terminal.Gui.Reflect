using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui.Reflect.Interfaces;
using Terminal.Gui.Reflect.Services;

namespace Terminal.Gui.Reflect.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        extension(IServiceCollection serviceCollection)
        {
            internal IServiceCollection AddReflectServices()
            {
                return serviceCollection
                      .AddSingleton<IViewControllerFactory, ViewControllerControllerFactory>()
                      .AddSingleton<IThemeService, ThemeService>()
                      .AddSingleton<IIconService, IconService>();
            }
        }
    }

}
