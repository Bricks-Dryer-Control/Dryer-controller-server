using Microsoft.AspNetCore.Mvc;
using Dryer_Server.WebApi.Model;
using Dryer_Server.Interfaces;

namespace Dryer_Server.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommonController : ControllerBase
    {
        IUiDataKeeper data; 
        IMainController controller;

        public CommonController(IUiDataKeeper data, IMainController controller)
        {
            this.data = data;
            this.controller = controller;
        }

        [HttpGet]
        public CommonStatus Get()
        {
            return controller.GetCommon();
        }

        [HttpPost]
        public CommonStatus Post(bool body)
        {
            controller.StopAll();
            return controller.GetCommon();
        }
    }
}
