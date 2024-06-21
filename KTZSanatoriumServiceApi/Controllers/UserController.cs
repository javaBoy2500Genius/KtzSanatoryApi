using KTZSanatoriumServiceApi.Data.Repository;
using KTZSanatoriumServiceApi.Models;
using KTZSanatoriumServiceApi.ModelsDto;
using KTZSanatoriumServiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Data;
using System.Reflection.Metadata;

namespace KTZSanatoriumServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDbRepository<User> _users;
        private readonly IDbRepository<Profile> _profiles;
        private readonly IDbRepository<Applications> _applications;
        private readonly IDbRepository<Sanatory> _sanatories;

        public UserController(IUserService userService, IDbRepository<User> users, IDbRepository<Profile> profiles, IDbRepository<Applications> applications, IDbRepository<Sanatory> sanatories)
        {
            _userService = userService;
            _users = users;
            _profiles = profiles;
            _applications = applications;
            _sanatories = sanatories;
        }


        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpPatch("changeUserRole/{id}")]
        public IActionResult ChangeUserRole(string id, UserRole role)
        {
            try
            {
                if (_users.FindById(id) == null)
                    throw new Exception("user not found");

                if (role != UserRole.User
                    && _userService.GetClaimValue(UserClaimEnum.UserRole) != UserRole.SuperAdmin.ToString())
                    throw new Exception("permission denied");

                var update = Builders<User>.Update.Set(x => x.Role, role);
                if (!_users.UpdateOne(x => x.Id == id, update)) throw new Exception("update failed");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [HttpPut("updateProfile")]
        public IActionResult UpdateProfile(ProfileDto profile)
        {
            try
            {
                _ = _profiles.FindById(profile.Id) ?? throw new Exception("profile not found");
                _ = _sanatories.FindById(profile.SanatoryId) ?? throw new Exception("sanatory not found");

                var user = _users.FindOne(x => x.ProfileId == profile.Id && !x.IsDeleted) ?? throw new Exception("user not found");
                var application = _applications.FindOne(x =>
                            x.UserId == user.Id &&
                            !x.IsDeleted &&
                            x.ApplicationStatus != ApplicationStatus.Acept &&
                            x.ApplicationStatus != ApplicationStatus.Decline);


                var updateProfile = new Profile
                {
                    ApplicationStatus = profile.ApplicationStatus,
                    Id = profile.Id,
                    BirthDayDate = profile.BirthDayDate,
                    Branch = profile.Branch,
                    Category = profile.Category,
                    ComeInDate = profile.ComeInDate,
                    CreatedDate = profile.CreatedDate,
                    DReport = profile.DReport,
                    HonararyRailway = profile.HonararyRailway,
                    IIN = profile.IIN,
                    IsDeleted = profile.IsDeleted,
                    Major = profile.Major,
                    MedicalReport = profile.MedicalReport,
                    MiddleName = profile.MiddleName,
                    Name = profile.Name,
                    NumberOfCardId = profile.NumberOfCardId,
                    PersonalNumber = profile.PersonalNumber,
                    Surname = profile.Surname,

                };
                if (!_profiles.ReplaceOne(updateProfile))
                    throw new Exception("failed update profile");

                if (profile.IsDeleted)
                {
                    user.IsDeleted = true;
                    if (!_users.ReplaceOne(user))
                        throw new Exception("failed update user");
                }

                if (application == null)
                {
                    application = new Applications()
                    {
                        ApplicationStatus = ApplicationStatus.Consideration,
                        UserId = user.Id,
                        SanatoryId = profile.SanatoryId,
                        IsDeleted = false,
                        ComeInDate = profile.ComeInDate,
                    };
                    _applications.InsertOne(application);
                }
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpDelete("deleteUser/{id}")]
        public IActionResult DeleteUser(string id)
        {
            try
            {
                if (_users.FindById(id) == null)
                    throw new Exception("user not found");

                if (_userService.GetClaimValue(UserClaimEnum.UserRole) != UserRole.SuperAdmin.ToString())
                    throw new Exception("permission denied");

                var update = Builders<User>.Update.Set(x => x.IsDeleted, true);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpPost("deleteUsers")]
        public IActionResult DeleteUsers(List<User> users)
        {
            try
            {
                if (_userService.GetClaimValue(UserClaimEnum.UserRole) != UserRole.SuperAdmin.ToString())
                    throw new Exception("permission denied");

                var ids = users.Select(x => x.Id).ToList();
                var filter = Builders<User>.Filter.In(x => x.Id, ids);
                var updateDefinition = Builders<User>.Update.Set(x => x.IsDeleted, true);

                if (!_users.UpdateMany(filter, updateDefinition))
                    throw new Exception("failed deleted");


                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [HttpGet("me")]
        public IActionResult GetMe()
        {
            try
            {
                var user = _users.FindById(_userService.GetClaimValue(UserClaimEnum.UserId)) ?? throw new Exception("user not found");
                var profile = _profiles.FindById(user.ProfileId) ?? throw new Exception("profile not found");

                var result = new UsersDto
                {
                    Id = user.Id,
                    Profile = profile,
                    IsDeleted = user.IsDeleted,
                    Login = user.Login,
                    Role = user.Role,
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
