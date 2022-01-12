using Dryer_Server.Interfaces;
using Dryer_Server.Persistance.Model.AutoControl;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Webapi_Service_Tests
{
    public class AutoControlControllerTest
    {
        protected DbContextOptions<AutoControlContext> ContextOptions { get; }
        protected List<AutoControl> existingAutoControls { get; } = new List<AutoControl>();
        protected AutoControlControllerTest(DbContextOptions<AutoControlContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        private void Seed()
        {
            using var context = new AutoControlContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.AddRange(CreateAutoControls());

            context.SaveChanges();
        }

        private IEnumerable<object> CreateAutoControls()
        {
            var ac1 = new AutoControl()
            {
                ControlDifference = 3,
                Ki = 1,
                ControlType = AutoControlType.PI,
                Kp = 1,
                MaxInFlow = 1,
                MaxOutFlow = 1,
                MinInFlow = 1,
                MinOutFlow = 1,
                Name = "ctrl1",
                Offset = 2,
                Percent = 3,
                TemperatureDifference = 3.0f,
                TimeToSet = new TimeSpan(2,3,4,5),
                Sets = new List<AutoControlItem>()
                {
                    new()
                    {
                        InFlow = 1,
                        OutFlow = 1,
                        Temperature = 1f,
                        ThroughFlow = 3,
                        Time = new TimeSpan(3,4,5)
                    },
                    new()
                    {
                        InFlow = 2,
                        OutFlow = 2,
                        Temperature = 2f,
                        ThroughFlow = 4,
                        Time = new TimeSpan(4,5,6)
                    }
                }
            };
            var ac2= new AutoControl()
            {
                ControlDifference = 3,
                Ki = 1,
                ControlType = AutoControlType.PI,
                Kp = 1,
                MaxInFlow = 1,
                MaxOutFlow = 1,
                MinInFlow = 1,
                MinOutFlow = 1,
                Name = "ctrl2",
                Offset = 2,
                Percent = 3,
                TemperatureDifference = 3.0f,
                TimeToSet = new TimeSpan(2, 3, 4, 5),
                Sets = new List<AutoControlItem>()
                {
                    new()
                    {
                        InFlow = 1,
                        OutFlow = 1,
                        Temperature = 1f,
                        ThroughFlow = 3,
                        Time = new TimeSpan(3,4,5)
                    }
                }
            };
            var dbac1 = new DbAutoControl(ac1);
            var dbac2 = new DbAutoControl(ac2);
            var dbac3 = new DbAutoControl(ac1);
            dbac3.Deleted = true;
            dbac3.ControlDifference = 5;
            existingAutoControls.Add(ac1);
            existingAutoControls.Add(ac2);

            return new List<object> {dbac1, dbac2, dbac3};
        }
    }
}
