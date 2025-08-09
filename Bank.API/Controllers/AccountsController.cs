using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("bank/[controller]")]
    public class AccountsController : ControllerBase
    {
        // [HttpGet]
        // public async Task<ActionResult<List<AccountResponseDto>>> GetAllAccounts()
        // {
        //     var users = await _usersService.GetAllUsersAsync();

        //     // Map the users to a list of response DTOs
        //     var userResponseDtos = _mapper.Map<List<UserResponseDto>>(users);

        //     return Ok(userResponseDtos);
        // }
    }
}