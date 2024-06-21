using KTZSanatoriumServiceApi.Data.Repository;
using KTZSanatoriumServiceApi.Models;
using KTZSanatoriumServiceApi.ModelsDto;
using KTZSanatoriumServiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Net;

namespace KTZSanatoriumServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDbRepository<User> _users;
        private readonly IDbRepository<Profile> _proffiles;

        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;


        public AuthController(
            IDbRepository<User> users,
            IDbRepository<Profile> proffiles,
            IUserService userService,
            ILogger<AuthController> logger)
        {
            _users = users;
            _proffiles = proffiles;
            _userService = userService;
            _logger = logger;
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpPost("register")]
        public IActionResult Register(UsersDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var r = _userService.GetClaimValue(UserClaimEnum.UserRole);
                if (user.Role != UserRole.User &&
                    _userService.GetClaimValue(UserClaimEnum.UserRole) == UserRole.Admin.ToString())
                    throw new Exception("permission denied");

                var userEx = _users.FindOne(x => x.Login == user.Login && !x.IsDeleted);

                if (userEx != null)
                    throw new Exception("user exist");

                var profile = new Profile()
                {
                    ApplicationStatus = user.Profile.ApplicationStatus,
                    BirthDayDate = user.Profile.BirthDayDate,
                    Branch = user.Profile.Branch,
                    Category = user.Profile.Category,
                    ComeInDate = user.Profile.ComeInDate,
                    DReport = user.Profile.DReport,
                    HonararyRailway = user.Profile.HonararyRailway,
                    IIN = user.Profile.IIN,
                    IsDeleted = user.Profile.IsDeleted,
                    Major = user.Profile.Major,
                    MedicalReport = user.Profile.MedicalReport,
                    MiddleName = user.Profile.MiddleName,
                    Name = user.Profile.Name,
                    NumberOfCardId = user.Profile.NumberOfCardId,
                    PersonalNumber = user.Profile.PersonalNumber,
                    Surname = user.Profile.Surname


                };

                _proffiles.InsertOne(profile);

                var newUser = new User
                {
                    ProfileId = profile.Id,
                    IsDeleted = profile.IsDeleted,
                    Login = user.Login,
                    Password = _userService.Encrypt(user.Password),
                    Role = user.Role,
                };

                _users.InsertOne(newUser);


                return CreatedAtAction(nameof(Register), user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] ModelsDto.Authorization data)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = _users.FindOne(x => x.Login == data.Login && !x.IsDeleted);
                if (user == null)
                    return BadRequest("user login not found");

                if (_userService.Decrypt(user.Password) != data.Password)
                    return BadRequest("user passwod not equal");

                var token = _userService.CreateToken(new TokenSetiing
                {
                    TokenLifeTime = DateTime.Now.AddDays(1),
                    UserId = user.Id,
                    UserRole = user.Role
                });
                return Ok(token);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

   

    }
}
