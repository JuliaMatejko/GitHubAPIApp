using GitHubAPIApp.Services.Abstract;
using GitHubAPIApp.Services.Concrete;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;

namespace GitHubAPIApp
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
            services.AddControllers();

            //This was added manually!
            //Dependency injection
            services.AddHttpClient("App", config =>
            {
                var userAgentValue = new ProductInfoHeaderValue("GitHubAPIApp","v1");
                config.DefaultRequestHeaders.UserAgent.Add(userAgentValue);
            });
            services.AddTransient<IGitHubClient, GitHubClient>();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "GitHubAPIApp", Version = "v1" }); });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GitHubAPIApp v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
