using AutoMapper;
using CityInfo.API.Common;
using CityInfo.API.Models;
using CityInfo.API.Repositories;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService,
            ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (cityId <= 0)
                {
                    return BadRequest();
                }

                var cityExists = _cityInfoRepository.CityExists(cityId);

                if (!cityExists)
                {
                    _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest.");

                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);

                var results = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);

                return StatusCode(500, GlobalConstants.DefaultWebApiError);
            }            
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (cityId <= 0 || id <= 0)
            {
                return BadRequest();
            }

            var cityExists = _cityInfoRepository.CityExists(cityId);

            if (!cityExists)
            {
                _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest.");

                return NotFound();
            }

            var pointOfInterestForCity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestForCity == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<PointOfInterestDto>(pointOfInterestForCity);

            return Ok(result);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestDto pointOfInterest)
        {
            if (cityId <= 0 || pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "Description should be different from Name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cityExists = _cityInfoRepository.CityExists(cityId);

            if (!cityExists)
            {
                _logger.LogInformation($"City with id {cityId} was not found when creating point of interest.");

                return NotFound();
            }

            var newPointOfInterest = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, newPointOfInterest);

            bool success = _cityInfoRepository.Save();

            if (!success)
            {
                return StatusCode(500, GlobalConstants.DefaultWebApiError);
            }

            return CreatedAtRoute("GetPointOfInterest", 
                new { cityId = cityId, id = newPointOfInterest.Id }, newPointOfInterest);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, 
            [FromBody] PointOfInterestDto pointOfInterest)
        {
            if (cityId <= 0 || id <= 0 || pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "Description should be different from Name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cityExists = _cityInfoRepository.CityExists(cityId);

            if (!cityExists)
            {
                _logger.LogInformation($"City with id {cityId} was not found when updating point of interest.");

                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestEntity == null)
            {
                _logger.LogInformation($"Point of interest with id {id} for city with id {cityId} was not found when updating point of interest.");

                return NotFound();
            }

            Mapper.Map(pointOfInterest, pointOfInterestEntity);

            bool success = _cityInfoRepository.Save();

            if (!success)
            {
                return StatusCode(500, GlobalConstants.DefaultWebApiError);
            }

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, 
            [FromBody] JsonPatchDocument<PointOfInterestDto> patchDoc)
        {
            if (cityId <= 0 || id <= 0 || patchDoc == null)
            {
                return BadRequest();
            }

            var cityExists = _cityInfoRepository.CityExists(cityId);

            if (!cityExists)
            {
                _logger.LogInformation($"City with id {cityId} was not found when partially updating point of interest.");

                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestEntity == null)
            {
                _logger.LogInformation($"Point of interest with id {id} for city with id {cityId} was not found when updating point of interest.");

                return NotFound();
            }

            var pointOfInterestToPatch = Mapper.Map<PointOfInterestDto>(pointOfInterestEntity);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The Description should be different from the Name.");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            bool success = _cityInfoRepository.Save();

            if (!success)
            {
                return StatusCode(500, GlobalConstants.DefaultWebApiError);
            }

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (cityId <= 0 || id <= 0)
            {
                return BadRequest();
            }

            var cityExists = _cityInfoRepository.CityExists(cityId);

            if (!cityExists)
            {
                _logger.LogInformation($"City with id {cityId} was not found when deleting point of interest.");

                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestEntity == null)
            {
                _logger.LogInformation($"Point of interest with id {id} for city with id {cityId} was not found when updating point of interest.");

                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterestForCity(pointOfInterestEntity);

            bool success = _cityInfoRepository.Save();

            if (!success)
            {
                return StatusCode(500, GlobalConstants.DefaultWebApiError);
            }

            _mailService.Send("Point of interest deleted.", 
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
