﻿using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Models;
using SeeviceProvider_PL.Swagger;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Authentication;
using ServiceProvider_BLL.Authentication.Filters;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_BLL.Reposatories;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

namespace SeeviceProvider_PL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            // services.AddEndpointsApiExplorer();

            //services.AddSwaggerGen();


            //services.AddCors(options =>
            //        options.AddDefaultPolicy(builder =>
            //                builder.AllowAnyOrigin()
            //                       .AllowAnyMethod()
            //                       .AllowAnyHeader()

            //        )
            //);

            services.AddCors(options =>
                    options.AddDefaultPolicy(builder =>
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(_ => true) // للسماح بكل origins مع credentials
                    )
            );

            services
                .AddSwaggerServices()
                .AddMapsterConfiguration()
                .AddFluentValidationConfiguration()
                .AddAuthConfiguration(configuration);

            var connectionString = configuration.GetConnectionString("Default Connection") ??
             throw new InvalidOperationException("connection string 'Default Connection' not found.");

            services.AddDbContext<AppDbContext>(options =>
            options.UseLazyLoadingProxies().UseSqlServer(connectionString));



            //services.AddIdentity<Vendor, IdentityRole>()
            //    .AddEntityFrameworkStores<AppDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped<IAuthRepositry, AuthRepositry>();
            services.AddScoped<IAnalyticsRepositry, AnalyticsRepositry>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddExceptionHandler<GlobalExeptionHandler>();
            services.AddProblemDetails();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(
                        configuration["RabbitMQ:Host"],
                        configuration["RabbitMQ:VirtualHost"],
                        h =>
                        {
                            h.Username(configuration["RabbitMQ:Username"]!);
                            h.Password(configuration["RabbitMQ:Password"]!);
                        });

                    cfg.Message<NotificationMessage>(c =>
                    {
                        c.SetEntityName("NotificationMessage");
                    });

                    cfg.ConfigureJsonSerializerOptions(options =>
                    {
                        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                        return options;
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });



            return services;
        }

        private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }

        private static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
        {
            var mappingConfig = TypeAdapterConfig.GlobalSettings;

            mappingConfig.Scan(typeof(VendorRepository).Assembly);

            services.AddSingleton<IMapper>(new Mapper(mappingConfig));

            return services;
        }

        private static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(typeof(VendorRepository).Assembly);

            return services;
        }

        private static IServiceCollection AddAuthConfiguration(this IServiceCollection services , IConfiguration configuration) 
        {
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddIdentity<Vendor, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(o =>
           {
               o.SaveToken = true;
               o.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                   ValidIssuers = [jwtSettings?.Issuer, "CentralUserManagementService"],
                   ValidAudiences = [jwtSettings?.Audience , "CentralUserManagementService"]
               };
           });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;

            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOrApprovedVendor", policy =>
                policy.RequireRole("Vendor","Admin").RequireAssertion(context =>
                        context.User.Identity!.IsAuthenticated &&
                        context.User.HasClaim(c => c.Type == "IsApproved" && c.Value.Equals("true", StringComparison.OrdinalIgnoreCase))));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApprovedVendor", policy =>
                policy.RequireRole("Vendor").RequireAssertion(context =>
                        context.User.Identity!.IsAuthenticated &&
                        context.User.HasClaim(c => c.Type == "IsApproved" && c.Value.Equals("true", StringComparison.OrdinalIgnoreCase))));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOrMobileUserOrApprovedVendor", policy =>
                    policy.RequireRole("Admin", "MobileUser", "ApprovedVendor").RequireAssertion(context =>
                        context.User.Identity!.IsAuthenticated &&
                        context.User.HasClaim(c => c.Type == "IsApproved" && c.Value.Equals("true", StringComparison.OrdinalIgnoreCase))));

            });

            services.AddScoped<IUserClaimsPrincipalFactory<Vendor>, CustomUserClaimsPrincipalFactory>();

            return services;
        }
    }
}
