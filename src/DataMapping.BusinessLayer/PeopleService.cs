using DataMapping.DataAccessLayer;
using DataMapping.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Entities = DataMapping.DataAccessLayer.Entities;

namespace DataMapping.BusinessLayer;

public class PeopleService(ApplicationDbContext dbContext)
{
    public async Task<IEnumerable<Person>> GetAsync()
    {
        var people = await dbContext.People.AsNoTracking()            
            //.Include(p => p.City)
            .OrderBy(p => p.FirstName).ThenBy(p => p.LastName)
            //.Select(p => new Person
            //{
            //    Id = p.Id,
            //    FirstName = p.FirstName,
            //    LastName = p.LastName,
            //    City = p.City != null ? p.City.Name : null
            //})
            //.Select(p => p.ToModel())
            .ToModel()
            .ToListAsync();

        return people;
    }

    public async Task<Person?> GetAsync(Guid id)
    {
        //var oldDbPerson = await dbContext.People.AsNoTracking()
        //    .Include(p => p.City)
        //    .FirstOrDefaultAsync(p => p.Id == id);

        var person = await dbContext.People.AsNoTracking()
            .Where(p => p.Id == id)
            .ToModel()
            .FirstOrDefaultAsync();

        return person;
    }
}

public static class PersonExtesions
{
    //public static Person ToModel(this Entities.Person person)
    //{
    //    return new()
    //    {
    //        Id = person.Id,
    //        FirstName = person.FirstName,
    //        LastName = person.LastName,
    //        City = person.City?.Name
    //    };
    //}

    public static IQueryable<Person> ToModel(this IQueryable<Entities.Person> people)
    {
        return people.Select(p => new Person
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            City = p.City != null ? p.City.Name : null
        });
    }
}