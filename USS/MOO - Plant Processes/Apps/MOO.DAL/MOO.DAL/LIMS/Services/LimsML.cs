using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Data;
using MOO.DAL.LIMS.Models;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace MOO.DAL.LIMS.Services
{
    public class LimsML
    {

        static LimsML()
        {
            Util.RegisterSqlServer();
        }


        /// <summary>
        /// executes the Entity Transacion list to LIMS ML
        /// </summary>
        /// <param name="EntityTransactions">String list of entity transactions from LIMS\LIMS_XML\Entities</param>
        /// <returns></returns>
        private static LIMSMLResponse ExecuteLIMS_ML(IEnumerable<string> EntityTransactions)
        {
            //get the base lims document
            string lims_Doc = ReadEmbeddedXML("MOO.DAL.LIMS.LIMSML_XML.Base.xml");
            //set the system password
            lims_Doc = lims_Doc.Replace("##SYSTEM_PASS##", MOO.Settings.LIMS_System_Pass);
            //Now add each of the Entity Transactions
            StringBuilder entities = new();
            foreach(var entity in EntityTransactions)
            {
                entities.AppendLine(entity.ToString());
            }
            //now put those entities in place of the ##ENTITIES##
            lims_Doc = lims_Doc.Replace("##ENTITIES##", entities.ToString());

            string url = MOO.Settings.LIMS_ML_Address;


            var client = new LIMS_WCF_Svc.LIMSWebServiceClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(url);
            var resp = client.ProcessAsync(lims_Doc).GetAwaiter().GetResult().ToString();

            LIMSMLResponse retVal = new(resp);
            return retVal;
        }



        /// <summary>
        /// Gets the XML for executing
        /// </summary>
        /// <param name="ExecuteCommand"></param>
        /// <returns></returns>
        private static string GetExecuteXML(string ExecuteCommand, int SampleId)
        {
            string lims_Doc = ReadEmbeddedXML("MOO.DAL.LIMS.LIMSML_XML.Entities.ExecuteSample.xml");
            lims_Doc = lims_Doc.Replace("##SAMPLE_ID##", SampleId.ToString());
            lims_Doc = lims_Doc.Replace("##COMMAND##", ExecuteCommand);
            return lims_Doc;            
        }



        /// <summary>
        /// Returns the UpdateXML string
        /// </summary>
        /// <param name="SampleId"></param>
        /// <param name="FieldsXML">xml string containing the field data</param>
        /// <returns></returns>
        private static string GetUpdateXML(int SampleId, string FieldsXML)
        {
            string lims_Doc = ReadEmbeddedXML("MOO.DAL.LIMS.LIMSML_XML.Entities.UpdateSample.xml");
            lims_Doc = lims_Doc.Replace("##SAMPLE_ID##", SampleId.ToString());
            lims_Doc = lims_Doc.Replace("##FIELDS##", FieldsXML);
            return lims_Doc;
        }


        /// <summary>
        /// Logs in a sample for the specified sample point
        /// </summary>
        /// <param name="SamplePoint">Sample point identity</param>
        /// <param name="User">User who is logging in the sample</param>
        /// <param name="Printer">Currently not used</param>
        /// <returns></returns>
        public static int LoginOilSample(string SamplePoint, string User, string Printer)
        {
            if (string.IsNullOrEmpty(Printer))
                throw new ArgumentNullException(nameof(Printer), "Missing Printer parameter");

            //printer parameter can be used once we upgrade LIMS and the workflow can use the Printer selection to select the correct printer to print to
            //currently the workflow chooses the printer based on the samplepoint location
            string lims_Doc = ReadEmbeddedXML("MOO.DAL.LIMS.LIMSML_XML.Entities.OilLoginSample.xml");
            lims_Doc = lims_Doc.Replace("##SampPoint##", SamplePoint);
            lims_Doc = lims_Doc.Replace("##Comments##", $"Logged in by {User}");
            lims_Doc = lims_Doc.Replace("##PRINTER##", Printer);
            var retVal = ExecuteLIMS_ML([lims_Doc]);
            if (!string.IsNullOrEmpty(retVal.Error))
                throw new Exception("Error Logging in sample:" + retVal.Error);  //return errror if it did not succeed
            else
            {
                //if we don't have an error in the xml, it should have succeeded,
                //look for the field element with id="ID_TEXT" to get the sample number assigned to it
                string id = retVal.ReadLIMSResponseField("ID_TEXT");
                //starting with Samplemanager 21.2 the idText field contains the sample number with an added underscore. remove this before parsing the number
                id = id.Replace("_", "");
                if (int.TryParse(id, out int sampNbr))
                    return sampNbr;
                else
                    throw new Exception($"Unable to obtain Sample Number from xml response {retVal.XMLResponse}");
            }

        }

        /// <summary>
        /// Receives the specified Sample
        /// </summary>
        /// <param name="SampleId"></param>
        /// <param name="SampledDate">Optional: If set, sets the sampleddate and collected date after receiving</param>
        /// <returns></returns>
        public static string ExecuteReceiveSample(int SampleId, DateTime? SampledDate = null, DateTime? CollectedDate = null)
        {

            string receiveXML = GetExecuteXML("RECEIVE", SampleId);
            string[] limsDoc;
            if (SampledDate.HasValue || CollectedDate.HasValue)
            {
                StringBuilder fieldsXML = new();
                //we will include an update to sample date and/or collected date
                if(SampledDate.HasValue)
                    fieldsXML.AppendLine($"<field id=\"SAMPLED_DATE\" datatype=\"Date\" direction=\"in\" >{SampledDate:s}</field>");

                if (CollectedDate.HasValue)
                {
                    fieldsXML.AppendLine($"<field id=\"USS_COLLECTED_DATE\" datatype=\"Date\" direction=\"in\" >{CollectedDate:s}</field>");
                    fieldsXML.AppendLine($"<field id=\"COLLECTED_ON\" datatype=\"Date\" direction=\"in\" >{CollectedDate:s}</field>");
                }
                
                string updateXML = GetUpdateXML(SampleId, fieldsXML.ToString());
                limsDoc = [receiveXML, updateXML];
            }
            else
                limsDoc = [receiveXML];

            LIMSMLResponse retVal = ExecuteLIMS_ML(limsDoc);
            return retVal.Error;
        }

        /// <summary>
        /// Cancels the specified sample
        /// </summary>
        /// <param name="SampleId"></param>
        /// <returns></returns>
        public static string ExecuteCancelSample(int SampleId)
        {

            string execXML = GetExecuteXML("CANCEL", SampleId);
            LIMSMLResponse retVal = ExecuteLIMS_ML([execXML]);
            return retVal.Error;
        }


        /// <summary>
        /// Sets the collected date on the specified sample
        /// </summary>
        /// <param name="SampleID"></param>
        /// <param name="CollectedDate"></param>
        /// <param name="UpdateSampledDate">Update sample date to match collected date</param>
        /// <returns></returns>
        public static string SetCollected(int SampleID, DateTime CollectedDate)
        {
            string fieldsXML = $"<field id=\"USS_COLLECTED_DATE\" datatype=\"Date\" direction=\"in\" >{CollectedDate:s}</field>";
            fieldsXML += $"\n<field id=\"COLLECTED_ON\" datatype=\"Date\" direction=\"in\" >{CollectedDate:s}</field>";           
                
            string updateXML = GetUpdateXML(SampleID, fieldsXML);
            LIMSMLResponse retVal = ExecuteLIMS_ML([updateXML]);
            return retVal.Error;
        }


        /// <summary>
        /// sets the transfer date on the specified sample number
        /// </summary>
        /// <param name="SampleID"></param>
        /// <param name="TransferDate"></param>
        /// <returns></returns>
        public static string SetTransferDate(int SampleID, DateTime TransferDate)
        {
            string fieldsXML = $"<field id=\"USS_TRANSFER_DATE\" datatype=\"Date\" direction=\"in\" >{TransferDate:s}</field>";
           
            string updateXML = GetUpdateXML(SampleID, fieldsXML);
            LIMSMLResponse retVal = ExecuteLIMS_ML([updateXML]);
            return retVal.Error;
        }

        /// <summary>
        /// Modifies the sample date for the specified sample number
        /// </summary>
        /// <param name="SampleID"></param>
        /// <param name="SampledDate"></param>
        /// <remarks>IMPORTANT NOTE: Sample Manager updates Sample Date when Received, 
        /// any changes to sample date prior to receiving will be overwritten</remarks>
        /// <returns></returns>
        public static string SetSampledDate(int SampleID, DateTime SampledDate)
        {
            string fieldsXML = $"<field id=\"SAMPLED_DATE\" datatype=\"Date\" direction=\"in\" >{SampledDate:s}</field>";
            string updateXML = GetUpdateXML(SampleID, fieldsXML);
            LIMSMLResponse retVal = ExecuteLIMS_ML([updateXML]);
            return retVal.Error;
        }



        /// <summary>
        /// gets the LIMSML predefined xml files from the LIMSML_XML folder
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        private static string ReadEmbeddedXML(string ResourceName)
        {
            string result = "";
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(ResourceName))
            using (StreamReader reader = new(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
