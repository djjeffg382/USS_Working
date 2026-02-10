using MOO.DAL.ToLive.Models;
using Serilog;
using System.IO;
using System.Net.Mail;
using System.Text;
using DAL = MOO.DAL.ToLive;

namespace OM_Lab.Data.LIMSExports
{
    internal class FTIR : EqpExport
    {
        public FTIR(IConfiguration Config) : base(Config)
        {
            SetupDirectoryStructure();
        }

        public override string Test_Name => "FTIR";

        public override bool Export(LIMS_Batch Batch, List<LIMS_Batch_Samples> Samples, out string ErrMsg)
        {
            StringBuilder expLog = new();  //log of any errors and final results
            try
            {

                List<DAL.Models.LIMS_Batch_Samples> expSamples = GetSamplesForThisTest(Samples);
                int samplesExported = 0;
                if (expSamples.Count > 0)
                {

                    string fName = $"{ExportRoot}\\{Batch.Batch_Type}\\{GetSafeFileName(Batch.Batch_Name)}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    using StreamWriter writer = new(fName + ".tmp");  //create as a tmp first
                    WriteFileHeader(writer, expSamples.Count);
                    int nSamp = 1;

                    foreach (var bs in expSamples)
                    {
                        try
                        {
                            writer.Write(nSamp + ",");
                            //'FTIR does not allow pipes "|" so we will use a period instead
                            //'We will have to replace the period with a pipe when coming back to LIMS
                            writer.Write($"{bs.Sample_Number}.{bs.Sample.Sampling_Point.Identity},");
                            writer.Write("MineralOil,");
                            writer.Write("Ref Spectra  Database\\NEW OILS\\" + bs.Sample.Oil_Type.Phrase_Text);

                            writer.WriteLine(",,");
                            nSamp++;
                            samplesExported++;
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
                    expLog.AppendLine($"{samplesExported} samples exported");
                }
            }
            catch (Exception ex)
            {
                expLog.AppendLine($"Error on export: {ex.Message}");
                Log.Error(ex, $"Error on export {Test_Name} for batch {Batch.Batch_Id}");
            }
            this.ExportLog = expLog.ToString();
            ErrMsg = ExportLog;
            return true;
        }

        private static void WriteFileHeader(StreamWriter writer, int SampCount)
        {
            writer.WriteLine("Oil Data,,,,,");

            writer.WriteLine(",,,,,");
            writer.WriteLine($"{SampCount},,,,,");
            writer.WriteLine("0,,,,,");
            writer.WriteLine("Position , Sample ID , Analysis , Reference Spectrum , Fuel Calibration , Oil Type");
        }
    }
}
