using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dryer_Server.WebApi.Model;
using Dryer_Server.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace Dryer_Server.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChamberController : ControllerBase
    {
        IUiDataKeeper data; 
        IMainController controller;

        public ChamberController(IUiDataKeeper data, IMainController controller)
        {
            this.data = data;
            this.controller = controller;
        }

        static Random rnd = new Random();

        [HttpGet]
        public IEnumerable<ChamberInfo> Get()
        {
            return data.GetChambers();
        }

        [HttpGet]
        [Route("{no}")]
        public ChamberInfo Get(int no)
        {
            return data.GetChamber(no);
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
            controller.ChangeActuators(no, body.NewSets.InFlow, body.NewSets.OutFlow, body.NewSets.ThroughFlow);
        }
    }
}
