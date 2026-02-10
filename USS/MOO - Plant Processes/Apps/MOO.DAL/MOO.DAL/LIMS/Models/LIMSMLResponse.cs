using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MOO.DAL.LIMS.Models
{
    /// <summary>
    /// class used for handling the LIMS ML responses
    /// </summary>
    public sealed class LIMSMLResponse(string XMLResponse)
    {

        /// <summary>
        /// The raw XML of the response
        /// </summary>
        public string XMLResponse { get; set; } = XMLResponse;


        /// <summary>
        /// The error string parsed out from the xml
        /// </summary>
        public string Error { get { return ParseXMLForErrors(); } }

        /// <summary>
        /// Reads LIMS Response field from the XML
        /// </summary>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public string ReadLIMSResponseField(string FieldName) {

            //examole of an xml response contains fields.  The fieldname parameter will refer to the id attribute as we search all of the fields returned
            //<field id="ID_TEXT" direction="out" datatype="Text">0197067</field><field id="SAMPLING_POINT" direction="in" datatype="Text">M0580_1</field>
            XmlReader xmlRdr = XmlReader.Create(new StringReader(XMLResponse));

            while (xmlRdr.ReadToFollowing("field"))
            {
                var attr = xmlRdr.GetAttribute("id");                
                if (!string.IsNullOrEmpty(attr) && attr == FieldName)  //we found it
                {
                    var ret =  xmlRdr.ReadElementContentAsString();
                    return ret;
                }
                    
            }
            return "";               
            
        }

        private string ParseXMLForErrors()
        {
            XmlReader xmlRdr = XmlReader.Create(new StringReader(XMLResponse));
            xmlRdr.ReadToFollowing("errors");

            if (xmlRdr.IsEmptyElement)
                return "";            //no Errors, all is good
            else
            {
                xmlRdr.ReadToDescendant("error");
                xmlRdr.ReadToDescendant("summary");
                if (xmlRdr.ReadElementContentAsString().Contains("Check child errors"))
                {
                    //need to go to the child errors of this node
                    xmlRdr.ReadToNextSibling("errors");
                    xmlRdr.ReadToDescendant("error");
                    xmlRdr.ReadToDescendant("description");
                    return xmlRdr.ReadElementContentAsString();
                }
                else
                {
                    //this contains the error
                    //move to the description
                    xmlRdr.ReadToNextSibling("description");
                    return xmlRdr.ReadElementContentAsString();
                }
            }

        }
    }
}
