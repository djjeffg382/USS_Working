using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class KTCBlastTest : BaseTestClass
    {
        [Fact]
        public void TestDefaults()
        {
            var dft = DAL.Services.KTC_Blast_Unit_PriceSvc.Get();
            Assert.NotNull(dft);
            var r = new Random();
            string randTestVal = "test" + r.Next();
            dft.Blasters = randTestVal;
            DAL.Services.KTC_Blast_Unit_PriceSvc.Update(dft);
            dft = DAL.Services.KTC_Blast_Unit_PriceSvc.Get();
            Assert.Equal(randTestVal, dft.Blasters);
        }

        [Fact]
        public void TestKTCBlast()
        {
            List<DAL.Models.KTC_Blast> blastList = DAL.Services.KTC_BlastSvc.GetAll();
            Assert.NotNull(blastList);
            var r = new Random();
            string randTestVal1 = "test" + r.Next();
            DateTime date = DateTime.Now;
            string benchNumber = "test12435";
            string blastNumber = "blasttest12435";
            DAL.Models.KTC_Blast blast = new() { Additional_Db=2, 
                Area_Deep=5, 
                Additional_Db_Site="test", 
                Area_Shallow=5, 
                Bench_Number= benchNumber, 
                A_Chord_Ft=5, 
                A_Chord_Ft_Uc=5, 
                Blasted_Date= DateTime.Today, 
                Blasted_Deep_Gt=5, 
                Blasted_Shallow_Gt=5, Blasted_Time=5, Blasters="blaster", 
                Blasting_Wire_Ft=5, 
                Blasting_Wire_Ft_Uc=5, 
                Blast_Number=blastNumber, 
                Burden_And_Spacing="burden", 
                Caps_6ft=5,
                Caps_6ft_Uc=5, 
                Cc="cc", 
                Depth_Deep_Avg_Ft=5, 
                Depth_Shallow_Avg=5, 
                Drill="drill", 
                Electric_Det_15_Met=5, 
                Electric_Det_15_Met_Uc=5, 
                Electric_Det_20_Met=5, 
                Electric_Det_20_Met_Uc=5, 
                Engineer="engineer", 
                Explosive1_Lb=5, 
                Explosive1_Uc=5, 
                Explosive2_Lb=5, 
                Explosive2_Uc=5,
                Ez_Trunkline_40ft=5, 
                Ez_Trunkline_40ft_Uc=5, 
                Ez_Trunkline_60ft=5, 
                Ez_Trunkline_60ft_Uc=5, 
                Forman="forman", 
                Full_Column_Load="column", 
                Holes=5, 
                Hole_Size_In=5, 
                M35_Bus_Line_Ft=5, 
                M35_Bus_Line_Ft_Uc=5, 
                Material="material", 
                Noise_North_Ktc_Db=5, 
                Noise_West_Ktc_Db=5, 
                Primadets_30ft=5, 
                Primadets_30ft_Uc=5, 
                Primadets_40ft=5, 
                Primadets_40ft_Uc=5, 
                Primadets_50ft=5, 
                Primadets_50ft_Uc=5, 
                Primer_1lb= 5, 
                Primer_1lb_Uc=5, 
                Property_Location="property", 
                Stemming=5, 
                Subgrade_Ft="0'", 
                Survey= randTestVal1, 
                Total_Ft=5, 
                Total_Sub_Ft=5
        };
            DAL.Services.KTC_BlastSvc.Delete(blast);
            DAL.Services.KTC_BlastSvc.Insert(blast);
            blast = DAL.Services.KTC_BlastSvc.Get(benchNumber, blastNumber);
            Assert.Equal(blast.Survey, randTestVal1);
            var r2 = new Random();
            string randTestVal2 = "test" + r.Next();
            blast.Property_Location = randTestVal2;
            blast.Blasted_Date = DateTime.Today.AddDays(10);
            DAL.Services.KTC_BlastSvc.Update(blast);
            blast = DAL.Services.KTC_BlastSvc.Get(benchNumber, blastNumber);
            Assert.Equal(blast.Property_Location, randTestVal2);
        }
    }
}
