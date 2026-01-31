
using wandaAPI.Repositories;
using wandaAPI.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("wandaDb"); 

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountUsersRepository, AccountUsersRepository>();
builder.Services.AddScoped<IObjectiveRepository, ObjectiveRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionSplitRepository, TransactionSplitRepository>();

//Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IObjectiveService, ObjectiveService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionSplitService, TransactionSplitService>();

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()    
                  .AllowAnyMethod()    
                  .AllowAnyHeader();   
        });
});
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
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
