using System;
using System.IO;
using System.Collections.Generic;
using Dryer_Server.Interfaces;

namespace Dryer_OldProgram_Importer
{
    public class ProgramImporter: IProgramImporter
    {
        IAutoControlPersistance persister;

        public ProgramImporter(IAutoControlPersistance persister)
        {
            this.persister = persister;
        }

        public void Import(string filePath, string name)
        {
            var items = new List<AutoControlItem>();
            using (var fileContent = File.OpenText(filePath))
            {
                string line;
                while((line = fileContent.ReadLine()) != null)
                {
                    if (TryGetAotControlItem(line, out var item))
                        items.Add(item);
                }
            }

            var additionalPath = Path.Combine(filePath + "_dod", "dodatki");
            using (var addonsFile = File.OpenText(additionalPath))
            {
                var line = addonsFile.ReadLine();
                var values = line.Split('\t');
                if (values.Length == 12
                    && float.TryParse(values[0], out var setTime)
                    && float.TryParse(values[0], out var tempDiff)
                    && float.TryParse(values[0], out var controlDiff)
                    && float.TryParse(values[0], out var controlType)
                    && float.TryParse(values[0], out var kp)
                    && float.TryParse(values[0], out var ki)
                    && float.TryParse(values[0], out var minInFlow)
                    && float.TryParse(values[0], out var maxInFlow)
                    && float.TryParse(values[0], out var minOutFlow)
                    && float.TryParse(values[0], out var maxOutFlow)
                    && float.TryParse(values[0], out var percent)
                    && float.TryParse(values[0], out var offset))
                {
                    var autoControl = new AutoControl
                    {
                        Name = name,
                        TimeToSet = TimeSpan.FromSeconds(setTime),
                        TemperatureDifference = tempDiff,
                        ControlDifference = (int)controlDiff,
                        ControlType = (AutoControlType)((int)controlType),
                        Kp = kp,
                        Ki = ki,
                        MinInFlow = (int)minInFlow,
                        MaxInFlow = (int)maxInFlow,
                        MinOutFlow = (int)minOutFlow,
                        MaxOutFlow = (int)maxOutFlow,
                        Percent = percent,
                        Offset = (int)offset,
                        Sets = items,
                    };

                    persister.SaveDeactivateLatest(autoControl);
                }
            }
        }

        private bool TryGetAotControlItem(string line, out AutoControlItem item)
        {

            var values = line.Split('\t');
            if (values.Length == 5
                && int.TryParse(values[0], out var seconds)
                && int.TryParse(values[1], out var inFlow)
                && int.TryParse(values[2], out var outFlow)
                && int.TryParse(values[3], out var throuhFlow)
                && int.TryParse(values[4], out var temperature)
                )
            {
                item = new AutoControlItem
                {
                    Time = TimeSpan.FromSeconds(seconds),
                    InFlow = inFlow,
                    OutFlow = outFlow,
                    ThroughFlow = throuhFlow,
                    Temperature = temperature,
                };
                return true;
            }

            item = null;
            return false;
        }
    }
}
