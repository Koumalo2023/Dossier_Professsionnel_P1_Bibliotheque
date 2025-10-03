using api.Repositories;
using api.Services; 

namespace api.Configurations
{
    /// <summary>
    /// Configuration centralisée pour l'injection de dépendances des Services et Repository
    /// </summary>
    public static class AppInjectionConfig
    {
        /// <summary>
        /// Configure l'injection de dépendances pour tous les Services et Repository de l'application
        /// </summary>
        /// <param name="services">Collection des services</param>
        public static void ConfigureAppDependencies(this IServiceCollection services)
        {
            // Enregistrement des Repository
            ConfigureRepositories(services);
            
            // Enregistrement des Services
            ConfigureServices(services);
            
            // Enregistrement des clients HTTP
            ConfigureHttpClients(services);
        }

        /// <summary>
        /// Configure l'injection de dépendances pour les Repository
        /// </summary>
        /// <param name="services">Collection des services</param>
        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            
            // Ajouter ici les futurs Repository
            // services.AddScoped<ICategorieRepository, CategorieRepository>();
        }

        /// <summary>
        /// Configure l'injection de dépendances pour les Services
        /// </summary>
        /// <param name="services">Collection des services</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IGoogleBooksService, GoogleBooksService>();
            
            // Ajouter ici les futurs Services
            // services.AddScoped<ICategorieService, CategorieService>();
        }

        /// <summary>
        /// Configure les clients HTTP pour les services externes
        /// </summary>
        /// <param name="services">Collection des services</param>
        private static void ConfigureHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<IGoogleBooksService, GoogleBooksService>(client =>
            {
                client.BaseAddress = new Uri("https://www.googleapis.com/books/v1/");
                client.DefaultRequestHeaders.Add("User-Agent", "LibraryManagementSystem/1.0");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
        }
    }
}