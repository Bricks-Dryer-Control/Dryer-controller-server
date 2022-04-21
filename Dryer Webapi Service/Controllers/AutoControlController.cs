using Dryer_Server.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoControl = Dryer_Server.WebApi.Model.AutoControl;

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
        public IEnumerable<string> GetControlsWithoutItems()
        {
            return persistence.GetControls().Select(c => c.Name);
        }

        [HttpGet]
        [Route("{name}")]
        public AutoControl GetControlWithItems(string name)
        {
            return persistence.Load(name);
        }

        [HttpDelete]
        [Route("{name}")]
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
                mainController.StartAutoControl(requestBody.ChamberId, requestBody.Name, requestBody.StartPoint);
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
        }

    }
}
