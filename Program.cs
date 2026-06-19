using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VetCare.clinicManagement.application;
using VetCare.clinicManagement.domain.repositories;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;
using VetCare.clinicManagement.infrastructure.persistence.EFC.repositories;
using VetCare.iam.application;
using VetCare.iam.domain.repositories;
using VetCare.iam.domain.services;
using VetCare.iam.infrastructure.persistence.EFC.context;
using VetCare.iam.infrastructure.persistence.EFC.repositories;
using VetCare.iam.infrastructure.security;
using VetCare.scheduling.application;
using VetCare.scheduling.domain.repositories;
using VetCare.scheduling.infrastructure.persistence.EFC.context;
using VetCare.scheduling.infrastructure.persistence.EFC.repositories;
using VetCare.backoffice.application;
using VetCare.backoffice.domain.repositories;
using VetCare.backoffice.infrastructure.persistence.EFC.context;
using VetCare.backoffice.infrastructure.persistence.EFC.repositories;
using VetCare.profile.application;
using VetCare.profile.domain.repositories;
using VetCare.profile.infrastructure.persistence.EFC.context;
using VetCare.profile.infrastructure.persistence.EFC.repositories;
using VetCare.communication.application;
using VetCare.communication.domain.repositories;
using VetCare.communication.infrastructure.persistence.EFC.context;
using VetCare.communication.infrastructure.persistence.EFC.repositories;
using VetCare.dashboard.application;
using VetCare.API.Middleware;
using VetCare.shared.persistence.EFC.extensions.eventDispatcher;
using VetCare.clinicManagement.infrastructure.persistence.EFC.seed;
using VetCare.API.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Configuration
var connectionString = DatabaseBootstrap.EnhanceConnectionString(
    builder.Configuration.GetConnectionString("DefaultConnection"));
var jwtSecret = builder.Configuration["JwtSettings:Secret"];

// 2. Add DbContexts (One per Bounded Context)

builder.Services.AddDbContext<IamContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<ClinicContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<SchedulingContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<BackofficeContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<ProfileContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<CommunicationContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Add Dependency Injection

// Event Dispatching (MediatR)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

// IAM
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<AuthService>();

// ClinicManagement
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();
builder.Services.AddScoped<IVaccineRecordRepository, VaccineRecordRepository>();
builder.Services.AddScoped<IHospitalizationAdmissionRepository, HospitalizationAdmissionRepository>();
builder.Services.AddScoped<IHospitalizationTaskRepository, HospitalizationTaskRepository>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<ConsultationService>();
builder.Services.AddScoped<VaccineService>();
builder.Services.AddScoped<HospitalizationService>();

// Scheduling
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<AppointmentService>();

// Backoffice
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IEntryRepository, EntryRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<EconomicsService>();
builder.Services.AddScoped<ProcurementService>();

// Profile
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<ProfileProductivityQueryService>();

// Communication
builder.Services.AddScoped<IDirectMessageRepository, DirectMessageRepository>();
builder.Services.AddScoped<DirectMessageCommandService>();

// Dashboard
builder.Services.AddScoped<DashboardQueryService>();

// 4. Add Authentication/JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

// 5. Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VetCare API", Version = "v1" });
    
    // Configure Swagger to use JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

var supportedCultures = new[] { "en", "en-US", "es", "es-PE" };
app.UseRequestLocalization(options =>
{
    options.SetDefaultCulture("en")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await DatabaseBootstrap.ApplyMigrationsAsync(app.Services);

app.Run();
