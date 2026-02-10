using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class LIMS_Batch_Samples
    {
        public int Batch_Id { get; set; }
        public decimal Batch_Seq { get; set; }
        public int Sample_Number { get; set; }

        private MOO.DAL.LIMS.Models.Sample _Sample;
        public MOO.DAL.LIMS.Models.Sample Sample
        {
            get
            {
                //only get the sample if requested
                if (_Sample == null)
                {
                    _Sample = MOO.DAL.LIMS.Services.SampleSvc.Get(Sample_Number);
                    if(_Sample == null)
                    {
                        //if we still don't have a sample, create new and put sample not found
                        _Sample = new()
                        {
                            Sampling_Point = new() { Name = "Sample Not Found" },
                            Description = "Sample Not Found"
                        };
                    }
                }

                return _Sample;
            }
        }
    }
}
