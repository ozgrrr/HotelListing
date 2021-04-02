using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.IRepository;
using HotelListing.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Api.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _unitOfWork.Hotels.GetAll();
            var results = _mapper.Map<IList<HotelDTO>>(hotels);
            return Ok(results);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels([FromQuery] RequestParams requestParams)
        {
            var hotels = await _unitOfWork.Hotels.GetPagedList(requestParams);
            var results = _mapper.Map<IList<HotelDTO>>(hotels);
            return Ok(results);
        }

        //[Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotelsWithCountry()
        {
            var countries = await _unitOfWork.Hotels.GetAll(null, null, new List<string> { "Country" });
            var results = _mapper.Map<IList<CountryDTO>>(countries);
            return Ok(results);
        }

        //[Authorize]
        //  the Name parameter in the following annotation is to make this method to be able to be called with its name
        //  otherwise, it could only be called by its specified route.
        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            var hotel = await _unitOfWork.Hotels.Get(s => s.Id == id, new List<string> { "Country" });
            var result = _mapper.Map<HotelDTO>(hotel);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }

            var hotel = _mapper.Map<Hotel>(hotelDTO);
            await _unitOfWork.Hotels.Insert(hotel);
            await _unitOfWork.Save();

            //  it returns the result of GetHotel method by calling it by its name so that we can see the recently added hotel.
            return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
        }

        //[Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }

            var hotel = await _unitOfWork.Hotels.Get(s => s.Id == id);
            if(hotel == null)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
                return BadRequest("Submitted data is invalid");
            }

            //hotelDTO.Name = string.IsNullOrEmpty(hotelDTO.Name) ? hotel.Name : hotelDTO.Name;
            //hotelDTO.Address = string.IsNullOrEmpty(hotelDTO.Name) ? hotel.Name : hotelDTO.Name;
            //hotelDTO.Rating = double.IsNaN(hotelDTO.Rating) ? hotel.Rating
            //                : double.IsNegative(hotelDTO.Rating) ? hotel.Rating
            //                : hotelDTO.Rating;
            //hotelDTO.CountryId = hotelDTO.CountryId > 0 ? hotelDTO.CountryId : hotel.CountryId;

            _mapper.Map(hotelDTO, hotel);
            _unitOfWork.Hotels.Update(hotel);
            await _unitOfWork.Save();

            return NoContent();
        }

        //[Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
                return BadRequest();
            }

            var hotel = await _unitOfWork.Hotels.Get(s => s.Id == id);
            if (hotel == null)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(DeleteHotel)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Hotels.Delete(hotel.Id);
            await _unitOfWork.Save();

            return Ok();
        }
    }
}
