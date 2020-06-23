using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Contracts;
using Inventory.Entities;
using Inventory.Repository;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;


namespace Inventory
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //Logger
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
           
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //start

              services
             .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.
                GetBytes(Configuration["JWT:key"]));
                 options.TokenValidationParameters = new
                TokenValidationParameters
                 {
                     IssuerSigningKey = serverSecret,
                     ValidIssuer = Configuration["JWT:Issuer"],
                     ValidAudience = Configuration["JWT:Audience"]
                 };
             });


            //end
            //logger
         
            services.AddControllers();
            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            //database connection

            //services.AddDbContext<InventoryDBContext>(option => option.UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog = InventoryDB;"));
            var connectionString = Configuration["sqlconnection:connectionString"];
            services.AddDbContext<InventoryDBContext>(option => option.UseSqlServer(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Logger
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, InventoryDBContext inventoryDBContext)
        {
            //start
            app.UseAuthentication();
            //end

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            //Logger
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            app.UseRouting();

            app.UseAuthorization();

            inventoryDBContext.Database.Migrate();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
