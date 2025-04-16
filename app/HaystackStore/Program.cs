using HaystackStore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IVolumeManger, VolumeManger>();
builder.Services.AddSingleton<IStoreService, StoreService>();
builder.Services.AddSingleton<INeedleCache, NeedleCache>();
builder.Services.AddSingleton<IFsStoreService, FsStoreService>();
builder.Services.AddSingleton<IFileWait, GaussianFileWait>();
builder.Services.AddSingleton<IFileRead, FileRead>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.GetService<IStoreService>();
app.Services.GetService<IFileWait>().Enable();

app.Run();