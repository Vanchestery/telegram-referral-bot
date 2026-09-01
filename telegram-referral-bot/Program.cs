using ReferralBot.Extensions;
using ReferralBot.Middleware;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Referral Bot API";
        document.Info.Version = "v1";
        document.Info.Description = "Payment webhooks, promo codes, and partner cabinet.";
        return Task.CompletedTask;
    });
});
builder.Services.AddHealthChecks();

ContainerConfigurator.Configure(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
