using Basket.Data;
using Basket.Jobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

#region Quartz
//ser Service Job Scheduler
builder.Services.AddSingleton<IJobFactory, SingletonJobFaktory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();


// Add job Name for Service
builder.Services.AddSingleton<RemoveCartJob>();
// Set Schedule Time for "RemoveCartJob" Job 
builder.Services.AddSingleton(new JobScheduler(JobType:typeof(RemoveCartJob),CronExperssion:"0.5 * * * * ?"));

// start job
builder.Services.AddHostedService<QuartzHostdService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
