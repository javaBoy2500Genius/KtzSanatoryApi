using KTZSanatoriumServiceApi.Data.Repository;
using KTZSanatoriumServiceApi.Models;
using KTZSanatoriumServiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace KTZSanatoriumServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SanatoryController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDbRepository<Sanatory> _sanatories;

        public SanatoryController(IUserService userService, IDbRepository<Sanatory> sanatories)
        {
            _userService = userService;
            _sanatories = sanatories;
        }

        [HttpGet("sanatories")]
        public IActionResult GetSanatories()
        {
            try
            {
                var sanatories = _sanatories.FindAll(x => !x.IsDeleted);
                return Ok(sanatories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpPost("sanatory")]
        public IActionResult CreateSanatory(Sanatory sanatory)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _sanatories.InsertOne(sanatory);

                return CreatedAtAction(nameof(CreateSanatory), sanatory);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpPost("sanatories")]
        public IActionResult CreateSanatories(List<Sanatory> sanatories)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _sanatories.InsertMany(sanatories);

                return CreatedAtAction(nameof(CreateSanatory), sanatories);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpDelete("sanatory/{id}")]
        public IActionResult DeleteSanatory(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (_sanatories.FindById(id) == null)
                    throw new Exception("id not found");

                var updateDefinition = Builders<Sanatory>.Update.Set(x => x.IsDeleted, true);
                _sanatories.UpdateOne(x => x.Id == id, updateDefinition);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpPost("del/sanatories")]
        public IActionResult DeleteSanatories(List<Sanatory> sanatories)
        {
            try
            {
                var ids = sanatories.Select(x => x.Id).ToList();
                var filter = Builders<Sanatory>.Filter.In(x => x.Id, ids);
                var updateDefinition = Builders<Sanatory>.Update.Set(x => x.IsDeleted, true);
                if (!_sanatories.UpdateMany(filter, updateDefinition))
                    throw new Exception("failed deleted");
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpPut("sanatory")]
        public IActionResult UpdateSanatory(Sanatory updateDoc)
        {
            try
            {
                var sanatory = _sanatories.FindOne(x => x.Id == updateDoc.Id && !x.IsDeleted) ??
                    throw new Exception("id not valid");

                if (!_sanatories.ReplaceOne(updateDoc))
                    throw new Exception("failed update");
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [HttpPut("sanatories")]
        public IActionResult UpdateSanatories(List<Sanatory> updateDocs)
        {
            try
            {
                foreach (var updateDoc in updateDocs)
                {
                    var sanatory = _sanatories.FindOne(x => x.Id == updateDoc.Id && !x.IsDeleted) ?? throw new Exception("id not valid");
                    if (!_sanatories.ReplaceOne(updateDoc))
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
