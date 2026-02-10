using MOO.DAL.ToLive.Models;
using DAL = MOO.DAL.ToLive;
namespace OM_Lab.Data.Models

{
    public class LabChemPhys
    {
        public LabChemPhys(int ChemAnalysisId, int PhysAnalysisId) {
            var lcType = DAL.Services.Lab_Chem_TypeSvc.Get(ChemAnalysisId);
            var lpType = DAL.Services.Lab_Phys_TypeSvc.Get(PhysAnalysisId);
            Chem_Analysis.Lab_Chem_Type = lcType;
            Phys_Analysis.Lab_Phys_Type = lpType;

        }
        public LabChemPhys() { }


        public DateTime SampleDate { get { return Chem_Analysis.Analysis_Date; } set
            {
                Chem_Analysis.Analysis_Date = value;
                Phys_Analysis.Analysis_Date = value;

            }
        }

        public short? LineNumber
        {
            get { return Chem_Analysis.Line_Nbr; }
            set
            {
                Chem_Analysis.Line_Nbr = value;
                Phys_Analysis.Line_Nbr = value;
            }
        }

        public DAL.Models.Lab_Chem_Analysis Chem_Analysis { get; set; } = new();
        public DAL.Models.Lab_Phys_Analysis Phys_Analysis { get; set; } = new();
    }
}
