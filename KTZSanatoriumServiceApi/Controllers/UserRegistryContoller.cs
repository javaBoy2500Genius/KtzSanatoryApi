using KTZSanatoriumServiceApi.Data.Repository;
using KTZSanatoriumServiceApi.Models;
using KTZSanatoriumServiceApi.ModelsDto;
using KTZSanatoriumServiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using KTZSanatoriumServiceApi.Helpers.FilterHelper;
using static MongoDB.Driver.WriteConcern;

namespace KTZSanatoriumServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]

    public class UserRegistryContoller : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDbRepository<UserRegistryTrans> _usersRegisries;
        private readonly IDbRepository<User> _users;
        private readonly IDbRepository<Profile> _profiles;

        public UserRegistryContoller(IUserService userService, IDbRepository<UserRegistryTrans> usersRegisries, IDbRepository<User> users, IDbRepository<Profile> profiles)
        {
            _userService = userService;
            _usersRegisries = usersRegisries;
            _users = users;
            _profiles = profiles;
        }
        [FilterHelperAtribute<UserRegistryDto>]

        [HttpGet("userRegistries")]
        public IActionResult GetUserRegistries()
        {
            try
            {
                var usersRegistries = _usersRegisries
                    .FindAll(x => !x.IsDeleted)
                    .Join(_users.AsQueryable(),
                    userRegis => userRegis.UserId,
                    user => user.Id,
                    (userRegis, user) => new { userRegis, user })

                    .Join(_profiles.AsQueryable(),
                        combine => combine.user.ProfileId,
                        profile => profile.Id,
                        (combine, profle) => new { combine, profle }
                    );
                var results =
                    usersRegistries
                    .AsEnumerable()
                    .Select(x => new UserRegistryDto
                    {
                        Branch = x.combine.userRegis.Branch,
                        Id = x.combine.userRegis.Id,
                        ComeInDate = x.combine.userRegis.ComeInDate,
                        IIN = x.combine.userRegis.IIN,
                        IsDeleted = x.combine.userRegis.IsDeleted,
                        IsDReport = x.combine.userRegis.IsDReport,
                        MiddleName = x.combine.userRegis.MiddleName,
                        Name = x.combine.userRegis.Name,
                        PersonalNumber = x.combine.userRegis.PersonalNumber,
                        Plot = x.combine.userRegis.Plot,
                        Points = x.combine.userRegis.Points,
                        SanatoryName = x.combine.userRegis.SanatoryName,
                        SocialHelp = x.combine.userRegis.SocialHelp,
                        SurName = x.combine.userRegis.SurName,
                        TripStatus = x.combine.userRegis.TripStatus,
                        User = new UsersDto
                        {
                            Id = x.combine.user.Id,
                            IsDeleted = x.combine.user.IsDeleted,
                            Login = x.combine.user.Login,
                            Password = "",
                            Role = x.combine.user.Role,
                            Profile = x.profle
                        }
                    });





                return Ok(results.AsQueryable());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("userRegistry")]
        public IActionResult CreateUserRegistry([FromBody] UserRegistryDto data)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var userProfileId = _profiles.FindOne(x => x.IIN == data.IIN && !x.IsDeleted)?.Id ?? "";
                var user = _users.FindOne(x => x.ProfileId == userProfileId && !x.IsDeleted);

                var userRegistry = new UserRegistryTrans
                {
                    UserId = user?.Id ?? "",
                    IIN = data.IIN,
                    Branch = data.Branch,
                    ComeInDate = data.ComeInDate,
                    IsDeleted = data.IsDeleted,
                    IsDReport = data.IsDReport,
                    MiddleName = data.MiddleName,
                    Name = data.Name,
                    PersonalNumber = data.PersonalNumber,
                    Plot = data.Plot,
                    Points = data.Points,
                    SanatoryName = data.SanatoryName,
                    SocialHelp = data.SocialHelp,
                    SurName = data.SurName,
                    TripStatus = data.TripStatus,
                };

                if (user == null)
                {
                    var profile = new Profile
                    {
                        Branch = userRegistry.Branch,
                        ComeInDate = userRegistry.ComeInDate,
                        IsDeleted = userRegistry.IsDeleted,
                        Surname = userRegistry.SurName,
                        Name = userRegistry.Name,
                        MiddleName = userRegistry.MiddleName,
                        IIN = userRegistry.IIN,
                        PersonalNumber = userRegistry.PersonalNumber,
                        Major = data.User?.Profile?.Major ?? "",
                        BirthDayDate = data.User?.Profile?.BirthDayDate ?? DateTime.MinValue,
                        NumberOfCardId = data.User?.Profile?.NumberOfCardId ?? ""
                    };
                    _profiles.InsertOne(profile);

                    var newUser = new User
                    {
                        ProfileId = profile.Id,
                        IsDeleted = profile.IsDeleted,
                        Login = userRegistry.IIN,
                        Password = _userService.Encrypt(userRegistry.PersonalNumber)
                    };
                    _users.InsertOne(newUser);
                    userRegistry.UserId = newUser.Id;
                }
                _usersRegisries.InsertOne(userRegistry);

                return CreatedAtAction(nameof(CreateUserRegistry), userRegistry);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpPost("userRegistries")]
        public IActionResult CreateUsersRegistries([FromBody] List<UserRegistryDto> listData)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var newUsers = new List<User>();
                var userRegistries = new List<UserRegistryTrans>();
                foreach (var data in listData)
                {
                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);


                    var userProfileId = _profiles.FindOne(x => x.IIN == data.IIN && !x.IsDeleted)?.Id ?? "";
                    var user = _users.FindOne(x => x.ProfileId == userProfileId && !x.IsDeleted);

                    var userRegistry = new UserRegistryTrans
                    {
                        UserId = user?.Id ?? "",
                        IIN = data.IIN,
                        Branch = data.Branch,
                        ComeInDate = data.ComeInDate,
                        IsDeleted = data.IsDeleted,
                        IsDReport = data.IsDReport,
                        MiddleName = data.MiddleName,
                        Name = data.Name,
                        PersonalNumber = data.PersonalNumber,
                        Plot = data.Plot,
                        Points = data.Points,
                        SanatoryName = data.SanatoryName,
                        SocialHelp = data.SocialHelp,
                        SurName = data.SurName,
                        TripStatus = data.TripStatus,
                    };

                    if (user == null)
                    {
                        var profile = new Profile
                        {
                            Branch = userRegistry.Branch,
                            ComeInDate = userRegistry.ComeInDate,
                            IsDeleted = userRegistry.IsDeleted,
                            Surname = userRegistry.SurName,
                            Name = userRegistry.Name,
                            MiddleName = userRegistry.MiddleName,
                            IIN = userRegistry.IIN,
                            PersonalNumber = userRegistry.PersonalNumber,
                            Major = data.User?.Profile?.Major ?? "",
                            BirthDayDate = data.User?.Profile?.BirthDayDate ?? DateTime.MinValue,
                            NumberOfCardId = data.User?.Profile?.NumberOfCardId ?? ""

                        };
                        _profiles.InsertOne(profile);

                        var newUser = new User
                        {
                            ProfileId = profile.Id,
                            IsDeleted = profile.IsDeleted,
                            Login = userRegistry.IIN,
                            Password = _userService.Encrypt(userRegistry.PersonalNumber)
                        };
                        userRegistry.UserId = newUser.Id;
                        newUsers.Add(newUser);
                    }
                    userRegistries.Add(userRegistry);
                }

                _users.InsertMany(newUsers);
                _usersRegisries.InsertMany(userRegistries);
                return CreatedAtAction(nameof(CreateUserRegistry), listData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("userRegistry/{id}")]
        public IActionResult DeleteUserRegistry(string id)
        {
            try
            {
                var userReg = _usersRegisries.FindOne(x => x.Id == id && !x.IsDeleted) ??
                    throw new Exception("user registry not found");


                var updateDefinition = Builders<UserRegistryTrans>.Update.Set(x => x.IsDeleted, true);

                if (!_usersRegisries.UpdateOne(x => x.Id == id, updateDefinition))
                    throw new Exception("failed deleted");
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("del/userRegistries")]
        public IActionResult DeleteUserRegistries(List<UserRegistryDto> userRegistryTrans)
        {
            try
            {
                var ids = userRegistryTrans.Select(x => x.Id).ToList();
                var filter = Builders<UserRegistryTrans>.Filter.In(x => x.Id, ids);
                var updateDefinition = Builders<UserRegistryTrans>.Update.Set(x => x.IsDeleted, true);
                if (!_usersRegisries.UpdateMany(filter, updateDefinition))
                    throw new Exception("failed deleted");
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        [HttpPut("userRegistry")]
        public IActionResult UpdateUserRegistry(UserRegistryDto dto)
        {
            try
            {
                var userReg = _usersRegisries.FindOne(x => x.Id == dto.Id && !x.IsDeleted);
                if (userReg == null)
                    throw new Exception("id not valid");

                var update = new UserRegistryTrans()
                {
                    Branch = dto.Branch,
                    Id = dto.Id,
                    ComeInDate = dto.ComeInDate,
                    IIN = dto.IIN,
                    IsDeleted = dto.IsDeleted,
                    IsDReport = dto.IsDReport,
                    MiddleName = dto.MiddleName,
                    Name = dto.Name,
                    PersonalNumber = dto.PersonalNumber,
                    Plot = dto.Plot,
                    Points = dto.Points,
                    SanatoryName = dto.SanatoryName,
                    SocialHelp = dto.SocialHelp,
                    SurName = dto.SurName,
                    TripStatus = dto.TripStatus,
                    UserId = dto.User?.Id ?? ""
                };

                if (!_usersRegisries.ReplaceOne(update))
                    throw new Exception("failed update");
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("userRegistries")]
        public IActionResult UpdateUserRegistries(List<UserRegistryDto> updateDocs)
        {
            try
            {
                foreach (var dto in updateDocs)
                {
                    var userReg = _usersRegisries.FindOne(x => x.Id == dto.Id && !x.IsDeleted);
                    if (userReg == null)
                        throw new Exception("id not valid");

                    var update = new UserRegistryTrans()
                    {
                        Branch = dto.Branch,
                        Id = dto.Id,
                        ComeInDate = dto.ComeInDate,
                        IIN = dto.IIN,
                        IsDeleted = dto.IsDeleted,
                        IsDReport = dto.IsDReport,
                        MiddleName = dto.MiddleName,
                        Name = dto.Name,
                        PersonalNumber = dto.PersonalNumber,
                        Plot = dto.Plot,
                        Points = dto.Points,
                        SanatoryName = dto.SanatoryName,
                        SocialHelp = dto.SocialHelp,
                        SurName = dto.SurName,
                        TripStatus = dto.TripStatus,
                        UserId = dto.User?.Id ?? ""
                    };
                    if (!_usersRegisries.ReplaceOne(update))
                        throw new Exception("failed update");
                }


                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
