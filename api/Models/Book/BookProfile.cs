using AutoMapper;

namespace api.Models
{
    /// <summary>
    /// Profile AutoMapper pour les mappings entre l'entité Book et ses DTOs.
    /// </summary>
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            // Mapping de Book vers BookDto
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AvailableCopies, opt => opt.MapFrom(src => src.AvailableCopies > 0 ? src.AvailableCopies : 0));

            // Mapping de CreateBookDto vers Book
            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore()) // Géré manuellement dans le service
                .ForMember(dest => dest.Category, opt => opt.Ignore()) // Géré manuellement dans le service
                .ForMember(dest => dest.AvailableCopies, opt => opt.MapFrom(src => src.TotalCopies))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Loans, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    // Nettoyage des chaînes de caractères
                    dest.Title = dest.Title?.Trim();
                    dest.Author = dest.Author?.Trim();
                    dest.Description = dest.Description?.Trim();
                    dest.Publisher = dest.Publisher?.Trim();
                    dest.Isbn = dest.Isbn?.Replace("-", "").Replace(" ", ""); // Normalisation ISBN
                    if (string.IsNullOrEmpty(dest.CoverUrl))
                        dest.CoverUrl = null;
                    if (string.IsNullOrEmpty(dest.Description))
                        dest.Description = null;
                    if (string.IsNullOrEmpty(dest.Publisher))
                        dest.Publisher = null;
                });

            // Mapping de UpdateBookDto vers Book
            CreateMap<UpdateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableCopies, opt => opt.Ignore()) // Calculé automatiquement
                .ForMember(dest => dest.TotalCopies, opt => opt.Ignore()) // Géré séparément pour la logique métier
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Loans, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    // Nettoyage des chaînes de caractères
                    dest.Title = dest.Title?.Trim();
                    dest.Author = dest.Author?.Trim();
                    dest.Description = dest.Description?.Trim();
                    dest.Publisher = dest.Publisher?.Trim();
                    dest.Isbn = dest.Isbn?.Replace("-", "").Replace(" ", ""); // Normalisation ISBN
                    if (string.IsNullOrEmpty(dest.CoverUrl))
                        dest.CoverUrl = null;
                    if (string.IsNullOrEmpty(dest.Description))
                        dest.Description = null;
                    if (string.IsNullOrEmpty(dest.Publisher))
                        dest.Publisher = null;
                })
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Mapping pour la mise à jour partielle (PATCH)
            CreateMap<Book, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}