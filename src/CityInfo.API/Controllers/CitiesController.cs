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
    public class CitiesController : Controller
    {
        private readonly IMapper _mapper;
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        ICityInfoRepository _cityInfoRepository;

        public CitiesController(
            IMapper mapper, 
            ILogger<PointsOfInterestController> logger, 
            IMailService mailService, 
            ICityInfoRepository cityInfoRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            try
            {
                var cities = _cityInfoRepository.GetCities();

                var results = _mapper.Map<IEnumerable<CityDto>>(cities);

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting cities.", ex);

                return StatusCode(500, GlobalConstants.DefaultWebApiError);
            }
        }

        [HttpGet("{id}", Name = "GetCity")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

                if (city == null)
                {
                    return NotFound();
                }

                var results = _mapper.Map<CityDto>(city);

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting city with id {id}.", ex);

                return StatusCode(500, GlobalConstants.DefaultWebApiError);
            }
        }

        [HttpGet("search", Name = "SearchCities")]
        public IActionResult SearchCities([FromQuery]int? page, [FromQuery]int? pageSize, [FromQuery]string searchText,
            [FromQuery]string sortField, [FromQuery]string sortDirection)
        {
            try
            {
                try
                {
                    var cities = _cityInfoRepository.GetCitiesPaged(page.GetValueOrDefault(1), pageSize.GetValueOrDefault(10), searchText, sortField, sortDirection);

                    var results = _mapper.Map<IEnumerable<CityDto>>(cities);

                    return Ok(results);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Exception while getting cities.", ex);

                    return StatusCode(500, GlobalConstants.DefaultWebApiError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while searching cities.", ex);

                return StatusCode(500, GlobalConstants.DefaultWebApiError);
            }
        }

        [HttpPost()]
        public IActionResult CreateCity([FromBody] CityDto city)
        {
            try
            {
                if (city == null)
                {
                    return BadRequest();
                }

                if (city.Description == city.Name)
                {
                    ModelState.AddModelError("Description", "Description should be different from Name.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newCity = _mapper.Map<Entities.City>(city);

                _cityInfoRepository.AddCity(newCity);

                bool success = _cityInfoRepository.Save();

                if (!success)
                {
                    return StatusCode(500, GlobalConstants.DefaultWebApiError);
                }

                return CreatedAtRoute("GetCity", new { id = newCity.Id }, newCity);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while creating new city.", ex);

                return StatusCode(500, GlobalConstants.DefaultWebApiError);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCity(int id, [FromBody] CityDto city)
        {
            if (id <= 0 || city == null)
            {
                return BadRequest();
            }

            if (city.Description == city.Name)
            {
                ModelState.AddModelError("Description", "Description should be different from Name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cityEntity = _cityInfoRepository.GetCity(id, false);

            if (cityEntity == null)
            {
                _logger.LogInformation($"City with id {id} was not found when updating city.");

                return NotFound();
            }

            _mapper.Map(city, cityEntity);

            bool success = _cityInfoRepository.Save();

            if (!success)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateCity(int id, [FromBody] JsonPatchDocument<CityDto> patchDoc)
        {
            if (id <= 0 || patchDoc == null)
            {
                return BadRequest();
            }

            var cityEntity = _cityInfoRepository.GetCity(id, false);

            if (cityEntity == null)
            {
                _logger.LogInformation($"City with id {id} was not found when partially updating city.");

                return NotFound();
            }

            var cityToPatch = _mapper.Map<CityDto>(cityEntity);

            patchDoc.ApplyTo(cityToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (cityToPatch.Description == cityToPatch.Name)
            {
                ModelState.AddModelError("Description", "The Description should be different from the Name.");
            }

            TryValidateModel(cityToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(cityToPatch, cityEntity);

            bool success = _cityInfoRepository.Save();

            if (!success)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCity(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var cityEntity = _cityInfoRepository.GetCity(id, false);

            if (cityEntity == null)
            {
                _logger.LogInformation($"City with id {id} was not found when deleting city.");

                return NotFound();
            }

            _cityInfoRepository.DeleteCity(cityEntity);

            bool success = _cityInfoRepository.Save();

            if (!success)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            _mailService.Send("City deleted.",
                $"City {cityEntity.Name} with id {cityEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
