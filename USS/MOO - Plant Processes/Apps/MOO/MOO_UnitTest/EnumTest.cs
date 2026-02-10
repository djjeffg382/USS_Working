using Xunit;
using MOO.Enums.Extension;
using System.ComponentModel.DataAnnotations;
namespace MOO_UnitTest
{
    public class EnumTest
    {


        [Fact]
        public void TestEnum()
        {
            var zz = EnumExtension.GetEnumByDescription<MOO.Plant>("Minntac");
            MOO.MinntacAreas aa = MOO.MinntacAreas.AggStep3;
            var xx = aa.GetDescription();
            Assert.NotNull(xx);
            var display = aa.GetDisplay().Name;
            Assert.NotNull(display);

            var displayAttribute = aa.GetAttribute<DisplayAttribute>();
            Assert.NotNull(displayAttribute);


            var bb = EnumExtension.GetDescriptions<MOO.MinntacAreas>();
            Assert.NotNull(bb);

            //test when we set an enum value to an enum that doesn't exist
            MOO.MinntacAreas? nonExistEnum = (MOO.MinntacAreas)200;
            Assert.NotNull(nonExistEnum);
            display = nonExistEnum.GetDisplay()?.Name;

            Assert.Null(display);


           // var a = 0;
        }
    }
}
