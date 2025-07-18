using DataMapping.BusinessLayer;
using DataMapping.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalHelpers.OpenApi;

namespace DataMapping.Endpoints;

public class CitiesEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/cities", async (CityService cityService) =>
        {
            var people = await cityService.GetAsync();
            return TypedResults.Ok(people);
        });

        endpoints.MapGet("/api/cities/{id:guid}", async Task<Results<Ok<City>, NotFound>> (Guid id, CityService cityService) =>
        {
            var city = await cityService.GetAsync(id);
            if (city is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(city);
        })
        .ProducesDefaultProblem(StatusCodes.Status404NotFound);

        endpoints.MapPost("/api/cities", async (City city, CityService cityService) =>
        {
            var insertedCity = await cityService.AddAsync(city);
            return TypedResults.Created($"/api/cities/{insertedCity.Id}", insertedCity);
        });

        endpoints.MapPut("/api/cities/{id:guid}", async (Guid id, UpdateCity city, CityService cityService) =>
        {
            await cityService.UpdateAsync(id, city);
            return TypedResults.NoContent();
        });

        endpoints.MapDelete("/api/cities/{id:guid}", async (Guid id, CityService cityService) =>
        {
            await cityService.DeleteAsync(id);
            return TypedResults.NoContent();
        });
    }
}
