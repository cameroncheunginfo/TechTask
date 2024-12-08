
using DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Application.Addresses;
using Application.Customers;
using Application.Orders;
using Domain;
using Application.Orders.Validators;
using Application.Outbox;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
               .AddScoped<IOrderReader, OrderReader>()
               .AddScoped<IOrderCreator, OrderCreator>()
               .AddScoped<IOrderUpdater, OrderUpdater>()
               .AddScoped<ICustomerProvider, CustomerProvider>()
               .AddScoped<IAddressProvider, AddressProvider>()
               .AddTransient<ICreateOrderRequestValidator, CreateOrderRequestValidator>()
            ;

        builder.Services
               .AddScoped<DbContext, OrderDbContext>(config =>
                                                     {
                                                         var context = config.GetRequiredService<OrderDbContext>();
                                                         context.Database.EnsureCreated();
                                                         return context;
                                                     })
               .AddTransient<IUnitOfWork, UnitOfWork>()
               .AddTransient<IRepository<Order>, Repository<Order>>()
               .AddTransient<IRepository<Customer>, Repository<Customer>>()
               .AddTransient<IRepository<Address>, Repository<Address>>()
               .AddTransient<IRepository<Variant>, Repository<Variant>>()
            ;

        builder.Services
               .AddScoped<IRepository<OutboxMessage>, Repository<OutboxMessage>>()
               .AddScoped<IOutboxMessageCreator, OutboxMessageCreator>()
               .AddScoped<IOutboxMessageSender, OutboxMessageSender>()
            ;

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
                                       {
                                           options.ExampleFilters();
                                       });
        builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

        builder.Services.AddDbContext<OrderDbContext>(options =>
                                                      {
                                                          options.UseSqlite(CreateInMemoryDatabase());
                                                      });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }

    private static DbConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("DataSource=file:example.sqlite");
        
        connection.Open();

        return connection;
    }
}


