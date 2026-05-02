using AutoMapper;
using back_end.BLL.DTOs;
using back_end.Domain;

namespace back_end.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
            CreateMap<Order, OrderDto>()
                  .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<Vehicle, VehicleDto>();
            CreateMap<CreateVehicleDto, Vehicle>();
            CreateMap<UpdateVehicleDto, Vehicle>();

            CreateMap<JobApplication, JobApplicationDto>();
            CreateMap<CreateJobApplicationDto, JobApplication>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "New"));

            CreateMap<Lead, LeadDto>();
            CreateMap<CreateLeadDto, Lead>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "New"));
        }
    }
}
