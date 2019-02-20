using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApiAuth.Model;

namespace WebApiAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //缓存
            //services.AddMemoryCache(options =>
            //{
            //    options.CompactionPercentage = 0.02d;
            //    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
            //    options.SizeLimit = 1024;
            //});
            //Configs.ApiRequestExpireTime = int.Parse(Configuration.GetSection("ApiConfig")["ApiRequestExpireTime"]);
            //services.AddSession(option => {
            //    option.IdleTimeout = TimeSpan.FromMinutes(20);
            //});

            //string conn= Configuration.GetSection("RedisConfig")["Connection"];
            //redis缓存配置
            //RedisConfig.Connection = Configuration.GetSection("RedisConfig")["Connection"];
            //RedisConfig.DefaultDatabase = Convert.ToInt32(Configuration.GetSection("RedisConfig")["DefaultDatabase"]);
            //RedisConfig.InstanceName = Configuration.GetSection("RedisConfig")["InstanceName"];
            //services.AddSession(option => {
            //    option.IdleTimeout = TimeSpan.FromMinutes(20);
            //});

            //services.AddDbContext<EFContext>(options=>options.UseSqlServer(Configuration.GetConnectionString("conn")));//注入DbContext

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOptions();
            services.Configure<Configs>(Configuration.GetSection("ApiConfig")).AddMvc();
            services.AddMvc(option =>
            {
                //添加自己的授权验证
                option.Filters.Add(typeof(AuthAttribute));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
