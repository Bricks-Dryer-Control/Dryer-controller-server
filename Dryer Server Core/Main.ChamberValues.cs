namespace Dryer_Server.Core
{
    public partial class Main
    {
        private record ChamberValues
        {
            public int InFlow {get;set;}
            public int OutFlow {get;set;}
            public int ThroughFlow {get;set;}
            public int Special {get;set;}
        }
    }
}
