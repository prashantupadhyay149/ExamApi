using ExamApi.Data;

var app = builder.Build();

// ✅ PORT (Render fix)
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Add($"http://0.0.0.0:{port}");

// ✅ AUTO MIGRATION
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();