using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web;
using System.Collections;

namespace MOO.DAL
{
    //Exception that will be used to for validating data from DAL
    public class InvalidDataException : Exception
    {
        public override string Message => ErrorMessages;

        public InvalidDataException()
        {
            ValidationResults = [];
        }

        public InvalidDataException(string Message)
        {
            ValidationResults = [];
            ValidationResults.Add(new ValidationResult(Message));

        }


        public InvalidDataException(string Message, string FieldName)
        {
            ValidationResults = [];
            ValidationResults.Add(new ValidationResult(Message, new string[] {FieldName}));

        }

        /// <summary>
        /// Gets the list of error messages
        /// </summary>
        public string ErrorMessages { get
            {
                StringBuilder msg = new();
                foreach (ValidationResult validationResult in ValidationResults)
                {
                    msg.AppendLine(validationResult.ErrorMessage);
                }
                return msg.ToString();
            }
        }

        /// <summary>
        /// Gets the list of Error messages with a HTML line break between the lines
        /// </summary>
        public  string ErrorMessagesHTML
        {
            get
            {
                StringBuilder msg = new();
                foreach (ValidationResult validationResult in ValidationResults)
                {
                    msg.AppendLine(HttpUtility.HtmlEncode(validationResult.ErrorMessage));
                    msg.AppendLine("<br/>");
                    
                }
                return msg.ToString();
            }
        }

        /// <summary>
        /// The list of validation errors
        /// </summary>
        public List<ValidationResult> ValidationResults { get; set; }

        /// <summary>
        /// Checks the data object based on the System.ComponentModel.DataAnnotations attributes applied to the object
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static InvalidDataException CheckData(object Data)
        {
            ValidationContext context = new ValidationContext(Data, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool valid = Validator.TryValidateObject(Data, context, validationResults, true);

            if (!valid)
            {
                InvalidDataException retVal = new InvalidDataException()
                {
                    ValidationResults = validationResults
                };
                return retVal;
            }
            else 
                return null;

        }
    }
}
