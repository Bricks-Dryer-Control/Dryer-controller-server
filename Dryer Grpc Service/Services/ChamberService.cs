using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dryer_Grpc_Service
{
    public class ChamberService : Chamber.ChamberBase
    {
        public override Task<ChamberInfos> CheckAllChambers(Empty request, ServerCallContext context)
        {
            var time = Timestamp.FromDateTime(DateTime.UtcNow);

            var infos = Enumerable.Range(1, 20).Select(i => new ChamberInfo {
                No = i,
                Humidity = i,
                Temperature = i,
                ReadingTime = time,
                ActualActuators = new ChamberValues { InFlow = i, OutFlow = i, ThroughFlow = i },
                SetActuators = new ChamberValues { InFlow = 480 - i, OutFlow = 480 - i, ThroughFlow = 480 - i },
                Status = new ChamberStatus { IsAuto = i % 2 == 0, QueuePosition = i % 5 == 0 ? null : i, WorkingStatus = (ChamberStatus.Types.WorkingStatus)(i%5)}
            });

            var result = new ChamberInfos();
            result.Results.AddRange(infos);

            return Task.FromResult(result);
        }

        public override Task<ChamberInfo> CheckChamber(CheckChamberRequest request, ServerCallContext context)
        {
            var no = request.No;
            return Task.FromResult(new ChamberInfo());
        }
    }
}
