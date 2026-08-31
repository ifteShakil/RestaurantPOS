using AutoMapper;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Shared.DTOs.Branch;
using RestaurantPOS.Shared.DTOs.Menu;
using RestaurantPOS.Shared.DTOs.Order;
using RestaurantPOS.Shared.DTOs.Payment;
using RestaurantPOS.Shared.DTOs.Table;
using RestaurantPOS.Shared.DTOs.User;

namespace RestaurantPOS.API.MappingProfiles;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        // Branch
        CreateMap<Branch, BranchDto>();
        CreateMap<CreateBranchDto, Branch>();

        // Menu
        CreateMap<MenuItem, MenuItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BasePrice)); // পরে branch override লজিক আলাদাভাবে বসাবো
        CreateMap<CreateMenuCategoryDto, MenuCategory>();
        CreateMap<MenuItemVariant, MenuItemVariantDto>();
        CreateMap<MenuCategory, MenuCategoryDto>();
        CreateMap<CreateMenuItemDto, MenuItem>();

        // Table
        CreateMap<RestaurantTable, TableDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<CreateTableDto, RestaurantTable>();

        // Order
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TableName, opt => opt.MapFrom(src => src.Table!.Name))
            .ForMember(dest => dest.WaiterName, opt => opt.MapFrom(src => src.Waiter!.FullName))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Items.Sum(i => i.UnitPrice * i.Quantity)));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem!.Name))
            .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.MenuItemVariant != null ? src.MenuItemVariant.Name : null));

        // Bill & Payment
        CreateMap<Bill, BillDto>();
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.PaymentMethodName, opt => opt.MapFrom(src => src.PaymentMethodType!.Name));

        // User
        CreateMap<AppUser, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null));
    }
}
