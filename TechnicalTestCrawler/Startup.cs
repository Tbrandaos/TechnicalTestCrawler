using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalTestCrawler.Domain.Dto;
using TechnicalTestCrawler.Domain.Services;
using TechnicalTestCrawler.Infra.Context;
using TechnicalTestCrawler.Infra.Entities;
using TechnicalTestCrawler.Infra.Services;

namespace TechnicalTestCrawler
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
            services.AddDbContext<DataContext>(
                context => context.UseNpgsql(Configuration.GetConnectionString("Default"))
            );


            services.AddRazorPages();

            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Technical Test Crawler API", Version = "v1" });
            });

            var config = new MapperConfiguration(cfg =>
            {
                #region Product
                cfg.CreateMap<ProductDto, Product>().ReverseMap();
                #endregion
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.AddHttpClient();

            services.AddScoped<ICrawlerService, CrawlerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Technical Test Crawler  v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            app.UseCors(c => c.AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowAnyOrigin()

            );
        }
    }
}
