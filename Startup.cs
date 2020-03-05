using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IntegrationEvents;
using IntegrationEvents.Directory.Entries;
using IntegrationEvents.Directory.Invitation;
using IntegrationEvents.Directory.Profile;
using AuthorizationServer.Application.CommandHandlers;
using AuthorizationServer.Application.Events;
using AuthorizationServer.Application.Queries;
using AuthorizationServer.Application.Security;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.Services;
using AuthorizationServer.ExceptionHandling.Extensions;
using AuthorizationServer.Infrastructure.Context;
using AuthorizationServer.Infrastructure.IdentityServer;
using AuthorizationServer.Infrastructure.IdentityServer.Services;
using AuthorizationServer.Infrastructure.IdentityServer.Stores;
using AuthorizationServer.Infrastructure.Managers;
using AuthorizationServer.Infrastructure.Message;
using AuthorizationServer.Infrastructure.Repositories;
using AuthorizationServer.Infrastructure.SharedResources;
using AuthorizationServer.Infrastructure.Storage;
using AuthorizationServer.Extensions;
using AuthorizationServer.Resources;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using Rebus.Config;
using Rebus.Extensions;
using Rebus.Logging;
using Rebus.MongoDb.Sagas;
using Rebus.Routing.TypeBased;
using Rebus.Sagas;
using Rebus.Serialization.Json;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;
using Swashbuckle.AspNetCore.Swagger;
using Model = AuthorizationServer.Domain.UserAggregate;

namespace AuthorizationServer
{
    public class Startup
    {
	    private IMongoDatabase _mongoDb;
	    public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public ILoggerFactory LoggerFactory { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Environment = environment;
            LoggerFactory = loggerFactory;
        }

        private IMongoDatabase InstantiateMongoDatabase()
        {
	        var connectionString = Configuration.GetRequiredValue<string>("MONGO_URL");
	        var databaseName = MongoUrl.Create(connectionString).DatabaseName;
	        return new MongoClient(connectionString).GetDatabase(databaseName);
        }

        public void ConfigureServices(IServiceCollection services)
        {


            //Application - Db Context
            _mongoDb = InstantiateMongoDatabase();
            services.AddSingleton(_mongoDb);
            services.AddScoped<ApplicationDbContext, ApplicationDbContext>(cw => new ApplicationDbContext(cw.GetService<IMongoDatabase>()));

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasPermissionHandler>();
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            //Azure storage
            //services.AddSingleton<StorageConfiguration>(
            //    new StorageConfiguration(
            //        $"DefaultEndpointsProtocol=https;AccountName={Configuration.GetRequiredValue<string>("AZURE_STORAGE_PUBLIC_ACCOUNT_NAME")};AccountKey={Configuration.GetRequiredValue<string>("AZURE_STORAGE_PUBLIC_ACCOUNT_KEY")};EndpointSuffix=core.windows.net",
            //        $"DefaultEndpointsProtocol=https;AccountName={Configuration.GetRequiredValue<string>("AZURE_STORAGE_PRIVATE_ACCOUNT_NAME")};AccountKey={Configuration.GetRequiredValue<string>("AZURE_STORAGE_PRIVATE_ACCOUNT_KEY")};EndpointSuffix=core.windows.net"));
            //services.AddScoped<IStorageManager, AzureStorageManager>();

            // Application - Repositories
            services.AddScoped<IApiResourceRepository, ApiResourceRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IIdentityResourceRepository, IdentityResourceRepository>();
            services.AddScoped<IPersistedGrandRepository, PersistedGrantRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IAccountInviteRepository, AccountInviteRepository>();

            // Application - Commands
            services.AddMediatR(typeof(ApiHandlers).GetTypeInfo().Assembly);


            // Application - Queries
            services.AddScoped<ApiQueries, ApiQueries>();
            services.AddScoped<ClientQueries, ClientQueries>();
            services.AddScoped<TenantQueries, TenantQueries>();
            services.AddScoped<UserQueries, UserQueries>();
            services.AddScoped<RoleQueries, RoleQueries>();
            services.AddScoped<PermissionQueries, PermissionQueries>();
            services.AddScoped<IdentityResourceQueries, IdentityResourceQueries>();
            services.AddScoped<IInvitesQueries, InvitesQueries>();

            // Application - Security
            services.AddScoped<SecurityService, SecurityService>();

            //Custom extension for mongo implementation
            services.AddIdentity<Model.IdentityUser, AuthorizationServer.Domain.RoleAggregate.IdentityRole>(options => {
                // For multi tenant
                options.User.RequireUniqueEmail = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddDefaultTokenProviders();

            services.AddScoped<IUserStore<Model.IdentityUser>, UserRepository>();
            services.AddScoped<IRoleStore<Domain.RoleAggregate.IdentityRole>, RoleRepository>();
            services.AddScoped<IUserClaimsPrincipalFactory<Model.IdentityUser>, UserClaimsPrincipalFactory<Model.IdentityUser>>();

            services.AddScoped<UserManager<Model.IdentityUser>, UserManager<Model.IdentityUser>>();
            services.AddScoped<SignInManager<Model.IdentityUser>, CustomSignInManager>();


            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization(options => {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(SharedResource));
                    })
                    .AddJsonOptions(options =>
                    {
                        //use to get camelCase on ExpendoObject and dynamic object
                        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

                    });

            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });


