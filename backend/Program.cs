using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using backend.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AuthContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthContext") ?? throw new InvalidOperationException("Connection string 'AuthContext' not found.")));
var config = builder.Configuration;

builder.Services.AddScoped<IAuthService, AuthService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagOptions =>
{
  swagOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = @"JWT Authorization header using the Bearer scheme.<br/>
                      Enter your token in the text input below.<br/>
                      Example: '12345abcdef'",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT"
  });

  swagOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(authOptions =>
{
  authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwtOptions =>
{
  var signingKeyData = config["JwtSettings:SigningKey"];
  var signingKeyBytes = Encoding.UTF8.GetBytes(signingKeyData!);
  var signingKey = new SymmetricSecurityKey(signingKeyBytes);

  var issuer = config["JwtSettings:Issuer"];
  var audience = config["JwtSettings:Audience"];

  jwtOptions.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateAudience = true,
    ValidateIssuer = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = issuer,
    ValidAudience = audience,
    IssuerSigningKey = signingKey
  };
});

builder.Services.AddAuthorization(authOptions =>
{
  authOptions.AddPolicy("IsAdmin", policyOptions =>
  {
    policyOptions.RequireClaim("groups", "admin");
  });
});

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsOptions =>
corsOptions.AllowAnyHeader()
.AllowAnyMethod()
.WithOrigins("http://localhost:3000",
             "https://localhost:3001"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
