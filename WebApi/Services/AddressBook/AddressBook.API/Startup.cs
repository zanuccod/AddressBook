using AddressBook.API.Models;
using AddressBook.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AddressBook.API
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

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "AddressBook.API" });
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddSingleton<IContactService, ContactService>();

            AddDataModel(services);
        }

        private void AddDataModel(IServiceCollection services)
        {
            var targetDatabase = Configuration.GetValue<string>("TargetDatabase");
            switch (targetDatabase)
            {
                case "SqlServer":
                    services.AddSingleton<IContactDataModel, ContactDataModelSQLServer>();
                    break;
                case "SqlLite":
                    services.AddSingleton<IContactDataModel, ContactDataModelSQLite>();
                    break;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AddressBook.API");
                });

                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
