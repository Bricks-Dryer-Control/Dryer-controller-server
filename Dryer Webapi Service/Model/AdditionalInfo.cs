using System;

namespace Dryer_Server.WebApi.Model
{
    public record AdditionalInfo
    {
        public AdditionalRoofInfo[] Roofs { get; set; }
        public AdditionalStatus[] Wents { get; set; }
    }
}