using Application;
using Application.Repositories;
using Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.IntegrationTests;

[TestClass]
public abstract class BaseIntegrationTest
{
    private static IServiceProvider _serviceProvider = null!;

    protected static ITenantRepository TenantRepository => GetService<ITenantRepository>();
    protected static ITagGroupRepository TagGroupRepository => GetService<ITagGroupRepository>();
    protected static ITagRepository TagRepository => GetService<ITagRepository>();


    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        var currentDirectory = Path.GetDirectoryName(typeof(BaseIntegrationTest).Assembly.Location)!;

        var appSettingsPath = Path.Combine(currentDirectory, "appsettings.json");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath)
            .Build();

        _serviceProvider = new ServiceCollection()
            .AddApplicationServices()
            .AddInfra(configuration)
            .BuildServiceProvider();
    }

    private static TService GetService<TService>()
        where TService : notnull
    {
        return _serviceProvider.GetRequiredService<TService>();
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        var context = GetService<AppDbContext>();

        context.Tenants.RemoveRange(context.Tenants);
        context.TagGroups.RemoveRange(context.TagGroups);
        context.Tags.RemoveRange(context.Tags);

        await context.SaveChangesAsync();
    }
}