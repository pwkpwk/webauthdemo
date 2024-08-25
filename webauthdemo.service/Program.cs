using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using webauthdemo.fscontrollers;
using webauthdemo.service.Authorization;
using webauthdemo.service.gRPC;
using webauthdemo.unggoy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcSwagger();

// Enable injection of IHttpContextAccessor into components of the ASP.NET HTTP pipeline.
builder.Services.AddHttpContextAccessor();
// Add 'Unggoy' authorization policy
builder.Services.AddUnggoyAuthorization();
// Also register the required Unggoy action verifier used by the authorization handler
// registered by AddUnggoyAuthorization
builder.Services.AddSingleton<IUnggoyActionVerifier, UnggoyActionVerifier>();

builder.Services.AddSingleton<IAuthorizationHandler, BackupAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, MainAuthorizationMiddlewareResultHandler>();
builder.Services.AddGrpc();

var mvcConfig = builder.Services.AddControllers();
mvcConfig.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(FunController).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use API controllers with routing attributes
app.UseRouting();

// UseAuthentication & UseAuthorization must be placed between UseRouting and MapControllers
// yo inject authorization middleware in the right order
app.UseAuthorization();

app.MapGrpcService<GrpcWeatherService>();
app.MapGrpcService<GrpcEchoService>();
// Discover controllers in all parts added above and map all routes to endpoints
app.MapControllers();

await app.RunAsync();
