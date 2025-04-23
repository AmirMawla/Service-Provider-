
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_BLL.Reposatories;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using SeeviceProvider_PL.Hubs;

namespace SeeviceProvider_PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Inject all services
            builder.Services.AddDependency(builder.Configuration);

            builder.Services.AddSignalR();

            var app = builder.Build();

            // Seed the database with initial data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    var userManager = services.GetRequiredService<UserManager<Vendor>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Await the Initialize method to ensure it completes before moving on
                    await SeedData.Initialize(context, userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }



            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseCors();

           // app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<MessageHub>("/hubs/message");

            app.UseExceptionHandler();

            app.Run();
        }
    }
}