            services.AddSwaggerGen(c => {

                c.DocInclusionPredicate((version, apiDescription) =>
                {
                    var values = apiDescription.RelativePath
                        .Split('/')
                        .Skip(1);

                    apiDescription.RelativePath = version + "/" + string.Join("/", values);

                    var versionParameter = apiDescription.ParameterDescriptions
                        .SingleOrDefault(p => p.Name == "version");

                    if (versionParameter != null)
                        apiDescription.ParameterDescriptions.Remove(versionParameter);

                    foreach (var parameter in apiDescription.ParameterDescriptions)
                        parameter.Name = char.ToLowerInvariant(parameter.Name[0]) + parameter.Name.Substring(1);

                    return true;
                });

                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "User API",
                    Contact = new Contact { Name = "Louis 21", Email = "aymeric.mortemousque@louis21.com", Url = "https://www.louis21.com" },
                });

                var sampleTenantAuthority = Configuration.GetRequiredValue<string>("JWT_AUTHORITY").Replace("://", "://sandbox.");
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = sampleTenantAuthority + "/connect/authorize",
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "openid" },
                        { "email", "email" },
                        { "profile" , "profile"},
                        { "role", "role"},
                        { "permission", "permission" },
                        { "tenant", "tenant" },
                        { "quarksupone_api", "quarksupone_api" }
                    }

                });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.CustomSchemaIds(t => t.FullName.Contains("`") ? t.FullName.Substring(0, t.FullName.IndexOf('`')) : t.FullName);

                c.AddSecurityRequirement(
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"oauth2", new string[] { }},
                        {"Bearer", new string[] { }}
                    }
                );

            });

            services.AddCors();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IApplicationContext, ApplicationContext>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetService<IActionContextAccessor>().ActionContext;
                actionContext.HttpContext.Request.PathBase = new PathString("/user");
                return new UrlHelper(actionContext);
            });
            services.AddScoped<ISharedResource, SharedResource>();
            services.AddScoped<ICustomTokenRequestValidator, CustomClaimInjection>();
            services.AddScoped(sp => (IRebusLoggerFactory) new ConsoleLoggerFactory(true));
            services.AddScoped<ISagaStorage, MongoDbSagaStorage>();


            //Identity server persistance
            services.AddTransient<IClientStore, ClientStore>();
            services.AddTransient<IResourceStore, ResourceStore>();
            services.AddTransient<ICorsPolicyService, CustomCorsPolicyService>();
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            var tokenCleanupOptions = new TokenCleanupOptions();
            services.AddSingleton(tokenCleanupOptions);
            services.AddSingleton<TokenCleanup>();

            //Identity server
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
            })
            .AddDeveloperSigningCredential()
            .AddExtensionGrantValidator<Infrastructure.IdentityServer.ExtensionGrantValidator>()
            .AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>()
            .AddJwtBearerClientAuthentication()
            .AddAspNetIdentity<Model.IdentityUser>()
            .AddRedirectUriValidator<CustomRedirectUriValidator>()
            .AddProfileService<CustomProfileService>()
            .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();
                    
            services.AddAuthorization(Configuration);

            //rebus configuration
            services.AutoRegisterHandlersFromAssemblyOf<ApiHandlers>();

            var destinationAddress = nameof(IntegrationEvents.Directory);
            // Configure and register Rebus
            services.AddRebus(configure => configure
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "Messages"))
                .Routing(r => r.TypeBased().MapAssemblyOf<IntegrationEvents.User.Account.AccountCreated>("Messages")));

            //services.AddRebus(configure => configure
            //    .Serialization(s => s.UseNewtonsoftJson(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None }))
            //    .Logging(l => l.Console())
            //    .Sagas(configurer => configurer.StoreInMongoDb(_mongoDb))
            //    .Transport(t => t.UseRabbitMq(Configuration.GetRequiredValue<string>("RABBITMQ_URL") + $"/{Configuration.GetRequiredValue<string>("HOST")}", nameof(IntegrationEvents.User)))
            //    .Routing(r =>
            //    {
            //        var builder = r.TypeBased();
            //        typeof(IntegrationEvent).Assembly.GetTypes()
            //            .Where(eventType => eventType.Namespace.Contains(destinationAddress))
            //            .ForEach(eventType => builder.Map(eventType, destinationAddress));
            //        builder.Map<UserCreated>(nameof(IntegrationEvents.User));
            //    })
            //);

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.UseRebus(bus =>
            {
	            bus.Subscribe<CandidateInvited>().Wait();
	            bus.Subscribe<RecruitInvited>().Wait();
	            bus.Subscribe<CandidateHired>().Wait();
	            bus.Subscribe<EmployeeOnboarded>().Wait();
	            bus.Subscribe<HRManagerOnboarded>().Wait();
	            bus.Subscribe<DirectorOnboarded>().Wait();
	            bus.Subscribe<EmployeePromotedToManager>().Wait();
	            bus.Subscribe<EmployeePromotedToHRManager>().Wait();
	            bus.Subscribe<EmployeePromotedToDirector>().Wait();
	            bus.Subscribe<EmployeeDemotedFromManager>().Wait();
	            bus.Subscribe<EmployeeDemotedFromHRManager>().Wait();
	            bus.Subscribe<EmployeeDemotedFromDirector>().Wait();
	            bus.Subscribe<SupportCollaboratorCreated>().Wait();
	            bus.Subscribe<CandidateCreated>().Wait();
	            bus.Subscribe<UserCreated>().Wait();
            });
		}

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors(
                options => options.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
             );

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("fr")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en")),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            var fordwardedHeaderOptions = new ForwardedHeadersOptions
            {
               ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };

            fordwardedHeaderOptions.KnownNetworks.Clear();
            fordwardedHeaderOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(fordwardedHeaderOptions);

            app.UsePathBase(new PathString("/user"));
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
            app.UseExceptiontMiddleware();
            app.UseSwagger(c => {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value + "/api/user/");
                c.RouteTemplate = "{documentName}/swagger.json";
            });

            app.UseRebus();
        }
    }

    public static class ServiceExtensions
    {
        public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration Configuration)
        {
            // configures the OpenIdConnect handlers to persist the state parameter into the server-side IDistributedCache.
            services.AddOidcStateDataFormatterCache("aad", "demoidsrv");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.Authority = Configuration.GetRequiredValue<string>("JWT_AUTHORITY");
                o.Audience = Configuration.GetRequiredValue<string>("JWT_AUDIENCE");
                o.SaveToken = true;
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
                options.AddPolicy("permissions:read", policy => policy.Requirements.Add(new HasPermissionRequirement("permissions:read", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("permissions:write", policy => policy.Requirements.Add(new HasPermissionRequirement("permissions:write", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("authentications:read", policy => policy.Requirements.Add(new HasPermissionRequirement("authentications:read", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("authentications:write", policy => policy.Requirements.Add(new HasPermissionRequirement("authentications:write", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("tenants:read", policy => policy.Requirements.Add(new HasPermissionRequirement("tenants:read", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("tenants:write", policy => policy.Requirements.Add(new HasPermissionRequirement("tenants:write", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("users:read", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("users:write", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("users:read", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("users:write", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("roles:read", policy => policy.Requirements.Add(new HasPermissionRequirement("roles:read", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("roles:write", policy => policy.Requirements.Add(new HasPermissionRequirement("roles:write", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("roles:read:root", policy => policy.Requirements.Add(new HasPermissionRequirement("roles:read:root", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("users:write:root", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write:root", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("users:read:root", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read:root", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("users:write:tenant", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write:tenant", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("users:read:tenant", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read:tenant", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("users:write:user", policy => policy.Requirements.Add(new HasPermissionRequirement("users:write:user", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("users:read:user", policy => policy.Requirements.Add(new HasPermissionRequirement("users:read:user", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("application(tenants:read)", policy => policy.Requirements.Add(new HasScopeRequirement("tenants:read", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("application(tenants:write)", policy => policy.Requirements.Add(new HasScopeRequirement("tenants:write", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));

                options.AddPolicy("application(users:read)", policy => policy.Requirements.Add(new HasScopeRequirement("users:read", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
                options.AddPolicy("application(users:write)", policy => policy.Requirements.Add(new HasScopeRequirement("users:write", Configuration.GetRequiredValue<string>("JWT_AUTHORITY"))));
            });

            return services;
        }
    }
}
