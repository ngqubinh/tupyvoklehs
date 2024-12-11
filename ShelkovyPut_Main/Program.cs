using Domain.Models.Auth;
using Infrastructure.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.ShelkovyPutService(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true);
builder.Services.AddSignalR();

builder.Services.AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });

builder.Services.AddCors(options => 
{   
    options.AddPolicy("ForTauri", builder => 
        builder.WithOrigins("http://localhost:1420").AllowAnyHeader().AllowAnyMethod()); 
});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.UseSwagger();
app.UseSwaggerUI();
app.MapRazorPages();
app.UseCors("ForTauri");

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints => {
    endpoints.MapHub<ChatHub>("/chathub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});  
app.Run();
