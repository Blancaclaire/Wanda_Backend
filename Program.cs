
using wandaAPI.Repositories;
using wandaAPI.Services;

var builder = WebApplication.CreateBuilder(args);

//Repositories

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

//Services

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("wandaDb"); //aqui lo ponemos de normal? 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

