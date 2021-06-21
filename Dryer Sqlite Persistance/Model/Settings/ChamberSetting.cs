using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Settings
{
    [Table("Chamber")]
    public record ChamberSetting : ChamberConfiguration
    {
        public DateTime CreationTimeUtc { get; set; }

        public ChamberSetting()
        { }
        
        public ChamberSetting(ChamberConfiguration other): base(other)
        {
            CreationTimeUtc = DateTime.UtcNow;
        }
    }
}