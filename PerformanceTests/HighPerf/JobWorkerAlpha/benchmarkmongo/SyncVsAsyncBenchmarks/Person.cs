using MongoDB.Bson;

namespace benchmarks;

public class Person
{
    public required ObjectId Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}