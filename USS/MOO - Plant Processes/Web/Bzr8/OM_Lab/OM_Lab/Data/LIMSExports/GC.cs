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
    internal class GC : EqpExport
    {
        public GC(IConfiguration Config) : base(Config)
        {
            SetupDirectoryStructure();
        }

        public override string Test_Name => "GC";

        public override bool Export(LIMS_Batch Batch, List<LIMS_Batch_Samples> Samples, out string ErrMsg)
        {
            StringBuilder expLog = new();  //log of any errors and final results
            try
            {
                List<DAL.Models.LIMS_Batch_Samples> expSamples = GetSamplesForThisTest(Samples);

                if (expSamples.Count > 0)
                {

                    string fName = $"{ExportRoot}\\{Batch.Batch_Type}\\{GetSafeFileName(Batch.Batch_Name)}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    using StreamWriter writer = new(fName + ".tmp");  //create as a tmp first
                    int nSamp = 0;

                    foreach (var bs in expSamples)
                    {
                        try
                        {
                            writer.WriteLine($"{bs.Sample_Number}|{bs.Sample.Sampling_Point.Identity}");
                            nSamp++;
                        }
                        catch (Exception sampException)
                        {
                            expLog.AppendLine($"Error on sample {bs.Sample_Number}: {sampException.Message}");
                            Log.Error(sampException, $"Error on export {Test_Name} for batch {Batch.Batch_Id} and sample number {bs.Sample_Number}");
                        }
                        
                    }
                    writer.Flush();
                    writer.Close();
                    File.Move(fName + ".tmp", fName);

                    expLog.AppendLine($"{nSamp} samples exported");
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

    }
}
