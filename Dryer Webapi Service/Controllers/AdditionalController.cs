using Microsoft.AspNetCore.Mvc;
using Dryer_Server.WebApi.Model;

namespace Dryer_Server.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdditionalController : ControllerBase
    {
        [HttpGet]
        public AdditionalInfo Get()
        {
            return new AdditionalInfo
            {
                Roofs = new AdditionalRoofInfo[]
                {
                    new AdditionalRoofInfo
                    {
                        roof = new AdditionalStatus {actualValue = 10, setValue = 10, status = new ChamberStatus{IsAuto=false, QueuePosition=null, Working=ChamberStatus.WorkingStatus.addon}},
                        through = new AdditionalStatus {actualValue = 10, setValue = 10, status = new ChamberStatus{IsAuto=false, QueuePosition=null, Working=ChamberStatus.WorkingStatus.addon}},
                    },
                    new AdditionalRoofInfo
                    {
                        roof = new AdditionalStatus {actualValue = 10, setValue = 10, status = new ChamberStatus{IsAuto=false, QueuePosition=null, Working=ChamberStatus.WorkingStatus.addon}},
                        through = new AdditionalStatus {actualValue = 10, setValue = 10, status = new ChamberStatus{IsAuto=false, QueuePosition=null, Working=ChamberStatus.WorkingStatus.addon}},
                    },
                },
                Wents = new AdditionalStatus[]
                {
                    new AdditionalStatus
                    {
                        actualValue = 13,
                        setValue = 100,
                        status = new ChamberStatus{IsAuto=false, Working = ChamberStatus.WorkingStatus.queued, QueuePosition = 12}
                    }
                }
            };
        }

        [HttpPost]
        [Route("Went/{no}")]
        public AdditionalInfo Went(int no, int value)
        {
            var x = Get();
            x.Wents[no - 1].setValue = value;
            return x;
        }

        [HttpPost]
        [Route("Roof/{no}")]
        public AdditionalInfo Roof(int no, bool roof)
        {
            var x = Get();
            x.Roofs[no - 1].roof.setValue = 480;
            return x;
        }
    }
}
