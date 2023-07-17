using MainData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worker_Event;
using Worker_Event.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<DatabaseContext>(options =>
        {
            var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, b =>
            {
                b.CommandTimeout(1200);
            });
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableDetailedErrors();
        });

        services.AddScoped<IEventService, EventService>();

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();