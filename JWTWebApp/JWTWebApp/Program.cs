using JWTWebApp.Helper;
using JWTWebApp.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors();
// Configure AppSettings section from configuration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddControllers();

// Register services
builder.Services.AddScoped<IUserService, UserService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT API", Version = "v1" });
    // You can add authentication details here if necessary for Swagger.
});


builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowLocalhost", policy =>
        {
            policy.WithOrigins("http://localhost:5247")  // Allow your frontend's URL
                  .AllowAnyHeader()  // Allow any headers
                  .AllowAnyMethod()  // Allow any HTTP methods (GET, POST, etc.)
                  .AllowCredentials();  // Allow cookies and credentials to be sent
        });
    });



// Configure JWT Authentication
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddCookie(options =>
{
    options.Cookie.Name = "YourAppAuthCookie"; // Set cookie name
    options.Cookie.HttpOnly = true; // Security: ensures cookie is only sent in HTTP requests
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use secure cookies in production (set to Always if HTTPS is used)
    options.Cookie.SameSite = SameSiteMode.Strict; // Helps to prevent CSRF attacks
    options.SlidingExpiration = true; // Cookie expires after a set time but is renewed with activity
     
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };

    
});

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT API V1");
    });
}

app.UseCors("AllowLocalhost");


app.UseHttpsRedirection();
app.UseAuthentication();  // Make sure this comes before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
