using MOO.DAL.Blast.Models;
using MOO.DAL.Blast.Services;
using MOO.DAL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Blast;

namespace DAL_UnitTest.Blast
{
    public class BlastTest : BaseTestClass
    {
        [Fact]
        public void TestBlast()
        {
            var d = BlastSvc.GetDrills(62);
            Assert.NotEmpty(d);
            var b = BlastSvc.Get(155);
            Assert.NotNull(b);
            var list = BlastSvc.GetByDateRange(DateTime.Now.AddYears(-8).AddDays(-5), DateTime.Now.AddYears(-8));
            Assert.NotEmpty(list);

            var p = PatternSvc.GetAll();
            Assert.NotEmpty(p);

            //Must have a new pattern item with a new ID to be able to insert a new blast item
            //There is only one unique pattern for every blast item
            //Therefore, a temporary pattern item is created with a unique ID below
            var patt = new Pattern()
            {
                Id = 5005
            };
            PatternSvc.Insert(patt);

            //Then, the new blast item will use the new pattern item for the pattern id
            var blast = new DAL.Models.Blast()
            {
                Id = 5005,
                Pattern = new Pattern()
                {
                    Id = 5005,
                },
                Blast_Meeting_Time = DateTime.Now,
                Comments = "Testing",
                Subdrill = 0,
                Pit = new CAT_Pits()
                {
                    Id = 2,
                },
                Rock_Type = new CAT_Rock_Type()
                {
                    Id = 2,
                },
                Open_Sinking = DAL.Enums.Open_Sinking_Type.O
            };
            BlastSvc.Insert(blast);
            blast.Comments = "Test Test Test";
            BlastSvc.Update(blast);
            BlastSvc.Delete(blast);
            PatternSvc.Delete(patt);
        }
    }
}
