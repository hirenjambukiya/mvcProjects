using ex_APIarchitecture.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddProjectDependencies(builder.Configuration);

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
