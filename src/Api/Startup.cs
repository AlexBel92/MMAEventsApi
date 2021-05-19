using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using MMAEvents.ApplicationCore.Interfaces;
using MMAEvents.ApplicationCore.Services;
using MMAEvents.Infrastructure.Data;
using MMAEvents.Infrastructure.Logging;
using MMAEvents.Infrastructure.Services;
using MMAEvents.Api.Services;
using System.Net.Http;
using AutoMapper;
using System;

namespace MMAEvents.Api
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
            services.AddHttpClient();

            services.Configure<MongoDbContextSettings>(Configuration.GetSection(MongoDbContextSettings.Position));
            services.AddSingleton<IMongoDbContextSettings>(sp => sp.GetRequiredService<IOptions<MongoDbContextSettings>>().Value);
            services.AddSingleton(typeof(MongoDbContext));

            services.AddScoped(typeof(IRepository<>), typeof(MongoDbRepository<>));

            services.AddScoped<EventsViewModelService>();
            services.AddScoped(typeof(IEventsViewModelService), typeof(CachedEventsViewModelService));

            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

            services.AddMemoryCache();

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddAutoMapper(typeof(EventsUpdateService).Assembly);

            services.AddResponseCaching();

            services.AddControllers(
                options =>
                {
                    options.CacheProfiles.Add("Default30",
                    new CacheProfile()
                    {
                        Duration = 30,
                        NoStore = false,
                        Location = ResponseCacheLocation.Any
                    });
                    options.CacheProfiles.Add("NoCache",
                    new CacheProfile()
                    {
                        Duration = 0,
                        NoStore = true,
                        Location = ResponseCacheLocation.None
                    });
                    options.CacheProfiles.Add("VaryByQueryKeys30",
                    new CacheProfile()
                    {
                        Duration = 30,
                        NoStore = false,
                        Location = ResponseCacheLocation.Client,
                        VaryByQueryKeys = new string[] { "EventName", "Date", "Quantity" }
                    });

                    options.RespectBrowserAcceptHeader = true;
                    options.ReturnHttpNotAcceptable = false;
                }
            ).AddXmlDataContractSerializerFormatters();

            services.AddScoped<ITelegramEventChangeClient, TelegramRestEventChangeClient>(
                sp => new TelegramRestEventChangeClient(sp.GetRequiredService<HttpClient>(),
                                                        sp.GetRequiredService<IMapper>(),
                                                        sp.GetRequiredService<IAppLogger<TelegramRestEventChangeClient>>(),
                                                        new Uri(
                                                            Configuration.GetSection("TelegramRestEventChangeClient:Uri").Value)));

            services.AddScoped<IEventsUpdateService, EventsUpdateService>();
            
            services.Configure<EventsParserOptions>(Configuration.GetSection(EventsParserOptions.Positin));
            services.AddSingleton<EventsParserOptions>(sp => sp.GetRequiredService<IOptions<EventsParserOptions>>().Value);
            services.AddHostedService<EventsParserHostedService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
                c.EnableAnnotations();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler("/error");

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
