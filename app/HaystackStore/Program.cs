using HaystackStore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IVolumeManger, VolumeManger>();
builder.Services.AddSingleton<IStoreService, StoreService>();
builder.Services.AddSingleton<INeedleCache>(sp => new NeedleCache(1));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
