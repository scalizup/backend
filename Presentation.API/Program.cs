using Application;
using Application.Common.Interfaces;
using Infra;
using Microsoft.AspNetCore.Mvc;
using Presentation.API.Middlewares;
using Presentation.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(t =>
    {
        if (t.FullName!.Contains("Command") || t.FullName!.Contains("Query") || t.FullName!.Contains("Dto"))
        {
            var rawName = t.FullName.Split("+");
            var lastPart = rawName[0].Split(".").Last();

            if (rawName[0].Contains("PageQueryResponse"))
            {
                return $"{lastPart}.Paginated";
            }

            return $"{lastPart}.{rawName[1]}";
        }

        return t.FullName;
    });

    options.SupportNonNullableReferenceTypes();
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services
    .AddApplicationServices()
    .AddInfra(builder.Configuration);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddCors();

builder.Services.AddControllers();

builder.Services.AddScoped<IUser, CurrentUser>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors(policyBuilder => policyBuilder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyHeader());

    app
        .UseSwagger()
        .UseSwaggerUI();
}

app.MapControllers();

app.UseExceptionHandler(options => { });

app.UseHttpsRedirection();
app.Run();