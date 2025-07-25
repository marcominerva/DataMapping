namespace DataMapping.Shared.Models;

public class AddPerson
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public Guid? CityId { get; set; }
}
