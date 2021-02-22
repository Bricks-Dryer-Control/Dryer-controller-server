using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dryer_Server.WebApi.Model;
using Microsoft.AspNetCore.Cors;

namespace Dryer_Server.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommonController : ControllerBase
    {
        [HttpGet]
        public CommonStatus Get()
        {
            return new CommonStatus { WorkingNow = 1, InQueue = 2, Direction = false, TurnedOn = 20 };
        }

        [HttpPost]
        public CommonStatus Post(bool body)
        {
            return new CommonStatus { WorkingNow = 0, InQueue = 0, Direction = true, TurnedOn = 120 };
        }
    }
}
