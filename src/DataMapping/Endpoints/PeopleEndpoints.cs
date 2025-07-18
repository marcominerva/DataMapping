
using DataMapping.BusinessLayer;
using DataMapping.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalHelpers.OpenApi;

namespace DataMapping.Endpoints;

public class PeopleEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/people", async (PeopleService peopleService) =>
        {
            var people = await peopleService.GetAsync();
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
    }
}
