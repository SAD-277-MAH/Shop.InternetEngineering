using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shop.Common.Extentions;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Repo.Infrastructure;
using Shop.Services.Seed.Service;
using Shop.Services.Site.Interface;
using Shop.Services.Site.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation
{
    public class Startup
    {
        public IConfiguration _configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddAuthorization(option =>
            {
                option.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                option.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
                option.AddPolicy("RequireBloggerAdminRole", policy => policy.RequireRole("BloggerAdmin"));
                option.AddPolicy("RequireBloggerRole", policy => policy.RequireRole("Blogger"));
                option.AddPolicy("RequireAccountantRole", policy => policy.RequireRole("Accountant"));

                option.AddPolicy("Access", policy => policy.RequireRole("Admin", "User", "BloggerAdmin", "Blogger", "Accountant"));
            });

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<User, Role>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders().AddErrorDescriber<PersianIdentityErrorDescriber>();

            var mapperConfig = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapperProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IUnitOfWork<DatabaseContext>, UnitOfWork<DatabaseContext>>();
            services.AddTransient<SeedService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedService seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            seeder.SeedRoles();
            seeder.SeedUsers();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
