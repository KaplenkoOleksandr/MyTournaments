using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyTournaments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class playerteamController : ControllerBase
    {
        public class TestChartsController : ControllerBase
        {
            private readonly DBTournamentsContext _context;


        }
    }
}