using DataMapping.DataAccessLayer;
using DataMapping.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Entities = DataMapping.DataAccessLayer.Entities;

namespace DataMapping.BusinessLayer;

public class CityService(ApplicationDbContext dbContext)
{
    public async Task<IEnumerable<City>> GetAsync()
    {
        var cities = await dbContext.Cities
            .OrderBy(p => p.Name)
            .ToModel()
            .ToListAsync();

        return cities;
    }

    public async Task<City?> GetAsync(Guid id)
    {
        var city = await dbContext.Cities
            .Where(p => p.Id == id)
            .ToModel()
            .FirstOrDefaultAsync();

        return city;
    }

    public async Task<City> AddAsync(City city)
    {
        var entity = new Entities.City
        {
            Id = city.Id,
            Name = city.Name
        };

        dbContext.Cities.Add(entity);
        await dbContext.SaveChangesAsync();

        city.Id = entity.Id; // Ensure the ID is set after saving
        return city;
    }

    public async Task UpdateAsync(Guid id, UpdateCity city)
    {
        var entity = await dbContext.Cities.FirstOrDefaultAsync(c => c.Id == id);

        if (entity is not null)
        {
            entity.Name = city.Name;
            await dbContext.SaveChangesAsync();
        }

        //var rowsAffected = await dbContext.Cities
        //    .Where(c => c.Id == id)
        //    .ExecuteUpdateAsync(c => c.SetProperty(c => c.Name, city.Name));
    }

    public async Task DeleteAsync(Guid id)
    {
        var city = await dbContext.Cities.FindAsync(id);
        if (city is not null)
        {
            dbContext.Cities.Remove(city);
            await dbContext.SaveChangesAsync();
        }

        //dbContext.Cities.Remove(new Entities.City { Id = id });
        //await dbContext.SaveChangesAsync();

        //await dbContext.Cities
        //    .Where(c => c.Id == id)
        //    .ExecuteDeleteAsync();
    }
}

public static class CityExtensions
{
    public static IQueryable<City> ToModel(this IQueryable<Entities.City> cities)
    {
        return cities.Select(c => new City
        {
            Id = c.Id,
            Name = c.Name
        });
    }
}