using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dryer_Server.Persistance.Model.Historical
{
    public record AutoControlStage
    {
        [Key, Column(Order = 0)]
        public int ParentId { get; set; }
        [Key, Column(Order = 1)]
        public int Order { get; set; }
        public int AutoControlInfoId { get; set; }
        public virtual ChamberControl parent { get; set; }
    }
}