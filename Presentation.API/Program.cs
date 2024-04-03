using Application;
using Application.Common.Interfaces;
using Infra;
using Infra.Identity;
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

builder.Services
    .AddApplicationServices()
    .AddInfra(builder.Configuration);

builder.Services.AddCors();

builder.Services.AddControllers();

builder.Services.AddScoped<IUser, CurrentUser>();

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

app
    .MapGroup("/api/users")
    .MapIdentityApi<ApplicationUser>()
    .WithTags("Users");

app.MapControllers();

app.UseHttpsRedirection();
app.Run();