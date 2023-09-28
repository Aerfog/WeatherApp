var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("WeatherAPI", client =>
{
    client.BaseAddress = new Uri("https://api.weatherapi.com");
    // Set any required headers or authentication, if applicable
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();