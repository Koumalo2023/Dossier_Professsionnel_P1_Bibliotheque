using api.MappingProfiles;
using api.Models; 
using api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; 
using Serilog;
using System.Text;
using api.Configurations;
using api.Middleware;
using api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Configuration de Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuration centralisée de l'injection de dépendances
builder.Services.ConfigureDependencies();

builder.Services.AddHttpContextAccessor();

// Configuration Swagger centralisée
var swaggerConfig = SwaggerConfig.LoadFromConfiguration(builder.Configuration);
var swaggerValidation = swaggerConfig.Validate();
if (!swaggerValidation.IsValid)
{
    throw new InvalidOperationException($"Configuration Swagger invalide : {swaggerValidation.ErrorMessage}");
}

builder.Services.AddSingleton(swaggerConfig);
builder.Services.AddSwaggerGen(options =>
{
    swaggerConfig.ConfigureSwaggerGen(options);
});

// Configuration JWT centralisée
var jwtConfig = JwtConfig.LoadFromConfiguration(builder.Configuration);
builder.Services.AddSingleton(jwtConfig);

// Configurer l'authentification JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
        ClockSkew = TimeSpan.Zero // Pas de tolérance pour l'expiration
    };

    // Gestion des événements JWT
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});


//Configurer les cookies JWT
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddAuthorization();

// Configuration CORS professionnelle
var corsConfig = CorsConfig.LoadFromConfiguration(builder.Configuration);
var validationResult = corsConfig.Validate();
if (!validationResult.IsValid)
{
    throw new InvalidOperationException($"Configuration CORS invalide : {validationResult.ErrorMessage}");
}

builder.Services.AddSingleton(corsConfig);

// Configurer les politiques CORS pour développement et production
builder.Services.AddCors(options =>
{
    // Politique pour le développement
    options.AddPolicy(CorsConfig.DevelopmentPolicy,
        policy =>
        {
            policy.WithOrigins(corsConfig.DevelopmentOrigins.ToArray())
                  .WithMethods(corsConfig.AllowedMethods.ToArray())
                  .WithHeaders(corsConfig.AllowedHeaders.ToArray())
                  .WithExposedHeaders(corsConfig.ExposedHeaders.ToArray())
                  .AllowCredentials()
                  .SetPreflightMaxAge(TimeSpan.FromSeconds(corsConfig.PreflightCacheDuration));
        });

    // Politique pour la production
    options.AddPolicy(CorsConfig.ProductionPolicy,
        policy =>
        {
            policy.WithOrigins(corsConfig.ProductionOrigins.ToArray())
                  .WithMethods(corsConfig.AllowedMethods.ToArray())
                  .WithHeaders(corsConfig.AllowedHeaders.ToArray())
                  .WithExposedHeaders(corsConfig.ExposedHeaders.ToArray())
                  .AllowCredentials()
                  .SetPreflightMaxAge(TimeSpan.FromSeconds(corsConfig.PreflightCacheDuration));
        });
});

// Enregistrer le filtre d'exception global
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
});

// Configuration SeedUser centralisée
var seedConfig = SeedUserConfig.LoadFromConfiguration(builder.Configuration);
var seedValidation = seedConfig.Validate();
if (!seedValidation.IsValid)
{
    throw new InvalidOperationException($"Configuration SeedUser invalide : {seedValidation.ErrorMessage}");
}

builder.Services.AddSingleton(seedConfig);
builder.Services.AddScoped<SeedUserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API v1");
    });
}

// Initialisation des données de seed
using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<SeedUserService>();
    await seedService.InitializeAsync();
}

// Appliquer la politique CORS appropriée selon l'environnement
if (app.Environment.IsDevelopment())
{
    app.UseCors(CorsConfig.DevelopmentPolicy);
}
else
{
    app.UseCors(CorsConfig.ProductionPolicy);
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Ajouter le middleware de gestion des exceptions
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
