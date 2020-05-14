using AutoMapper;
using BookStoreApi.DTOs;
using BookStoreApi.Models;

namespace BookStoreApi.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
        }
    }
}
