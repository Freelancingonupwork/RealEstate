using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealEstateDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RealEstate
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
            services.AddControllersWithViews();
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                //options.ExpireTimeSpan = TimeSpan.FromSeconds(Convert.ToDouble(3600));
                options.LoginPath = new PathString("/Account/Login");

            }).AddGoogle(options =>
            {
                options.ClientId = Configuration.GetSection("Google")["GoogleClientId"];
                options.ClientSecret = Configuration.GetSection("Google")["GoogleClientSecret"];

            }).AddMicrosoftAccount(o =>
            {
                o.CallbackPath = Configuration.GetSection("Microsoft")["MicrosoftCallbackPath"];
                o.ClientId = Configuration.GetSection("Microsoft")["MicrosoftClientId"];
                o.ClientSecret = Configuration.GetSection("Microsoft")["MicrosoftClientSecret"];
                o.Scope.Add("offline_access");
                o.Events = new OAuthEvents()
                {
                    OnRemoteFailure = HandleOnRemoteFailure
                };
            });

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddDbContext<RealEstateContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
        }

        private async Task HandleOnRemoteFailure(RemoteFailureContext context)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync("<html><head><link rel='preconnect' href='https://fonts.gstatic.com'><link href='https://fonts.googleapis.com/css2?family=Roboto:ital,wght@0,100;0,300;0,400;0,500;0,700;0,900;1,100;1,300;1,400;1,500;1,700;1,900&display=swap' rel ='stylesheet'><link rel='stylesheet' href='css/bootstrap.min.css'/><link rel='stylesheet' href='css/style.css'/></head><body><div class='login-wrapper'><div class='container'><div class='logo-wrapper'><img class='logo-img' src='image/logo.png' alt='' /></div>");
            await context.Response.WriteAsync("<div><div class='login-form mb-4' style=height:50% !important;><h4>You have denied the application permissions.<br> Please try again.</h4><br>");
            //await context.Response.WriteAsync("A remote failure has occurred: <br>" +
            //    context.Failure.Message.Split(Environment.NewLine).Select(s => HtmlEncoder.Default.Encode(s) + "<br>").Aggregate((s1, s2) => s1 + s2));

            //if (context.Properties != null)
            //{
            //    await context.Response.WriteAsync("Properties:<br>");
            //    foreach (var pair in context.Properties.Items)
            //    {
            //        await context.Response.WriteAsync($"-{ HtmlEncoder.Default.Encode(pair.Key)}={ HtmlEncoder.Default.Encode(pair.Value)}<br>");
            //    }
            //}

            await context.Response.WriteAsync("<h5><a href=\"/\">Home</a></h5></div></div></div>");
            await context.Response.WriteAsync("</body></html>");

            // context.Response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.Encode(context.Failure.Message));

            context.HandleResponse();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseCookiePolicy();
            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                MinimumSameSitePolicy = SameSiteMode.Lax
            });
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
