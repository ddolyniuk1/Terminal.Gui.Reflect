using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui.Reflect.Interfaces;
using Terminal.Gui.Reflect.Services;

namespace Terminal.Gui.Reflect.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReflectServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                  .AddSingleton<IViewControllerFactory, ViewControllerControllerFactory>()
                  .AddSingleton<IThemeService, ThemeService>()
                  .AddSingleton<IIconService, IconService>();
        }
    }

}
