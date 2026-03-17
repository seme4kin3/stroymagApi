using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Filters;
using Application;
using Supabase;


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

builder.Services.AddSingleton(provider =>
{
    var url = builder.Configuration["Supabase:Url"]!;
    var key = builder.Configuration["Supabase:ServiceRoleKey"]!;

    var options = new SupabaseOptions
    {
        AutoConnectRealtime = false
    };

    return new Client(url, key, options);
});

builder.Services.AddControllers().AddJsonOptions(_ => { });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
//builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ������ �� Excel 
//builder.Services.AddScoped<IExcelImportService, ExcelImportService>();

var app = builder.Build();

// Swagger 
app.UseSwagger();
app.UseSwaggerUI();

// ��������� �������� ��� ������ (�����������, �� ������)
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
