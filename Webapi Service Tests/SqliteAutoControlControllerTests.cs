using Dryer_Server.Interfaces;
using Dryer_Server.Persistance;
using Dryer_Server.Persistance.Model.AutoControl;
using Dryer_Server.Persistance.Model.Historical;
using Dryer_Server.Persistance.Model.Settings;
using Dryer_Server.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Dryer_Sqlite.Persistance.Model.AutoControl;
using Xunit;

namespace Webapi_Service_Tests
{
    public class SqliteAutoControlControllerTests : AutoControlControllerTest
    {
        private readonly IAutoControlPersistance persistence;
        private readonly IMainController mainController;
        public SqliteAutoControlControllerTests() :
            base(new DbContextOptionsBuilder<AutoControlContext>().UseSqlite("Filename=Test.db").Options)
        {
            persistence= new SqlitePersistanceManager(ContextOptions,
                new DbContextOptions<HistoricalContext>(), new DbContextOptions<SettingsContext>(), new DbContextOptions<TimeBasedAutoControlContext>());
            
        }


        [Fact]
        public void GetControlsWithoutItems()
        {
            var controller = new AutoControlController(persistence, mainController);

            var result=controller.GetControlsWithoutItems().ToList();

            Assert.Equal(2,result.Count);
            Assert.Contains("ctrl1", result.Select(ctrl=>ctrl.Name));
            Assert.Contains("ctrl2", result.Select(ctrl => ctrl.Name));
            Assert.Empty(result.SelectMany(ctrl=>ctrl.Sets));
        }


        [Fact]
        public void GetControlsWithItems()
        {
            var controller = new AutoControlController(persistence, mainController);

            var result = controller.GetControlWithItems("ctrl1");

            Assert.Equal("ctrl1", result.Name);
            Assert.Equal(2,result.Sets.Count);
        }

        [Fact]
        public void DeleteByName()
        {
            var controller = new AutoControlController(persistence, mainController);

            controller.DeleteByName("ctrl1");

            using var context = new AutoControlContext(ContextOptions);
            Assert.NotEmpty(context.Definitions.Where(ctrl => ctrl.Name == "ctrl1"));
            Assert.Empty(context.Definitions.Where(ctrl => ctrl.Name == "ctrl1" && ctrl.Deleted==false));
        }

        [Fact]
        public void CreateAutoControlShouldDeactivateExistingWithTheSameName()
        {
            var controller = new AutoControlController(persistence,mainController);
            var ac = existingAutoControls[0];

            controller.CreateAutoControl(ac);

            using var context = new AutoControlContext(ContextOptions);
            Assert.Equal(3,context.Definitions.Count(ctrl => ctrl.Name == "ctrl1"));
            Assert.Equal(1, context.Definitions.Count(ctrl => ctrl.Name == "ctrl1" && ctrl.Deleted == false));
        }

        [Fact]
        public void CreateAutoControl()
        {
            var controller = new AutoControlController(persistence, mainController);
            var ac = new AutoControl()
            {
                Name = "test"
            };


            controller.CreateAutoControl(ac);

            using var context = new AutoControlContext(ContextOptions);
            Assert.Contains(ac.Name, context.Definitions.Select(i=>i.Name));
        }

    }
}
