using AutoMapper; 

namespace api.Models
{
    /// <summary>
    /// Profile AutoMapper pour les mappings entre l'entité Loan et ses DTOs.
    /// </summary>
    public class LoanProfile : Profile
    {
        public LoanProfile()
        {
            // Mapping de Loan vers LoanDto
            CreateMap<Loan, LoanDto>();

            // Mapping de CreateLoanDto vers Loan
            CreateMap<CreateLoanDto, Loan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Défini dans le service
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.LoanDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "BORROWED"))
                .ForMember(dest => dest.ReturnDate, opt => opt.Ignore());

            // Mapping de UpdateLoanDto vers Loan
            CreateMap<UpdateLoanDto, Loan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.BookId, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.LoanDate, opt => opt.Ignore())
                .ForMember(dest => dest.DueDate, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}