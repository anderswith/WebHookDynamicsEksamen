using WebHookDynamics.DynamicsFacade;
using WebHookDynamics.DynamicsFacade.Interfaces;
using WebHookDynamics.Services;
using WebHookDynamics.Services.Interfaces;

namespace WebHookDynamics;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IDynamicsWebhookLogic, DynamicsWebhookLogic>();
        
        

        builder.Services.AddSingleton<IEmail, Email>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var smtpServer = configuration["Email:SmtpServer"];
            var smtpPort = int.Parse(configuration["Email:SmtpPort"]);
            var smtpUser = configuration["Email:SmtpUser"];
            var smtpPass = configuration["Email:SmtpPass"];
            return new Email(smtpServer, smtpPort, smtpUser, smtpPass);
        }); 
        
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseAuthorization();
        app.UseDefaultFiles(); 
        app.UseStaticFiles();
        app.MapControllers();
        app.Run();
    }
}