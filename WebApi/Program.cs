using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Handler;
using WebApi.IHandler;
using WebApi.Middlewares;
using WebApi.Validators.TreeNodes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ExceptionHandlerMiddleware>();
builder.Services.AddScoped<IValidator<string>, CreateTreeValidator>();
builder.Services.AddScoped<ITreeHandler, TreeHandler>();
builder.Services.AddScoped<IJournalHandler, JournalHandler>();
builder.Services.AddScoped<ITreeNodeHandler, TreeNodeHandler>();

var connectionString = builder.Configuration.GetConnectionString("TestDatabase");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseExceptionHandlingMiddleware();

app.Run();
