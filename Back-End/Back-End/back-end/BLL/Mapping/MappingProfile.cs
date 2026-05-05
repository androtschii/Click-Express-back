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
                .ForMember(dest => dest.ProductName,  opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Model : null))
                .ForMember(dest => dest.DriverName,   opt => opt.MapFrom(src => src.Driver != null ? src.Driver.FullName : null));

            CreateMap<Vehicle, VehicleDto>();
            CreateMap<CreateVehicleDto, Vehicle>();
            CreateMap<UpdateVehicleDto, Vehicle>();

            CreateMap<JobApplication, JobApplicationDto>();
            CreateMap<CreateJobApplicationDto, JobApplication>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "New"));

            CreateMap<Lead, LeadDto>();
            CreateMap<CreateLeadDto, Lead>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "New"));

            CreateMap<Driver, DriverDto>()
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Model : null));
            CreateMap<CreateDriverDto, Driver>();
            CreateMap<UpdateDriverDto, Driver>();

            CreateMap<SavedLoad, SavedLoadDto>()
                .ForMember(dest => dest.ProductName,        opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice,       opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ProductImageUrl,    opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductCategory,    opt => opt.MapFrom(src => src.Product.Category))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description));
        }
    }
}
