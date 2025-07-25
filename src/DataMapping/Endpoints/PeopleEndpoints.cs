
using DataMapping.BusinessLayer;
using DataMapping.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalHelpers.OpenApi;

namespace DataMapping.Endpoints;

public class PeopleEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/people", async (PeopleService peopleService, int pageIndex = 0, int pageSize = 5, string orderBy = "FirstName") =>
        {
            var people = await peopleService.GetAsync(pageIndex, pageSize, orderBy);
            return TypedResults.Ok(people);
        });

        endpoints.MapGet("/api/people/{id:guid}", async Task<Results<Ok<Person>, NotFound>> (Guid id, PeopleService peopleService) =>
        {
            var person = await peopleService.GetAsync(id);
            if (person is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(person);
        })
        .ProducesDefaultProblem(StatusCodes.Status404NotFound);

        endpoints.MapPost("/api/people", async (AddPerson person, PeopleService peopleService) =>
        {
            var insertedPerson = await peopleService.AddAsync(person);
            return TypedResults.Created($"/api/people/{insertedPerson.Id}", insertedPerson);
        });
    }
}
