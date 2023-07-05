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

    var users = new List<User>{
        new User
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
            Avatar = "https://scontent.fsgn2-7.fna.fbcdn.net/v/t39.30808-6/313442034_1807317972944994_4019730258150584617_n.jpg?_nc_cat=100&cb=99be929b-3346023f&ccb=1-7&_nc_sid=09cbfe&_nc_ohc=M_CPNK2CZGoAX-mxKQ9&_nc_ht=scontent.fsgn2-7.fna&oh=00_AfAYqVkcgGpNwlUnATVWJpNDULn1QTrnfjQhaQh7kHMJVw&oe=64A8897A"
            },
        new User
            {
            Id = Guid.NewGuid(),
            Email = "Quang@gmail.com",
            Address = "Binh Dinh",
            Fullname = "Phan Van Quang",
            Username = "Quang",
            Password = SecurityExtension.HashPassword<User>("Quang123@", salt),
            Role = UserRole.Member,
            Salt = salt,
            Status = UserStatus.Active,
            PhoneNumber = "0977264754",
            Avatar = "https://scontent.fsgn2-4.fna.fbcdn.net/v/t1.6435-9/46511023_112887389730166_4777066461464100864_n.jpg?_nc_cat=101&cb=99be929b-3346023f&ccb=1-7&_nc_sid=09cbfe&_nc_ohc=3NWrwXo914oAX_vSol2&_nc_ht=scontent.fsgn2-4.fna&oh=00_AfCPs78anqRhOY8axN3B20mEOLWmXEXDWd0zsPeGqJ04zw&oe=64CBCA93"
            },
        new User
            {
            Id = Guid.NewGuid(),
            Email = "Trieu@gmail.com",
            Address = "Thu Duc, TP Ho Chi Minh",
            Fullname = "Do Hai Trieu",
            Username = "Trieu",
            Password = SecurityExtension.HashPassword<User>("Trieu123@", salt),
            Role = UserRole.Member,
            Salt = salt,
            Status = UserStatus.Active,
            PhoneNumber = "0977264751",
            Avatar = "https://scontent.fsgn2-6.fna.fbcdn.net/v/t39.30808-6/320325615_700525281683853_2489659888413717520_n.jpg?_nc_cat=110&cb=99be929b-3346023f&ccb=1-7&_nc_sid=174925&_nc_ohc=79TLhbiqpAQAX-6d5b1&_nc_ht=scontent.fsgn2-6.fna&oh=00_AfDhEYhkCllvKTwtaKUlm8J4VeIBoquyTEapW5H3v9RxmQ&oe=64AA0A98"
            }
    };

    await _unitOfWork.UserRepository.InsertAsync(users, Guid.Empty, DateTime.UtcNow);
    return Ok();
  }
}
