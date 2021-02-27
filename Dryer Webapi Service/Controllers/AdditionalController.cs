﻿using Microsoft.AspNetCore.Mvc;
using Dryer_Server.WebApi.Model;
using Dryer_Server.Interfaces;

namespace Dryer_Server.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdditionalController : ControllerBase
    {
        IUiDataKeeper data; 
        IMainController controller;

        public AdditionalController(IUiDataKeeper data, IMainController controller)
        {
            this.data = data;
            this.controller = controller;
        }

        [HttpGet]
        public AdditionalInfo Get()
        {
            return data.GetAdditionalInfo();
        }

        [HttpPost]
        [Route("Went/{no}")]
        public AdditionalInfo Went(int no, int value)
        {
            controller.ChangeWent(no, value);
            return data.GetAdditionalInfo();
        }

        [HttpPost]
        [Route("Roof/{no}")]
        public AdditionalInfo Roof(int no, bool roof)
        {
            controller.ChangeRoof(no, roof);
            return data.GetAdditionalInfo();
        }
    }
}
