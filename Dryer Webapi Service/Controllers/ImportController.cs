using Dryer_Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dryer_Server.WebApi.Controllers
{
    [ApiController]
    [Route("import")]
    public class ImportController : Controller
    {
        IProgramImporter programImporter;

        public ImportController(IProgramImporter programImporter)
        {
            this.programImporter = programImporter;
        }

        public record ImportItem
        {
            public string name;
            public string path;
        }

        [HttpPost]
        public ActionResult Post([FromBody]ImportItem item)
        {
            programImporter.Import(item.path, item.name);
            return Ok();
        }
    }
}
