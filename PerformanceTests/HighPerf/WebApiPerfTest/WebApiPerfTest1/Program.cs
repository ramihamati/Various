using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

[ApiController]
[Route("/api/v1/[controller]")]
public class UsersController : ControllerBase{
    public List<User> GetUsers() {
        var list = new List<User>(1000);
        for (int index = 1; index < 1001; index++) {
            list.Add(new User {
                Id = index,
                Age = 25,
                First_Name = "First_Name" + index,
                Last_Name = "Last_Name" + index,
                Framework = "dotnet6 aaaaaaaa"
            });
        }

        return list;
    }

    [HttpGet(Name = "Users")]
    public IEnumerable<User> Get() {
        return GetUsers();
    }
}

public class User{
    public int Id { get; set; }
    public string First_Name { get; set; }
    public string Last_Name { get; set; }
    public int Age { get; set; }
    public string Framework { get; set; }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}