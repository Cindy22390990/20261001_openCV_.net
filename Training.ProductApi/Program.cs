using NLog;
using NLog.Web;
using Training.ProductApi.Repositories;
using Training.ProductApi.Services;


namespace Training.ProductApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var logger =
                NLog.LogManager
                .Setup()
                .LoadConfigurationFromFile("NLog.config")
                .GetCurrentClassLogger();


            try
            {

                var builder =
                    WebApplication.CreateBuilder(args);



                // NLog
                builder.Logging.ClearProviders();

                builder.Host.UseNLog();



                // Controller
                builder.Services.AddControllers();



                // Swagger
                builder.Services.AddEndpointsApiExplorer();

                builder.Services.AddSwaggerGen();



                // Service
                builder.Services.AddScoped<PythonImageProcessor>();

                builder.Services.AddScoped<ImageService>();



                // Repository
                builder.Services.AddScoped<ImageRepository>();



                var app =
                    builder.Build();



                if (app.Environment.IsDevelopment())
                {

                    app.UseSwagger();

                    app.UseSwaggerUI();

                }



                app.UseHttpsRedirection();


                app.UseAuthorization();


                app.MapControllers();


                app.Run();

            }
            catch (Exception ex)
            {

                logger.Error(ex);

                throw;

            }
            finally
            {

                NLog.LogManager.Shutdown();

            }

        }
    }
}