using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimulaEmprestimo.Api.Models;

var builder = WebApplication.CreateBuilder(args);

var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
await keepAliveConnection.OpenAsync();

builder.Services.AddDbContext<ProdutoContexto>(options =>
    options.UseSqlite(keepAliveConnection));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( opcoes =>
{
    opcoes.SwaggerDoc("v1", new()
    {
        Title = "Simula Empréstimo API",
        Version = "v1",
        Description = "API gerada para o Desafio do sipsi.caixa/",
        Contact = new()
        {
            Name = "Júnior Almeida",
            Email = "jun10r4lm31d4@outlook.com",
            Url = new Uri("https://www.github.com/jun10r4lm31d4")
        }
    });

    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    opcoes.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProdutoContexto>();
    await context.Database.EnsureCreatedAsync();
}

await app.RunAsync();
