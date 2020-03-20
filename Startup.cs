using System.Globalization;
using System.Linq;
using System.Reflection;
using AuthorizationServer.Application.CommandHandlers;
using AuthorizationServer.Application.Events;
using AuthorizationServer.Application.Queries;
using AuthorizationServer.Application.Security;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.Services;
using AuthorizationServer.ExceptionHandling.Extensions;
using AuthorizationServer.Infrastructure;
using AuthorizationServer.Infrastructure.Context;
using AuthorizationServer.Infrastructure.IdentityServer;
using AuthorizationServer.Infrastructure.IdentityServer.Services;
using AuthorizationServer.Infrastructure.IdentityServer.Stores;
using AuthorizationServer.Infrastructure.Managers;
using AuthorizationServer.Infrastructure.Message;
using AuthorizationServer.Infrastructure.Repositories;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IntegrationEvents;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using Rebus.Config;
using Rebus.Logging;
using Rebus.MongoDb.Sagas;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.Sagas;
using Rebus.Serialization.Json;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;

namespace AuthorizationServer
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
   
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });


            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasPermissionHandler>();
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            var authority = Configuration.GetValue<string>("AuthenticationSettings:Authority");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.Authority = authority;
                o.Audience = Configuration.GetValue<string>("AuthenticationSettings:Audience");
                o.SaveToken = true;
                o.RequireHttpsMetadata = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    // dont check issuer because of wildcard domain
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("permissions:read", policy => policy.Requirements.Add(new HasPermissionRequirement("permissions:read", authority)));
                options.AddPolicy("permissions:write", policy => policy.Requirements.Add(new HasPermissionRequirement("permissions:write", authority)));

                options.AddPolicy("authentications:read", policy => policy.Requirements.Add(new HasPermissionRequirement("authentications:read", authority)));
                options.AddPolicy("authentications:write", policy => policy.Requirements.Add(new HasPermissionRequirement("authentications:write", authority)));

                options.AddPolicy("tenants:read", policy => policy.Requirements.Add(new HasPermissionRequirement("tenants:read", authority)));
                options.AddPolicy("tenants:write", policy => policy.Requirements.Add(new HasPermissionRequirement("tenants:write", authority)));

                options.AddPolicy("users:read", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read", authority)));
                options.AddPolicy("users:write", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write", authority)));

                options.AddPolicy("users:read", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read", authority)));
                options.AddPolicy("users:write", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write", authority)));

                options.AddPolicy("roles:read", policy => policy.Requirements.Add(new HasPermissionRequirement("roles:read", authority)));
                options.AddPolicy("roles:write", policy => policy.Requirements.Add(new HasPermissionRequirement("roles:write", authority)));

                options.AddPolicy("roles:read:root", policy => policy.Requirements.Add(new HasPermissionRequirement("roles:read:root", authority)));
                options.AddPolicy("users:write:root", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write:root", authority)));
                options.AddPolicy("users:read:root", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read:root", authority)));

                options.AddPolicy("users:write:tenant", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write:tenant", authority)));
                options.AddPolicy("users:read:tenant", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read:tenant", authority)));

                options.AddPolicy("users:write:user", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write:user", authority)));
                options.AddPolicy("users:read:user", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read:user", authority)));

                options.AddPolicy("application(tenants:read)", policy => policy.Requirements.Add(new HasScopeRequirement("tenants:read", authority)));
                options.AddPolicy("application(tenants:write)", policy => policy.Requirements.Add(new HasScopeRequirement("tenants:write", authority)));

                options.AddPolicy("application(users:read)", policy => policy.Requirements.Add(new HasScopeRequirement("users:read", authority)));
                options.AddPolicy("application(users:write)", policy => policy.Requirements.Add(new HasScopeRequirement("users:write", authority)));
            });


            //Application - Db Context
            services.Configure<ApplicationDbSettings>(
                Configuration.GetSection(nameof(ApplicationDbSettings)));

            services.AddSingleton<IApplicationDbSettings>(sp =>
                sp.GetRequiredService<IOptions<ApplicationDbSettings>>().Value);

            services.AddSingleton<ApplicationDbContext, ApplicationDbContext>();


            //Application - IdentityService to get current user from token
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IIdentityService, HttpRequestIdentityService>();

            // Application - Repositories
            services.AddScoped<IApiResourceRepository, ApiResourceRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IIdentityResourceRepository, IdentityResourceRepository>();
            services.AddScoped<IPersistedGrandRepository, PersistedGrantRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            // Application - Queries
            services.AddScoped<ApiQueries, ApiQueries>();
            services.AddScoped<ClientQueries, ClientQueries>();
            services.AddScoped<TenantQueries, TenantQueries>();
            services.AddScoped<UserQueries, UserQueries>();
            services.AddScoped<RoleQueries, RoleQueries>();
            services.AddScoped<PermissionQueries, PermissionQueries>();
            services.AddScoped<IdentityResourceQueries, IdentityResourceQueries>();

            // Application - Security
            services.AddScoped<SecurityService, SecurityService>();

            //Custom extension for mongo implementation
            services.AddIdentity<Domain.UserAggregate.IdentityUser, Domain.RoleAggregate.IdentityRole>(options =>
            {
                // For multi tenant
                options.User.RequireUniqueEmail = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserStore<UserRepository>()
            .AddRoleStore<RoleRepository>()
            .AddSignInManager<CustomSignInManager>()
            .AddDefaultTokenProviders();


            // Application - Commands
            services.AddMediatR(typeof(ApiHandlers).GetTypeInfo().Assembly);

            // Add email and sms services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");


            // Rebus - Integration event 
            services.AddRebus(configure => configure
                            .Serialization(s => s.UseNewtonsoftJson(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None }))
                            .Logging(l => l.Console())
                            .Transport(x => x.UseInMemoryTransport(new InMemNetwork(), "bus"))
                            .Subscriptions(x => x.StoreInMemory())
                            .Routing(r => r.TypeBased().MapAssemblyOf<IntegrationEvent>("bus"))
                        );

            // Identity sever configuration
            services.AddScoped<ICustomTokenRequestValidator, CustomClaimInjection>();

            //Identity server persistance
            services.AddTransient<IClientStore, ClientStore>();
            services.AddTransient<IResourceStore, ResourceStore>();
            services.AddTransient<ICorsPolicyService, CustomCorsPolicyService>();
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            var tokenCleanupOptions = new TokenCleanupOptions();
            services.AddSingleton(tokenCleanupOptions);
            services.AddSingleton<TokenCleanup>();

            services.AddScoped<IProfileService, CustomProfileService>();
            //Identity server
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
            })
            .AddDeveloperSigningCredential()
            .AddExtensionGrantValidator<AuthorizationServer.Infrastructure.IdentityServer.ExtensionGrantValidator>()
            .AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>()
            .AddJwtBearerClientAuthentication()
            .AddAspNetIdentity<AuthorizationServer.Domain.UserAggregate.IdentityUser>()
            .AddRedirectUriValidator<CustomRedirectUriValidator>()
            .AddProfileService<CustomProfileService>()
            .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();

            services.AddControllersWithViews()
           .AddViewLocalization(
               LanguageViewLocationExpanderFormat.Suffix,
               opts => { opts.ResourcesPath = "Resources"; });

            // API versionning
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });


            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true; 
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("fr")
            };

            //app.UseExceptiontMiddleware();


            app.UseRouting();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en")),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,

            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}");

            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
 