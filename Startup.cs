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
            services.AddControllersWithViews().AddControllersAsServices();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
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

            CreateRoles(serviceProvider).Wait();
        }
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var roleNames = Statics.Roles.getRoles();

            foreach (var roleName in roleNames)
            {
                bool roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var createDefaultAdmin = true;
            if (createDefaultAdmin)
            {
                ApplicationUser? _user = await userManager.FindByEmailAsync(Statics.Root.mail);
                if (_user == null)
                {
                    //Here you could create a super user who will maintain the web app
                    var poweruser = new ApplicationUser
                    {
                        UserName = Statics.Root.mail,
                        Email = Statics.Root.mail,
                    };

                    string userPWD = Statics.Root.password;

                    var createPowerUser = await userManager.CreateAsync(poweruser, userPWD);
                    if (createPowerUser.Succeeded)
                    {
                        //here we tie the new user to the role
                        var result_role = await userManager.AddToRoleAsync(poweruser, Statics.Roles.ADMIN);
                        if (!result_role.Succeeded)
                        {
                            System.Diagnostics.Debug.WriteLine("Error: adding role");
                        }
                        //confirm email
                        var code = await userManager.GenerateEmailConfirmationTokenAsync(poweruser);
                        var result_confirm = await userManager.ConfirmEmailAsync(poweruser, code);
                        if (!result_confirm.Succeeded)
                        {
                            System.Diagnostics.Debug.WriteLine("Error: adding confirmation");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Error: adding poweruser");
                }
            }
        }
    }
}
