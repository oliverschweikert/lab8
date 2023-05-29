using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(authOptions => {
    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwtOptions => {
    var signingKeyData = config["JwtSettings:SigningKey"];
    var signingKeyBytes = Encoding.UTF8.GetBytes(signingKeyData!);
    var signingKey = new SymmetricSecurityKey(signingKeyBytes);

    var issuer = config["JwtSettings:Issuer"];
    var audience = config["JwtSettings:Audience"];

    jwtOptions.TokenValidationParameters = new TokenValidationParameters{
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = signingKey
    };
});

builder.Services.AddAuthorization(authOptions => {
    authOptions.AddPolicy("IsAdmin", policyOptions => {
        policyOptions.RequireClaim("groups", "admin");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
