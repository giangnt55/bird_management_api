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
            Fullname = "Cao Thuy Phuong Thuy",
            Username = "Thuy",
            Password = SecurityExtension.HashPassword<User>("Thuy123@", salt),
            Role = UserRole.Admin,
            Salt = salt,
            Status = UserStatus.Active,
            PhoneNumber = "0977264752",
            Avatar = "https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/263756895_1566058877076736_6535635120033309997_n.jpg?_nc_cat=110&cb=99be929b-3346023f&ccb=1-7&_nc_sid=0debeb&_nc_ohc=I4m9dKkLLdkAX9jyuL7&_nc_ht=scontent.fsgn2-6.fna&oh=00_AfBc1Ft06Gi3c2mBihIuWOdMXoxkMrIlNoR9ow6JgD9seQ&oe=64A06897"
        };

        await _unitOfWork.UserRepository.InsertAsync(user, Guid.Empty, DateTime.UtcNow);
        return Ok();
    }
}
