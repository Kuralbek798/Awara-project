using AutoMapper;
using AwaraIT.Training.Domain.Models.Crm.DTO;
using AwaraIT.Training.Domain.Models.Crm.Entities;

namespace AwaraIT.Kuralbek.Plugins.Hellpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Настройка маппинга между ProductCart и ProductCartDTO
            CreateMap<ProductCart, ProductCartDTO>()
                .ForMember(dest => dest.PossibleDealId, opt => opt.Ignore()); // Игнорирование поля PossibleDealId

            CreateMap<ProductCartDTO, ProductCart>()
                .ForMember(dest => dest.ProductCartId, opt => opt.MapFrom(src => src.ProductCartId))
                .ForMember(dest => dest.ProductReference, opt => opt.MapFrom(src => src.ProductReference))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.PriceAfterDiscount, opt => opt.MapFrom(src => src.PriceAfterDiscount));

        }
    }
}
