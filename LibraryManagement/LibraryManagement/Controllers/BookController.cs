using AutoMapper;
using LibraryManagement.DTOs.Book;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public BookController(IBookService bookService, IMapper mapper)
    {
        _bookService = bookService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllBooks([FromQuery] BookFilterRequest filter)
    {
        var bookResponses = await _bookService.GetAllBooksAsync(filter);
        return Ok(bookResponses);
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetBookById(int id)
    {
        var bookResponse = await _bookService.GetBookByIdAsync(id);
        if (bookResponse == null)
            return NotFound("Book not found");
        return Ok(bookResponse);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
    {
        var bookResponse = await _bookService.CreateBookAsync(request);
        if (bookResponse == null)
            return BadRequest("Book creation failed");
        return CreatedAtAction(nameof(GetBookById), new { id = bookResponse.Id }, bookResponse);
    }


    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookRequest request)
    {
        var bookResponse = await _bookService.UpdateBookAsync(id, request);
        if (bookResponse == null)
            return NotFound("Book not found");
        return Ok(bookResponse);
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int id)
    {

        // Check if the book exists
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound("Book not found");

        var result = await _bookService.DeleteBookAsync(id);
        if (!result)
            return BadRequest("Failed to delete the book");
        return Ok("Book deleted successfully");
    }
}