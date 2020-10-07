using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BinderApp.API.Data;
using BinderApp.API.Helpers;
using BinderApp.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BinderApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => options
                            .UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            ConfigureServices(services);
        }

         public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => options
                            .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Password configuration
            IdentityBuilder builder = services.AddIdentityCore<User>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            });

            //Creating an instance of Identity builder
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);

            //Configuring Identity
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            //Registering JWT Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        options.TokenValidationParameters = new TokenValidationParameters{
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                        };
                    });

            services.AddDbContext<DataContext>(options => options
                            .UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers(options => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddNewtonsoftJson(opt => 
                                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
                                
            services.AddCors();
            services.AddAutoMapper(typeof(DatingRepository).Assembly);

            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                        {
                            //Extension helper method to add cors headers
                            context.Response.AddApplicationError(error.Error.Message);
                            //write the error message into the response
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            //app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

            //Adding Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();
 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
