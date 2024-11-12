using AutoMapper;
using ItalianCharmBracelet.Data;
using ItalianCharmBracelet.ViewModels;

namespace ItalianCharmBracelet.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, Customer>();
        }
    }
}
