
using AuthService.Services;
using Common;
using FileService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var Configuration = builder.Configuration;
            // Add services to the container.
            // In-memory services
            builder.Services.AddSingleton<InMemoryUserStore>();
            builder.Services.AddSingleton<IAuthService, AuthService.Services.AuthService>();

            builder.Services.AddSingleton<InMemoryFileStore>();
            builder.Services.AddSingleton<IFileService, FileService.Services.FileService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
     
            builder.Services.AddControllers();

            // Add Swagger for documentation
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Gateway",
                    Version = "v1"
                });
                // Add JWT Authentication to Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer prefix (Bearer {token})",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
            // HttpClient to route requests to different services
            builder.Services.AddHttpClient("AuthService", client => client.BaseAddress = new Uri("http://localhost:32773/"));
            builder.Services.AddHttpClient("FileService", client => client.BaseAddress = new Uri("http://localhost:32775/"));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // Enable Swagger
            // Enable Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1"));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Register endpoint
                endpoints.MapPost("/auth/register", async context =>
                {
                    // Read the request body as a JSON object
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    var registerData = JsonConvert.DeserializeObject<RegisterRequest>(requestBody);

                    if (registerData == null || string.IsNullOrEmpty(registerData.Username) || string.IsNullOrEmpty(registerData.Password))
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid request. Username and Password are required.");
                        return;
                    }

                    var authService = context.RequestServices.GetRequiredService<IAuthService>();
                    var result = await authService.RegisterUserAsync(registerData);

                    await context.Response.WriteAsync(result.Token);
                });
                // Endpoint for login (POST)
                endpoints.MapPost("/auth/login", async context =>
                {
                    // Read the request body as a JSON object
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    var loginData = JsonConvert.DeserializeObject<LoginRequest>(requestBody);

                    if (loginData == null || string.IsNullOrEmpty(loginData.Username) || string.IsNullOrEmpty(loginData.Password))
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid request. Username and Password are required.");
                        return;
                    }

                    var authService = context.RequestServices.GetRequiredService<IAuthService>();
                    var result = await authService.LoginUserAsync(loginData);

                    if (result == null || string.IsNullOrEmpty(result.Token))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid credentials.");
                    }
                    else
                    {
                        await context.Response.WriteAsync(result.Token); // Return the JWT token or appropriate response
                    }
                });
                endpoints.MapGet("/auth/verify", async context =>
                {
                    var authResult = await context.AuthenticateAsync();
                    if (!authResult.Succeeded || !authResult.Principal.Identity.IsAuthenticated)
                    {
                        context.Response.StatusCode = 401; // Unauthorized
                        await context.Response.WriteAsync("Invalid or missing authorization token.");
                        return;
                    }

                    // Successfully authenticated
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Token is valid.");
                });
                // Endpoint for file upload (POST)
                endpoints.MapPost("/files/upload", async context =>
                {
                    // Check if the request contains multipart data
                    if (!context.Request.HasFormContentType || !context.Request.Form.Files.Any())
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("No file uploaded.");
                        return;
                    }

                    var file = context.Request.Form.Files[0]; // Assume only one file for simplicity
                    var fileUploadService = context.RequestServices.GetRequiredService<IFileService>();

                    // Pass the IFormFile directly to the service method
                    var result = await fileUploadService.UploadFileAsync(file);

                    await context.Response.WriteAsync(result); // Return result of file upload
                });

                // file download endpoint
                endpoints.MapGet("/files/download/{fileName}", async context =>
                {
                    var fileService = context.RequestServices.GetRequiredService<IFileService>();

                    // Retrieve the file name from the route
                    var fileName = context.Request.RouteValues["fileName"]?.ToString();
                    if (string.IsNullOrEmpty(fileName))
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("File name is required.");
                        return;
                    }

                    try
                    {
                        // Call the DownloadFileAsync method from the IFileService
                        var fileContent = await fileService.DownloadFileAsync(fileName);
                        if (fileContent == null || fileContent.Length == 0)
                        {
                            context.Response.StatusCode = 404;
                            await context.Response.WriteAsync("File not found.");
                            return;
                        }

                        // Set the response headers for file download
                        context.Response.ContentType = "application/octet-stream";
                        context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");

                        await context.Response.Body.WriteAsync(fileContent);
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync($"An error occurred: {ex.Message}");
                    }
                });
            });

            app.Run();
        }
        
    }
}
