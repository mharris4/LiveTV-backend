using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace LiveTV
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static Configuration cfg;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string connectionString = "Data Source=dbinstance.ch2thbmh4kld.us-east-1.rds.amazonaws.com;Initial Catalog=XMLTV;Persist Security Info=True;User ID=michael;Password=********";

            cfg = new Configuration();

            cfg.DataBaseIntegration(x => {
                x.ConnectionString = connectionString;

                x.Driver<SqlClientDriver>();
                x.Dialect<MsSql2008Dialect>();
            });

            cfg.AddAssembly(Assembly.GetExecutingAssembly());

            
        }
    }
}
