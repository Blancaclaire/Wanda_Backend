
using wandaAPI.Repositories;
using wandaAPI.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("wandaDb"); 

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountUsersRepository, AccountUsersRepository>();
builder.Services.AddScoped<IObjectiveRepository, ObjectiveRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
//Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IObjectiveService, ObjectiveService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()    // Permite cualquier origen (Frontend)
                  .AllowAnyMethod()    // Permite GET, POST, PUT, DELETE, etc.
                  .AllowAnyHeader();   // Permite cualquier cabecera
        });
});
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
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

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
