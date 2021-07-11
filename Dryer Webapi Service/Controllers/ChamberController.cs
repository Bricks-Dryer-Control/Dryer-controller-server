using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dryer_Server.WebApi.Model;
using Dryer_Server.Interfaces;

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

        [HttpPost]
        [Route("{no}/Set")]
        public ChamberInfo Set(int no, ChamberValues newSets)
        {
            controller.ChangeActuators(no, newSets.InFlow, newSets.OutFlow, newSets.ThroughFlow);
            return data.GetChamber(no);
        }

        [HttpPost]
        [Route("{no}/Turn")]
        public ChamberInfo Turn(int no, [FromBody] bool value)
        {
            controller.ChangeChamberReading(no, value);
            return data.GetChamber(no);
        }
    }
}
