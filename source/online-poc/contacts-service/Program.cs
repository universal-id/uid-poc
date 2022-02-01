using ContactsService.Data;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region MongoDb
var connectionString = builder.Configuration["DatabaseSettings:MongoConnectionString"];
var databaseName = builder.Configuration["DatabaseSettings:DatabaseName"];
builder.Services.AddScoped(c => new MongoClient(connectionString).GetDatabase(databaseName));
builder.Services.AddScoped<IMongoDBContext, MongoDBContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
