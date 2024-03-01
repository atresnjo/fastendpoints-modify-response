using FastEndpoints;
using ModifyResponse;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFastEndpoints(config =>
{
    config.Endpoints.Configurator = definition => definition.PostProcessor<PostValidationProcessor>(Order.Before);
});

app.Run();