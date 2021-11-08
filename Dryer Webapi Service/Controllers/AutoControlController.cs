using Dryer_Server.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dryer_Server.WebApi.Controllers
{
    [ApiController]
    [Route("auto")]
    public class AutoControlController : Controller
    {
        private readonly IAutoControlPersistance persistence;

        public AutoControlController(IAutoControlPersistance persistence)
        {
            this.persistence = persistence;
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
        public ObjectResult CreateAutoControl(AutoControl autoControl)
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
        [Route("start/{name}")]
        public void StartAutoControl(string name)
        {
            throw new NotImplementedException();
        }

    }
}
