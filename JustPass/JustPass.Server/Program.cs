using JustPass.Server.Services;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);


var isInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";


if (isInContainer)
{
    builder.WebHost.UseUrls("http://*:80");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (isInContainer)
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(
                      "https://localhost:55625",
                      "http://localhost:55625",
                      "https://localhost:7227",
                      "http://localhost:7227"
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<IServer, Server>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JustPass.Server", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JustPass.Server V1");
    });
}

if (isInContainer)
{
    app.Use(async (context, next) =>
    {
        context.Request.Scheme = "http";
        await next();
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseStaticFiles();
app.MapControllers();

app.Run();