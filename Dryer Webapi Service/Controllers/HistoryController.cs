using Microsoft.AspNetCore.Mvc;
using Dryer_Server.WebApi.Model;
using Dryer_Server.Interfaces;
using System;

namespace Dryer_Server.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HistoryController : ControllerBase
    {
        IMainController controller;

        public HistoryController(IMainController controller)
        {
            this.controller = controller;
        }

        [HttpGet]
        public HistoryResponse Get(int no, DateTime from, DateTime to)
        {
            return controller.GetHistory(no, from, to);
        }
    }
}
