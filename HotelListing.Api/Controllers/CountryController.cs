using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.IRepository;
using HotelListing.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetAll();
                var results = _mapper.Map<IList<CountryDTO>>(countries);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountries)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        //[Authorize]
        [HttpGet("{id:int}", Name = "GetCountry")]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                var country = await _unitOfWork.Countries.Get(p => p.Id == id, new List<string> { "Hotels" });
                var result = _mapper.Map<CountryDTO>(country);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountry)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        //[Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateCountry)}");
                return BadRequest(ModelState);
            }

            
            countryDTO.Name = char.ToUpper(countryDTO.Name[0]) + countryDTO.Name.Substring(1).ToLower();
            countryDTO.ShortName = countryDTO.ShortName.ToUpper();

            var existedCountry = await _unitOfWork.Countries.Get(p => p.Name == countryDTO.Name &&
                                                                      p.ShortName == countryDTO.ShortName);

            if (existedCountry == null)
            {
                try
                {
                    var country = _mapper.Map<Country>(countryDTO);
                    await _unitOfWork.Countries.Insert(country);
                    await _unitOfWork.Save();
                    return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Something went wrong in {nameof(CreateCountry)}");
                    return StatusCode(500, "Internal Server Error. Please try again.");

                }
            }
            else
            {
                return BadRequest("This Country already exists with same Short Name");
            }
        }

        //[Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid || id < 1 )
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                var country = await _unitOfWork.Countries.Get(s => s.Id == id);
                if (country == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountry)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(countryDTO, country);
                _unitOfWork.Countries.Update(country);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in {nameof(UpdateCountry)}");
                return StatusCode(500, "Internal Server Error. Please try again.");

            }
        }

        //[Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
                return BadRequest();
            }


            try
            {
                var country = await _unitOfWork.Countries.Get(s => s.Id == id);
                if (country == null)
                {
                    _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
                    return BadRequest("Submitted data is invalid");
                }

                var hotels = await _unitOfWork.Hotels.Get(s => s.CountryId == id);
                if (hotels != null)
                {
                    _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
                    return BadRequest($"There are registered Hotels in {country.Name}");
                }

                await _unitOfWork.Countries.Delete(country.Id);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(DeleteCountry)}");
                return StatusCode(500, "Internal Server Error. Please try again.");
            }
        }
    }
}
