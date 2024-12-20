using CPA_RoundRobin;
using CPA_RoundRobin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("RoundRobinClient", client =>
{
    // setting 3 second timeout
    client.Timeout = TimeSpan.FromSeconds(3);
});
builder.Services.AddHostedService<HealthCheckService>();
builder.Services.Configure<ApplicationSettings>(builder.Configuration);

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
