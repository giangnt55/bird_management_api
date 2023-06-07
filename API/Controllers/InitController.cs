using AppCore.Extensions;
using MainData;
using MainData.Entities;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class InitController : BaseController
{
    
    private readonly MainUnitOfWork _unitOfWork;

    public InitController(MainUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        var salt = SecurityExtension.GenerateSalt();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "giangntse150747@gmail.com",
            Address = "Di An, Binh Duong",
            Fullname = "Nguyen Truong Giang",
            Username = "Giang",
            Password = SecurityExtension.HashPassword<User>("Giang123@", salt),
            Role = UserRole.Admin,
            Salt = salt,
            Status = UserStatus.Active,
            PhoneNumber = "0977264752",
        };

        await _unitOfWork.UserRepository.InsertAsync(user, Guid.Empty, DateTime.UtcNow);
        return Ok();
    }
}