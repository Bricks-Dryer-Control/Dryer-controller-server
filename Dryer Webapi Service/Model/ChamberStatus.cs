using System.Text.Json.Serialization;

namespace Dryer_Server.WebApi.Model
{
    public record ChamberStatus
    {
        public enum WorkingStatus
        {
            off, waiting, queued, working, addon
        };
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WorkingStatus Working { get; set; }
        public bool IsAuto { get; set; }
        public int? QueuePosition { get; set; }
    }
}