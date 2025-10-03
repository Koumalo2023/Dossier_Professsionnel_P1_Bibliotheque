using api.Repositories;
using api.Services;
using api.Models; 

namespace api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void ConfigureDependencies(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Nouveau repository pour le profil utilisateur
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            // Services
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<AuthService>();
            
            // Nouveau service pour le profil utilisateur
            services.AddScoped<IUserProfileService, UserProfileService>();

            // AutoMapper - enregistrer le nouveau profil
            services.AddAutoMapper(typeof(UserProfileProfile));
        }
    }
}