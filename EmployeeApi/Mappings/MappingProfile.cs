using AutoMapper;
using EmployeeApi.DTOs;
using EmployeeApi.Models;
namespace EmployeeApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.DateJoined, opt => opt.MapFrom(src => DateTime.Now)); // Default value
        }
    }
}