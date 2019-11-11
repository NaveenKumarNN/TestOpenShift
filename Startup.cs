
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ViewBooking.BusinessLogic;
using ViewBooking.Models;
using Webjet.Common.Diagnostics;
using Webjet.Common.Web.Core;

namespace ViewBooking
{
    public class Startup
    {
        private readonly Dictionary<string, List<Exception>> _exceptions;
      
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            _exceptions = StartupErrorHandler.InitDictionaryOfExceptions();
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.Configure<RouteOptions>(routeOptions => { routeOptions.AppendTrailingSlash = true; });
                services.AddOptions();
                services.Configure<ViewBookingOptions>(Configuration.GetSection("ViewBooking"));
              

                services.AddMvc().AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });

            }
            catch (Exception e)
            {
                _exceptions[StartupErrorHandler.ExceptionsOnConfigureServices].Add(e);
            }
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="options"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, Microsoft.Extensions.Options.IOptions<ViewBookingOptions> options)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger(); 
            loggerFactory.AddSerilog();
            var config = options.Value;
            try
            {
                //to log errors thrown by Startup() and ConfigureServices()
                if (StartupErrorHandler.WriteExceptionsToResponse(app, _exceptions, loggerFactory)) return;
                ErrorHandlingSetup(app, env);
                app.UseStaticFiles();
                app.UsePathBase(config.ApplicationRoot);
                app.UseStatusCodePages();
#if DEBUG
                app.UseCors("Default");
#endif
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Traveller}/{action=Get}/{id?}");
                });
                //app.UseMvcWithDefaultRoute();
   
               AssemblyInformation.SetMainAssembly(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                StartupErrorHandler.WriteExceptionToResponse(app, ex, loggerFactory);
            }
        }


        private static void ErrorHandlingSetup(IApplicationBuilder appBuilder, IHostingEnvironment env)
        {
            // returns Api error response
            //from https://www.devtrends.co.uk/blog/handling-errors-in-asp.net-core-web-api
            appBuilder.UseStatusCodePagesWithReExecute("/apierror/{0}");
            appBuilder.UseExceptionHandler("/apierror/500");
            //register other middleware that might return a non-success status code
            if (env.IsDevelopment())
            {
#if DEBUG
               // appBuilder.UseDeveloperExceptionPage();
#endif
                //Commenting this out to reduce the datawarehouse spam in the Fiddler window
                //app.UseBrowserLink();
            }
        }

 
    }
}