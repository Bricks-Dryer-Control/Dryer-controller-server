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
    public class ChamberController : ControllerBase
    {
        static Random rnd = new Random();

        [HttpGet]
        public IEnumerable<ChamberInfo> Get()
        {
            var time = DateTime.UtcNow;

            var infos = Enumerable.Range(1, 20).Select(i => new ChamberInfo {
                No = i,
                Humidity = rnd.Next(0, 101),
                Temperature = rnd.Next(-20, 101),
                ReadingTime = (time += new TimeSpan(0, 0, 2)),
                ActualActuators = new ChamberValues { InFlow = rnd.Next(0, 481), OutFlow = rnd.Next(0, 481), ThroughFlow = rnd.Next(0, 481) },
                SetActuators = new ChamberValues { InFlow = rnd.Next(0, 481), OutFlow = rnd.Next(0, 481), ThroughFlow = rnd.Next(0, 481) },
                Status = new ChamberStatus { IsAuto = rnd.Next() % 2 == 0, QueuePosition = i % 5 == 0 ? null : i, Working = (ChamberStatus.WorkingStatus)(rnd.Next()%5)}
            });

            return infos;
        }

        [HttpGet]
        [Route("{no}")]
        public ChamberInfo Get(int no)
        {
            var time = DateTime.UtcNow;

            return  new ChamberInfo {
                No = no,
                Humidity = rnd.Next(0, 101),
                Temperature = rnd.Next(-20, 101),
                ReadingTime = time,
                ActualActuators = new ChamberValues { InFlow = rnd.Next(0, 481), OutFlow = rnd.Next(0, 481), ThroughFlow = rnd.Next(0, 481) },
                SetActuators = new ChamberValues { InFlow = rnd.Next(0, 481), OutFlow = rnd.Next(0, 481), ThroughFlow = rnd.Next(0, 481) },
                Status = new ChamberStatus { IsAuto = rnd.Next() % 2 == 0, QueuePosition = null, Working = (ChamberStatus.WorkingStatus)(rnd.Next()%5)}
            };
        }

        public record PostRequest
        {
            public bool IsOn {get;set;}
            public ChamberValues NewSets {get;set;}
        }

        [HttpPost]
        [Route("{no}")]
        public ChamberInfo Post(int no, PostRequest body)
        {
            var time = DateTime.UtcNow;

            return new ChamberInfo {
                No = no,
                Humidity = rnd.Next(0, 101),
                Temperature = rnd.Next(-20, 101),
                ReadingTime = time,
                ActualActuators = new ChamberValues { InFlow = rnd.Next(0, 481), OutFlow = rnd.Next(0, 481), ThroughFlow = rnd.Next(0, 481) },
                SetActuators = body.NewSets ?? new ChamberValues { InFlow = rnd.Next(0, 481), OutFlow = rnd.Next(0, 481), ThroughFlow = rnd.Next(0, 481) },
                Status = new ChamberStatus { IsAuto = body.IsOn, QueuePosition = null, Working = (ChamberStatus.WorkingStatus)(rnd.Next()%5)}
            };
        }
    }
}
