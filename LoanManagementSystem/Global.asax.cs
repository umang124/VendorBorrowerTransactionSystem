using LoanManagementSystem.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Extensions.DependencyInjection;
using Telerivet.Client;
using LoanManagementSystem.Interface;
using LoanManagementSystem.Services;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using LoanManagementSystem.Repositories.RepositoryInterface;
using LoanManagementSystem.DataAccessLayer;

namespace LoanManagementSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var services = new ServiceCollection();
            ConfigureServices(services);
            var defaultResolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(defaultResolver);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton(typeof(ITransactionService), typeof(TransactionService));
            services.AddSingleton(typeof(IUserService), typeof(UserService));
            services.AddSingleton(typeof(ITelerivetService), typeof(TelerivetService));
            services.AddSingleton(typeof(IUserRepository), typeof(UserRepository));
            services.AddSingleton(typeof(ITransactionRepository), typeof(TransactionRepository));

            services.AddMvcControllers("*");

        }
    }

    
}
