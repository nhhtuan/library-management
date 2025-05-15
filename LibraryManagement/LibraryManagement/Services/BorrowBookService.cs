using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs.Borrow;

namespace LibraryManagement.Services;

public class BorrowBookService
{
    private readonly IMapper _mapper;
    private readonly LibraryDbContext _db;

    public BorrowBookService(LibraryDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }



}