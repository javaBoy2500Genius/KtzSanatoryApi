using KTZSanatoriumServiceApi.Data.Repository;
using KTZSanatoriumServiceApi.Helpers;
using KTZSanatoriumServiceApi.Models;
using KTZSanatoriumServiceApi.ModelsDto;
using KTZSanatoriumServiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace KTZSanatoriumServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDbRepository<Applications> _dbApplication;
        private readonly IDbRepository<UserRegistryTrans> _dbUserRegistry;
        private readonly IDbRepository<User> _dbUsers;
        private readonly IDbRepository<Profile> _dbProfiles;
        private readonly IDbRepository<Sanatory> _dbSanatory;

        public ApplicationController(IUserService userService, IDbRepository<Applications> dbApplication, IDbRepository<UserRegistryTrans> dbUserRegistry, IDbRepository<User> dbUsers, IDbRepository<Profile> dbProfiles, IDbRepository<Sanatory> dbSanatory)
        {
            _userService = userService;
            _dbApplication = dbApplication;
            _dbUserRegistry = dbUserRegistry;
            _dbUsers = dbUsers;
            _dbProfiles = dbProfiles;
            _dbSanatory = dbSanatory;
        }

        [HttpGet("applications")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        public IActionResult GetApplications()
        {
            try
            {

                var applications = _dbApplication.FindAll(x => !x.IsDeleted)
                    .Join(_dbUsers.AsQueryable(),
                      application => application.UserId,
                      user => user.Id,
                      (application, user) => new { application, user })
                  .Join(_dbSanatory.AsQueryable(),
                      combined => combined.application.SanatoryId,
                      sanatory => sanatory.Id,
                      (combined, sanatory) => new { combined, sanatory })
                  .Join(_dbProfiles.AsQueryable(),
                  combined => combined.combined.user.ProfileId,
                  profile => profile.Id,
                  (combine, profile) => new { combine, profile })
                  .AsEnumerable()
                 ;

                var results = applications
                  .Select(x => new ApplicationDto
                  {
                      Id = x.combine.combined.application.Id,
                      ComeInDate = x.combine.combined.application.ComeInDate,
                      IsDeleted = x.combine.combined.application.IsDeleted,
                      ApplicationStatus = x.combine.combined.application.ApplicationStatus,
                      SocialHelp = x.combine.combined.application.SocialHelp,
                      User = new UsersDto
                      {
                          Id = x.combine.combined.user.Id,
                          CreatedDate = x.combine.combined.user.CreatedDate,
                          Profile = x.profile,
                          IsDeleted = x.combine.combined.user.IsDeleted,
                          Login = x.combine.combined.user.Login,
                          Password = "",
                          Role = x.combine.combined.user.Role,
                      },
                      Sanatory = x.combine.sanatory
                  });

                return Ok(results);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("application")]
        public IActionResult CreateApplication(Applications application)
        {
            try
            {
                var user = _dbUsers.FindById(application.UserId) ?? throw new Exception("user not found");
                var profile = _dbProfiles.FindById(user.ProfileId) ?? throw new Exception("profile not found");

                application.ApplicationStatus = ApplicationStatus.Consideration;
                application.Id = Guid.NewGuid().ToString();
                _dbApplication.InsertOne(application);
                var updateProfile = Builders<Profile>.Update.Set(x => x.ApplicationStatus, ApplicationStatus.Consideration);
                if (!_dbProfiles.UpdateOne(x => x.Id == profile.Id, updateProfile))
                    throw new Exception("update profile failed");


                return CreatedAtAction(nameof(CreateApplication), application);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("application/{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        public  IActionResult ChangeStatus(string id, ApplicationStatus status)
        {
            try
            {
                var application = _dbApplication.FindById(id) ?? throw new Exception("application not found");



                var user = _dbUsers.FindById(application.UserId) ?? throw new Exception("user not found");

                var profile = _dbProfiles.FindById(user.ProfileId) ?? throw new Exception("profile not found");

                var sanatory = _dbSanatory.FindById(application.SanatoryId) ?? throw new Exception("sanatory not found");

                var updateApplication = Builders<Applications>.Update.Set(x => x.ApplicationStatus, status);

                if (!_dbApplication.UpdateOne(x => x.Id == id, updateApplication))
                    throw new Exception("update application failed");


                var updateProfile = Builders<Profile>.Update.Set(x => x.ApplicationStatus, status);
                if (!_dbProfiles.UpdateOne(x => x.Id == profile.Id, updateProfile))
                    throw new Exception("update profile failed");

                if (status == ApplicationStatus.Acept)
                {
                        var userRegistry = _dbUserRegistry.FindOne(x => x.IIN == profile.IIN && !x.IsDeleted);
                    bool isNew = false;
                    if (userRegistry == null)
                    {
                        userRegistry = new UserRegistryTrans()
                        {
                            UserId = user.Id,
                            Branch = profile.Branch,
                            ComeInDate = profile.ComeInDate,
                            IIN = profile.IIN,
                            IsDReport = !string.IsNullOrWhiteSpace(profile.DReport),
                           
                            MiddleName = profile.MiddleName,
                            Name = profile.Name,
                            PersonalNumber = profile.PersonalNumber,
                            SanatoryName = sanatory.Name,
                            SurName = profile.Surname,
                            TripStatus = TripStatus.Waiting
                        };

                        isNew = true;

                    }
                    if (!string.IsNullOrWhiteSpace(profile.DReport))
                        userRegistry.Points = PointsCalculate.DReportConst;

                    if (!string.IsNullOrWhiteSpace(profile.HonararyRailway))
                        userRegistry.Points += PointsCalculate.HonararyRailwayConst;

                    if (DateTime.Now.Year - profile.ComeInDate.Date.Year > 1)
                        userRegistry.Points += 5;
                    else
                        userRegistry.Points += 3;

                    if (!isNew)
                    {
                        var update = Builders<UserRegistryTrans>.Update.Set(x => x.Points, userRegistry.Points);

                        if (!_dbUserRegistry.UpdateOne(x => x.Id == userRegistry.Id, update))
                            throw new Exception("can't update user registry");
                    }
                    else
                        _dbUserRegistry.InsertOne(userRegistry);

                }
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpDelete("application/{id}")]
        public IActionResult DeleteApplication(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (_dbApplication.FindById(id) == null)
                    throw new Exception("id not found");

                var updateDefinition = Builders<Applications>.Update.Set(x => x.IsDeleted, true);
                _dbApplication.UpdateOne(x => x.Id == id, updateDefinition);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("del/applications")]
        public IActionResult DeleteApplications(List<Applications> applications)
        {
            try
            {
                var ids = applications.Select(x => x.Id).ToList();
                var filter = Builders<Applications>.Filter.In(x => x.Id, ids);
                var updateDefinition = Builders<Applications>.Update.Set(x => x.IsDeleted, true);
                if (!_dbApplication.UpdateMany(filter, updateDefinition))
                    throw new Exception("failed deleted");
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
