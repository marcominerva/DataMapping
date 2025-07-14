using DataMapping.BusinessLayer;
using DataMapping.DataAccessLayer;
using DataMapping.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalHelpers.OpenApi;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration.GetConnectionString("SqlConnection"));

builder.Services.AddScoped<PeopleService>();

builder.Services.AddDefaultProblemDetails();
builder.Services.AddDefaultExceptionHandler();

builder.Services.AddOpenApi(options =>
{
    options.RemoveServerList();
    options.AddDefaultProblemDetailsResponse();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", app.Environment.ApplicationName);
    });
}

app.MapGet("/people", async (PeopleService peopleService) =>
{
    var people = await peopleService.GetAsync();
    return TypedResults.Ok(people);
});

app.MapGet("/people/{id:guid}", async Task<Results<Ok<Person>, NotFound>> (Guid id, PeopleService peopleService) =>
{
    var person = await peopleService.GetAsync(id);
    if (person is null)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(person);
})
.ProducesDefaultProblem(StatusCodes.Status404NotFound);

app.Run();