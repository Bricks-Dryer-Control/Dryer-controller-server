using System.Text.Json.Serialization;
using Dryer_Server.Interfaces;

namespace Dryer_Server.WebApi.Model
{
    public record ChamberStatus
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChamberConvertedStatus.WorkingStatus Working { get; set; }
        public bool IsAuto { get; set; }
        public int? QueuePosition { get; set; }
    }
}