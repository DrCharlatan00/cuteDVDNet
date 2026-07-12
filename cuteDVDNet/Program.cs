using cuteDVDCore.Services;
using cuteDVDCore.Services.Interfaces;
using cuteDVDNet.Mapping;
using cuteDVDNet.Middlewares;
using cuteDVDNet.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.




builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

#region DI
if (OperatingSystem.IsWindows())  
    builder.Services.AddScoped<IDriveFileService, WinDriveFileService>();

if (OperatingSystem.IsLinux())
    builder.Services.AddScoped<IDriveFileService, LinuxDriveFileService>();

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingFiles>();
});

builder.Services.AddScoped<INetDriveFileService, NetDriveFileService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<MiddlewareExceptioncs>();

#if DEBUG
app.UseMiddleware<InformationMiddleware>();
#endif

app.MapControllers();

app.MapGet("/", () => "live");

app.Run();
