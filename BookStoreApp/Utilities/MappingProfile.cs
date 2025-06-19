using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace BookStoreApp.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookDtoForUpdate, Book>();
              
        }
    }
}
