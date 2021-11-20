using Dryer_Server.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Dryer_Server.AutomaticControl;

namespace Dryer_Server.WebApi.Controllers
{
    [ApiController]
    [Route("auto")]
    public class AutoControlController : Controller
    {
        private readonly IAutoControlPersistance persistence;
        private readonly IMainController mainController;

        public AutoControlController(IAutoControlPersistance persistence, IMainController mainController)
        {
            this.persistence = persistence;
            this.mainController = mainController;
        }

        [HttpGet]
        public IEnumerable<AutoControl> GetControlsWithoutItems()
        {
            return persistence.GetControls().ToList();
        }

        [HttpGet]
        [Route("{name}")]
        public AutoControl GetControlWithItems(string name)
        {
            return persistence.GetControlWithItems(name);
        }

        [HttpDelete]
        public void DeleteByName(string name)
        {
            persistence.Delete(name);
        }

        [HttpPost]
        public ObjectResult CreateAutoControl([FromBody] AutoControl autoControl)
        {
            try
            {
                persistence.SaveDeactivateLatest(autoControl);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message);
            }
            return Ok(autoControl);
        }

        [HttpPost]
        [Route("start")]
        public ObjectResult StartAutoControl([FromBody] StartAutoControlRequestBody requestBody)
        {
            try
            {
                if (!mainController.TryGetChamber(requestBody.ChamberId, out var chamber))
                {
                    return BadRequest("No such chamber.");
                }
                var control = persistence.GetControlWithItems(requestBody.Name);
                var autoControl = new TimeBasedAutoControl(requestBody.CheckingDelay, requestBody.StartPoint, control, chamber);
                return Ok(null);
            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message);
            }
        }

        public record StartAutoControlRequestBody
        {
            public string Name { get; set; }
            public int ChamberId { get; set; }
            public TimeSpan StartPoint { get; set; }
            public TimeSpan CheckingDelay { get; set; }
        }

    }
}
