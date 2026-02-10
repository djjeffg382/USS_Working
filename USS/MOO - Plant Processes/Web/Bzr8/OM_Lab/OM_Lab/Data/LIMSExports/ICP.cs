using MOO.DAL.ToLive.Models;
using Serilog;
using System.IO;
using System.Net.Mail;
using System.Text;
using DAL = MOO.DAL.ToLive;

namespace OM_Lab.Data.LIMSExports
{
    internal class ICP : EqpExport
    {
        private readonly byte ICPNumber;
        public ICP(IConfiguration Config, byte iCPNumber) : base(Config)
        {
            ICPNumber = iCPNumber;
            SetupDirectoryStructure();
        }

        public override string Test_Name => "ICP";
        public override string Machine_Name => $"ICP{ICPNumber}";

        public override bool Export(LIMS_Batch Batch, List<LIMS_Batch_Samples> Samples, out string ErrMsg)
        {
            StringBuilder expLog = new();  //log of any errors and final results
            try
            {

                List<DAL.Models.LIMS_Batch_Samples> expSamples = GetSamplesForThisTest(Samples);

                if (expSamples.Count > 0)
                {
                    int samplesExported = 0;
                    string fName;
                    StreamWriter writer;
                    if (ICPNumber == 1)
                    {
                        fName = $"{ExportRoot}\\{Batch.Batch_Type}\\{GetSafeFileName(Batch.Batch_Name)}_{DateTime.Now:yyyyMMdd_HHmmss}.sifx";
                        //ICP1 requires file to be set up as Unicode and not ASCII
                        writer = new(fName + ".tmp", false, System.Text.Encoding.Unicode);  //create as a tmp first
                    }
                    else //if(ICPNumber == 2)
                    {
                        fName = $"{ExportRoot}\\{Batch.Batch_Type}\\{GetSafeFileName(Batch.Batch_Name)}_{DateTime.Now:yyyyMMdd_HHmmss}.sif";
                        writer = new(fName + ".tmp");  //create as a tmp first
                    }


                    WriteFileHeader(writer, expSamples.Count);
                    int nSamp = 1;

                    foreach (var bs in expSamples)
                    {
                        try
                        {

                            writer.Write($"Data{nSamp}={nSamp},");
                            //position starts as 107 so take 106 plus the sample number
                            writer.Write((106 + nSamp).ToString() + ",");
                            writer.WriteLine($"{bs.Sample.Id_Numeric}|{bs.Sample.Sampling_Point.Identity}");
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

        private void WriteFileHeader(StreamWriter writer, int SampCount)
        {
            writer.WriteLine("[System Description]");

            writer.WriteLine("Description=OIL");
            writer.WriteLine("MaxNoOfSamples");
            writer.WriteLine("[Constant Parameters]");
            writer.WriteLine("BatchID=ICP" + ICPNumber);
            writer.WriteLine("AnalystName=LUB");
            writer.WriteLine("VolumeUnits=Vol,mL,0.001,,");
            writer.WriteLine("[User Defined List]");
            writer.WriteLine("UserDefined1=");
            writer.WriteLine("UserDefined2=");
            writer.WriteLine("UserDefined3=");
            writer.WriteLine("UserDefined4=");
            writer.WriteLine("UserDefined5=");
            writer.WriteLine("[Variable Parameter List]");
            writer.WriteLine("NumberOfParameters=5");
            writer.WriteLine("Parameter1=SampleNo");
            writer.WriteLine("Parameter2=AutosamplerLocation");
            writer.WriteLine("Parameter3=SampleID");
            writer.WriteLine("Parameter4=AliquotVolume");
            writer.WriteLine("Parameter5=DilutedToVolume");
            writer.WriteLine("[Variable Parameter Data]");
            writer.WriteLine("NumberOfDataValues=" + SampCount);
        }
    }
}
