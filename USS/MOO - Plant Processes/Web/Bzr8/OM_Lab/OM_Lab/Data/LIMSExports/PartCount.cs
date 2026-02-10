using MOO.DAL.LIMS.Models;
using MOO.DAL.ToLive.Models;
using Serilog;
using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using DAL = MOO.DAL.ToLive;

namespace OM_Lab.Data.LIMSExports
{
    /// <summary>
    /// Export for Particle Counter
    /// </summary>
    internal class PartCount : EqpExport
    {
        //file is split based on number of samples in the tray
        /// <summary>
        /// number of samples per tray
        /// </summary>
        private const short SAMPLES_PER_TRAY = 24;

        public PartCount(IConfiguration Config) : base(Config)
        {
            SetupDirectoryStructure();
        }

        public override string Test_Name => "PARTCOUNT";

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
            string fName = $"{ExportRoot}\\{Batch.Batch_Type}\\{GetSafeFileName(Batch.Batch_Name)}_LNFTray{TrayNbr}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            using StreamWriter writer = new(fName + ".tmp");  //create as a tmp first            //all samples are on one line
            writer.Write("1");
            int sampNbr;
            LIMS_Batch_Samples smp;
            for (short nSamp = 0; nSamp <= (SAMPLES_PER_TRAY - 1); nSamp++)
            {
                sampNbr = nSamp + (SAMPLES_PER_TRAY * (TrayNbr - 1));
                if(sampNbr <= Samples.Count - 1)
                {
                    smp = Samples[sampNbr];
                    try
                    {
                        //particle counter only has room for 12 characters
                        //we will make a 3 character code for the sample number and take the right 9 characters of sample point (first character is either M or K)
                        writer.Write($", {IntegerToPartCountCode(smp.Sample_Number)}{smp.Sample.Sampling_Point.Identity[1..]}");
                        SamplesExported++;
                    }
                    catch (Exception sampException)
                    {
                        ExportLog.AppendLine($"Error on sample {smp.Sample_Number}: {sampException.Message}");
                        Log.Error(sampException, $"Error on export {Test_Name} for batch {Batch.Batch_Id} and sample number {smp.Sample_Number}");
                    }
                    
                }
            }
            writer.WriteLine();
            writer.Flush();
            writer.Close();
            File.Move(fName + ".tmp", fName);
        }





        /// <summary>
        /// converts the sample number integer to a 3 character code to be used by particle counter
        /// </summary>
        /// <param name="SampleNumber">The sample number to be converted</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string IntegerToPartCountCode(int SampleNumber)
        {
            //first take modulus 200,000.  This is because we don't have enough character for the number of samples we will
            //have so we must start over at 200,000 which is well before we run out of 3 letter codes
            int nModdedVal = SampleNumber % 200000;
            int[] codes = new int[3];

            codes[2] = (int)Math.Floor((decimal)(nModdedVal / 3844M));
            codes[1] = (int)Math.Floor((nModdedVal - (codes[2] * 3844M)) / 62M);
            codes[0] = (int)(nModdedVal - (codes[2] * 3844M) - (codes[1] * 62M));

            string retVal = "";
            int ASCCode;
            for (int nInt = codes.Length - 1; nInt >= 0; nInt--)
            {
                if (codes[nInt] < 10)
                {
                    //character code 0-9
                    ASCCode = 48 + codes[nInt];
                }
                else if (codes[nInt] >= 10 && codes[nInt] < 36)
                {
                    //character code A-Z
                    ASCCode = 65 + (codes[nInt] - 10);
                }
                else //if (codes[nInt] >= 36)
                {
                    //character code a-z
                    ASCCode = 97 + (codes[nInt] - 36);
                }
                retVal += ((char)ASCCode).ToString();
            }
            return retVal;
        }

        /// <summary>
        /// This converts the encoded particle counter sample code back to the sample number
        /// </summary>
        /// <param name="SampleCode"></param>
        /// <returns></returns>
        public static int PartCountCodeToInteger(string SampleCode)
        {
            int charCode;
            int[] codes = new int[3];
            int nCode = 0;

            for (short nChar = 2; nChar >= 0; nChar--)
            {
                charCode = Encoding.ASCII.GetBytes(SampleCode.Substring(nChar, 1))[0];
                if (charCode >= 48 && charCode <= 57)
                    codes[nCode] = charCode - 48; //characters 0-9
                else if (charCode >= 65 && charCode <= 90)
                    codes[nCode] = charCode - 65 + 10; //Character A-Z
                else if (charCode >= 97 && charCode <= 122)
                    codes[nCode] = charCode - 97 + 36; //Character a-z

                nCode++;
            }
            //we will have a base 62 code for this 10 (digits) + 26 (A-Z) + 26 (a-z)
            int retval, converted;
            converted = codes[0] + (codes[1] * 62) + (codes[2] * 62 * 62);
            //finally, we don't have enough letters to fit sample ID in code so we had to mod it with 200,000
            //we will now find a sample logged in within the last 6 months that fit this criteria
            StringBuilder sql = new();
            sql.AppendLine("SELECT  id_numeric");
            sql.AppendLine("FROM sample");
            sql.AppendLine("WHERE sampled_date > DATEADD(MONTH,-6,GETDATE())");
            sql.AppendLine("AND CAST(id_numeric as Int) % 200000 = " + converted);
            var smp = MOO.Data.ExecuteScalar(sql.ToString(), MOO.Data.MNODatabase.LIMS_Read, -1).ToString();
            retval = int.Parse(smp!);
            return retval;

        }



    }
}
