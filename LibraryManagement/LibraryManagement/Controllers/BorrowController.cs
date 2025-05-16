using LibraryManagement.DTOs.Borrow;
using LibraryManagement.DTOs.User;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BorrowController : ControllerBase
{
    private readonly IBorrowBookService _borrowService;

    public BorrowController(IBorrowBookService borrowService)
    {
        _borrowService = borrowService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllBorrowTransactions()
    {
        var borrowResponses = await _borrowService.GetAllBorrowTransactionsAsync();
        return Ok(borrowResponses);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetBorrowTransactionById(int id)
    {
        var borrowResponse = await _borrowService.GetBorrowTransactionByIdAsync(id);
        return Ok(borrowResponse);
    }

    [HttpGet("user/{phoneNumber}")]
    [Authorize]
    public async Task<IActionResult> GetBorrowTransactionsByPhoneNumber(string phoneNumber)
    {
        var borrowResponses = await _borrowService.GetBorrowTransactionsByPhoneNumberAsync(phoneNumber);
        return Ok(borrowResponses);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateBorrowTransaction([FromBody] CreateBorrowTransactionRequest request)
    {
        var borrowResponse = await _borrowService.CreateBorrowTransactionAsync(request);
        if (borrowResponse == null)
            return BadRequest("Borrow transaction creation failed");
        return CreatedAtAction(nameof(GetBorrowTransactionById), new { id = borrowResponse.Id }, borrowResponse);
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> PatchBorrowTransaction(int id, [FromBody] PatchBorrowTransactionRequest request)
    {
        var borrowResponse = await _borrowService.PatchBorrowTransactionAsync(id, request);

        return Ok(borrowResponse);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteBorrowTransaction(int id)
    {
        var result = await _borrowService.DeleteBorrowTransactionAsync(id);
        if (!result)
            return BadRequest("Borrow transaction deletion failed");
        return NoContent();
    }
}