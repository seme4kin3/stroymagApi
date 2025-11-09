using Infrastructure;
using Infrastructure.Import;
using Microsoft.EntityFrameworkCore;
using WebApi.Filters;
using Application;


var builder = WebApplication.CreateBuilder(args);

const string AllowLocalFrontend = "_allowLocalFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowLocalFrontend, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")     
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();                      
    });
});

builder.Services
    .AddApplication()                       
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(_ => { });
//builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Импорт из Excel 
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();

var app = builder.Build();

// Swagger 
app.UseSwagger();
app.UseSwaggerUI();

// Применяем миграции при старте (опционально, но удобно)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StroymagDbContext>();
    db.Database.Migrate();
}

app.UseCors(AllowLocalFrontend);

app.UseHttpsRedirection();

app.UseMiddleware<ValidationProblemDetailsMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
