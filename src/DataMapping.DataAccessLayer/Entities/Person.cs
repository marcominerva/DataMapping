namespace DataMapping.DataAccessLayer.Entities;

public class Person
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public Guid? CityId { get; set; }

    public DateTime CreationDate { get; set; }

    public City? City { get; set; }
}
