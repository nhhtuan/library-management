using AutoMapper;
using LibraryManagement.DTOs.Auth;
using LibraryManagement.DTOs.Book;
using LibraryManagement.DTOs.Borrow;
using LibraryManagement.DTOs.User;
using LibraryManagement.Enums;
using LibraryManagement.Models;

namespace LibraryManagement.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.UserRole.ToString())));
            CreateMap<UpdateUserRequest, UserResponse>();
            // Auth mappings
            CreateMap<LoginRequest, User>();

            // Book mappings
            CreateMap<Book, BookResponse>();


            // Borrow mappings
            CreateMap<CreateBorrowTransactionRequest, BorrowTransaction>().
            ForMember(dest => dest.BookBorrowTransactions, otp => otp.Ignore());
            CreateMap<BorrowTransaction, BorrowTransactionResponse>()
                .ForMember(dest => dest.BorrowedBooks, opt => opt.MapFrom(src => src.BookBorrowTransactions.Select(b => new BookBorrowTransactionResponse
                {
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    Author = b.Book.Author,
                    Genre = b.Book.Genre,
                    PublishedYear = b.Book.PublishedYear,
                    Description = b.Book.Description,
                    BorrowedQuantity = b.Quantity
                })));
        }
    }
}