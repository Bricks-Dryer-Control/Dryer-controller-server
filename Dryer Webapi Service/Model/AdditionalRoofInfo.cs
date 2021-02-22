namespace Dryer_Server.WebApi.Model
{
    public record AdditionalRoofInfo
    {
        public AdditionalStatus through { get; set; }
        public AdditionalStatus roof { get; set; }
    }
}