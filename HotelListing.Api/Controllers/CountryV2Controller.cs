using HotelListing.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Api.Controllers
{
    [ApiVersion("2.0")]  //  when deprecated is set true "api-deprecated-versions" appears in the header at the route. It is set to false by default.
    //[Route("api/v{v:apiversion}/Country/[action]")]
    [Route("api/Country/[action]")]
    [ApiController]
    public class CountryV2Controller : ControllerBase
    {
        private DatabaseContext _context;

        public CountryV2Controller(DatabaseContext context)
        {
            _context = context;
        }

        //[Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCountries()
        {
            return Ok(_context.Countries);
        }
    }
}
