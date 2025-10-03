using AutoMapper;
using api.Models;

namespace api.Models 
{
    /// <summary>
    /// Profile AutoMapper pour les mappings entre l'entité Reservation et ses DTOs.
    /// </summary>
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            // Mapping de Reservation vers ReservationDto
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title));

            // Mapping de CreateReservationDto vers Reservation
            CreateMap<CreateReservationDto, Reservation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Défini dans le service
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ExpiryDate, opt => opt.Ignore()) // Calculé dans le service
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "PENDING"))
                .ForMember(dest => dest.AvailableSince, opt => opt.Ignore())
                .ForMember(dest => dest.FulfilledLoanId, opt => opt.Ignore())
                .ForMember(dest => dest.FulfilledLoan, opt => opt.Ignore());

            // Mapping de UpdateReservationDto vers Reservation
            CreateMap<UpdateReservationDto, Reservation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.BookId, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.ReservationDate, opt => opt.Ignore())
                .ForMember(dest => dest.FulfilledLoan, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}