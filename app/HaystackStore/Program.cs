using HaystackStore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IVolumeManger, VolumeManger>();
builder.Services.AddSingleton<IStoreService, StoreService>();
builder.Services.AddSingleton<INeedleCache>(sp => new NeedleCache(1));

var app = builder.Build();

app.Run();
