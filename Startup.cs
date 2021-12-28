using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pweb_2021.Data;
using Pweb_2021.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pweb_2021
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
            string path = Path.Combine(Directory.GetCurrentDirectory(), "AppData");
            bool secureLogin = false;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                    .Replace("[DataDirectory]", path)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services
                .AddDefaultIdentity<ApplicationUser>(options =>
                {
                    if (secureLogin)
                    {
                        options.SignIn.RequireConfirmedAccount = true;
                    }
                    else
                    {
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequiredLength = 4;
                        options.Password.RequireDigit = false;
                    }
                })
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            //.AddRoles()
            //.AddUserManager<UserManager<ApplicationUser>>()
            //.AddDefaultTokenProviders()
            //.AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            CreateRoles(app.ApplicationServices);
        }
        private void CreateRoles(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                //initializing custom roles
                var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider
                    .GetService(typeof(RoleManager<IdentityRole>));
                var userManager = (UserManager<ApplicationUser>)scope.ServiceProvider
                    .GetService(typeof(UserManager<ApplicationUser>));
                string[] roleNames = { Statics.Roles.ADMIN, Statics.Roles.FUNCIONARIO };

                foreach (var roleName in roleNames)
                {
                    bool roleExist = roleManager.RoleExistsAsync(roleName).Result;
                    if (!roleExist)
                    {
                        //create the roles and seed them to the database: Question 1
                        roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
                    }
                }


                var usertowait = userManager.FindByEmailAsync(Statics.Root.mail);
                usertowait.Wait();
                ApplicationUser? _user = usertowait.Result;

                if (_user == null)
                {
                    //Here you could create a super user who will maintain the web app
                    var poweruser = new ApplicationUser
                    {
                        UserName = Statics.Root.user,
                        Email = Statics.Root.mail,
                    };

                    string userPWD = Statics.Root.password;

                    var poweruserwait = userManager.CreateAsync(poweruser, userPWD);
                    poweruserwait.Wait();
                    var createPowerUser = poweruserwait.Result;
                    if (createPowerUser.Succeeded)
                    {
                        //here we tie the new user to the role
                        userManager.AddToRoleAsync(poweruser, Statics.Roles.ADMIN).Wait();
                    }
                }
            }
        }
    }


}
