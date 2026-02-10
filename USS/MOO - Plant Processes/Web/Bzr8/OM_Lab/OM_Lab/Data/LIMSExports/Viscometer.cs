using MOO.DAL.ToLive.Models;
using Serilog;
using System.IO;
using System.Text;
using DAL = MOO.DAL.ToLive;

namespace OM_Lab.Data.LIMSExports
{
    /// <summary>
    /// Export for Gas Chromatograph
    /// </summary>
    internal class Viscometer : EqpExport
    {
        public Viscometer(IConfiguration Config) : base(Config)
        {
            SetupDirectoryStructure();
        }

        public override string Test_Name => "VISCOMETER";

        //file is split based on number of samples in the tray
        /// <summary>
        /// number of samples per tray
        /// </summary>
        private const short SAMPLES_PER_TRAY = 24;

        public override bool Export(LIMS_Batch Batch, List<LIMS_Batch_Samples> Samples, out string ErrMsg)
        {
            StringBuilder expLog = new();  //log of any errors and final results
            try
            {
                List<DAL.Models.LIMS_Batch_Samples> expSamples = GetSamplesForThisTest(Samples);

                int samplesExported = 0;

                if (expSamples.Count > 0)
                {
                    for (short nTray = 1; nTray <= Math.Ceiling((decimal)expSamples.Count / (decimal)SAMPLES_PER_TRAY); nTray++)
                    {
                        ExportTrayFile(nTray, Batch, expSamples, ref samplesExported, expLog);
                    }
                    expLog.AppendLine($"{samplesExported} samples exported");
                }
            }
            catch (Exception ex)
            {
                expLog.AppendLine($"Error on export: {ex.Message}");
                Log.Error(ex, $"Error on export {Test_Name} for batch {Batch.Batch_Id}");
            }
            ExportLog = expLog.ToString();
            ErrMsg = ExportLog;
            return true;
        }

        /// <summary>
        /// Exports the new tray file
        /// </summary>
        /// <param name="TrayNbr">The tray number.  This allows a limited number of samples per tray</param>
        /// <param name="Batch">The batch object</param>
        /// <param name="Samples">The complete sample list</param>
        /// <param name="SamplesExported">Reference to a SampleExported variable to keep track of total samples export</param>
        /// <param name="ExportLog">Export log variable to add any messages</param>
        private void ExportTrayFile(short TrayNbr, LIMS_Batch Batch, List<LIMS_Batch_Samples> Samples, ref int SamplesExported, StringBuilder ExportLog)
        {
            string fName = $"{ExportRoot}\\{Batch.Batch_Type}\\{GetSafeFileName(Batch.Batch_Name)}_ViscoTray{TrayNbr}_{DateTime.Now:yyyyMMdd_HHmmss}.cic";
            using StreamWriter writer = new(fName + ".tmp");  //create as a tmp first            
            writer.WriteLine("Position , Sample ID");
            //'not sure if this line is needed but the old system had it so we will continue to send it
            writer.WriteLine("1,");
            int sampNbr;
            LIMS_Batch_Samples smp;
            for (short nSamp = 0; nSamp <= (SAMPLES_PER_TRAY - 1); nSamp++)
            {
                sampNbr = nSamp + (SAMPLES_PER_TRAY * (TrayNbr - 1));
                if (sampNbr <= Samples.Count - 1)
                {
                    smp = Samples[sampNbr];
                    try
                    {

                        writer.Write("1,");
                        writer.Write($"{smp.Sample_Number}|{smp.Sample.Sampling_Point.Identity}");
                    }
                    catch (Exception sampException)
                    {
                        ExportLog.AppendLine($"Error on sample {smp.Sample_Number}: {sampException.Message}");
                        Log.Error(sampException, $"Error on export {Test_Name} for batch {Batch.Batch_Id} and sample number {smp.Sample_Number}");
                    }
                }
                writer.WriteLine();
                SamplesExported++;
            }
            writer.Flush();
            writer.Close();
            File.Move(fName + ".tmp", fName);
        }

    }
}
