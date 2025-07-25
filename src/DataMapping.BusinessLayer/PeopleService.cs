using System.Linq.Dynamic.Core;
using DataMapping.DataAccessLayer;
using DataMapping.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Entities = DataMapping.DataAccessLayer.Entities;

namespace DataMapping.BusinessLayer;

public class PeopleService(ApplicationDbContext dbContext)
{
    public async Task<IEnumerable<PersonListItem>> GetAsync(int pageIndex, int pageSize, string orderBy)
    {
        var query = dbContext.People.Include(p => p.City);
        var count = await query.CountAsync();

        var people = await query
            //.Include(p => p.City)
            .OrderBy(orderBy)
            .Skip(pageIndex * pageSize).Take(pageSize + 1)
            //.Select(p => new Person
            //{
            //    Id = p.Id,
            //    FirstName = p.FirstName,
            //    LastName = p.LastName,
            //    City = p.City != null ? p.City.Name : null
            //})
            //.Select(p => p.ToModel())
            .ToListModel()
            .ToListAsync();

        var hasNextItems = people.Count > pageSize;

        return people.Take(pageSize);
    }

    public async Task<Person?> GetAsync(Guid id)
    {
        //var oldDbPerson = await dbContext.People.AsNoTracking()
        //    .Include(p => p.City)
        //    .FirstOrDefaultAsync(p => p.Id == id);

        var person = await dbContext.People
            .Where(p => p.Id == id)
            .ToModel()
            .FirstOrDefaultAsync(); 

        return person;
    }

    public async Task<AddPerson> AddAsync(AddPerson person)
    {
        var cityExists = await dbContext.Cities.AnyAsync(c => c.Id == person.CityId);
        if (!cityExists)
        {
            throw new ArgumentException($"City with ID {person.CityId} does not exist.");
        }

        var entity = new Entities.Person
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            CityId = person.CityId,
            CreationDate = DateTime.UtcNow
        };

        dbContext.People.Add(entity);
        await dbContext.SaveChangesAsync();

        person.Id = entity.Id; // Ensure the ID is set after saving
        return person;
    }
}

public static class PersonExtensions
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

    public static IQueryable<PersonListItem> ToListModel(this IQueryable<Entities.Person> people)
    {
        return people.Select(p => new PersonListItem
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            City = p.City!.Name
        });
    }

    public static IQueryable<Person> ToModel(this IQueryable<Entities.Person> people)
    {
        return people.Select(p => new Person
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            City = p.City != null? new City
            {
                Id = p.City.Id,
                Name = p.City.Name
            } : null
        });
    }
}