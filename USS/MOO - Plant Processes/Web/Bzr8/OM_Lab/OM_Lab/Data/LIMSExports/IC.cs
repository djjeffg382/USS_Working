using MOO.DAL.ToLive.Models;
using Serilog;
using System.IO;
using System.Net.Mail;
using System.Text;
using DAL = MOO.DAL.ToLive;
namespace OM_Lab.Data.LIMSExports
{
    internal class IC : EqpExport
    {
        public IC(IConfiguration Config) : base(Config)
        {
            SetupDirectoryStructure();
        }

        public override string Test_Name => "IC";


        public override bool Export(LIMS_Batch Batch, List<LIMS_Batch_Samples> Samples, out string ErrMsg)
        {
            StringBuilder expLog = new();  //log of any errors and final results
            try
            {
                List<DAL.Models.LIMS_Batch_Samples> expSamples = GetSamplesForThisTest(Samples);

                if (expSamples.Count > 0)
                {

                    string fName = $"{ExportRoot}\\{Batch.Batch_Type}\\{GetSafeFileName(Batch.Batch_Name)}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    using StreamWriter writer = new(fName + ".tmp");  //create as a tmp first
                                                                      //start sample number at 4 they always run 3 check samples
                    int nSamp = 4;
                    int samplesExported = 0;
                    foreach (var bs in expSamples)
                    {
                        try
                        {
                            writer.Write("Anions,");                            //Method
                            writer.Write($"{bs.Sample_Number}|{bs.Sample.Sampling_Point.Identity},");    //Ident
                            writer.Write("Sample,");                            //Sample
                            writer.Write(nSamp + ",");                          //Position
                            writer.Write(",");                                  //Status
                            writer.Write("1,");                                 //Dilution
                            writer.Write(",");                                  //Info1
                            writer.Write("10,");                                //Value1
                            writer.Write("");                                   //Value2
                            writer.WriteLine();
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

            ExportLog = expLog.ToString();
            ErrMsg = ExportLog;
            return true;
        }
    }
}
