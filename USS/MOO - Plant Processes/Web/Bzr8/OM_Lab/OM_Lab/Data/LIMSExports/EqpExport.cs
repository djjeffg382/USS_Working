using System.IO;
using DAL = MOO.DAL.ToLive;
namespace OM_Lab.Data.LIMSExports
{
    internal abstract class EqpExport
    {

        /*Process for creating a new export:
         * 1. Create the export class in the Data/LIMSExports forlder
         * 2. Inherit EqpExport
         * 3. Implement Test_Name property to match the name of the Analyses in LIMS
         * 4. Override the Machine_Name property if needed (Test name doesn't match the machine name, used for ICP1 and ICP2)
         * 4. Implement the Export function
         * 5. Add section to AppSettings.JSON file under LabExportLocation using the value used in Machine_Name if overridden or Test_Name if not overridden
         * 6. Add creation of the new class you made to the GetAllExports function in EqpExport
         * 
         */
        private readonly IConfiguration Config;
        protected EqpExport(IConfiguration Config)
        {
            this.Config = Config;
        }


        /// <summary>
        /// Informational log of the last export call.  Can be used to display errors etc.
        /// </summary>
        public string ExportLog { get; set; } = "";


        protected void SetupDirectoryStructure()
        {
            try
            {
                if (!Directory.Exists(ExportRoot))
                    Directory.CreateDirectory(ExportRoot);
                //create directories for each of the batch types
                foreach (var batchType in Enum.GetNames(typeof(DAL.Enums.LIMS_Batch_Type)))
                {
                    if (!Directory.Exists($"{ExportRoot}\\{batchType}"))
                        Directory.CreateDirectory($"{ExportRoot}\\{batchType}");
                }
            }
            catch (Exception ex)
            {
                string msg = $"Error creating folder structure for the LIMS Lab Export {Test_Name}.  Error: {ex.Message}";
                MOO.Exceptions.ErrorLog.LogError("BZR/OM_LAB", msg, ex.StackTrace, "", MOO.Exceptions.ErrorLog.ErrorLogType.Crash);
            }
        }

        /// <summary>
        /// Gets the samples form the batch sample list that are for this test
        /// </summary>
        /// <param name="Samples"></param>
        /// <returns></returns>
        protected List<DAL.Models.LIMS_Batch_Samples> GetSamplesForThisTest(List<DAL.Models.LIMS_Batch_Samples> Samples)
        {
            List<DAL.Models.LIMS_Batch_Samples> expSamples = Samples.FindAll(x => x.Sample.Tests.Contains(Test_Name));
            return expSamples;
        }


        protected string ExportRoot { get { return Config.GetValue<string>($"LabExportLocation:{Machine_Name}"); } }

        /// <summary>
        /// The name of the test, used to get export location from appsettings
        /// </summary>
        public abstract string Test_Name
        {
            get;
        }
        /// <summary>
        /// The machine name if different than test name, used to get export location from appsettings
        /// </summary>
        public virtual string Machine_Name => Test_Name;


        public abstract bool Export(DAL.Models.LIMS_Batch Batch, List<DAL.Models.LIMS_Batch_Samples> Samples, out string ErrMsg);

        public static string GetSafeFileName(string FileName)
        {
            string retVal = FileName;
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                retVal = retVal.Replace(c.ToString(), string.Empty);
            return retVal;

        }

        /// <summary>
        /// get a list of all LIMS Lab Exports
        /// </summary>
        /// <returns></returns>
        public static List<EqpExport> GetAllExports(IConfiguration Config)
        {
            List<EqpExport> retVal =
            [
                new IC(Config),
                new ICP(Config,1),
                new ICP(Config,2),
                new FTIR(Config),
                new GC(Config),
                new PartCount(Config),
                new Viscometer(Config)
            ];
            return retVal;
        }
    }
}
