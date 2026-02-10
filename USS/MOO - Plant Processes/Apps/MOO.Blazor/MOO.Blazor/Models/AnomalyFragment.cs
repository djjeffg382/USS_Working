using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Blazor.Models
{
    public class AnomalyFragment(string SensorName, RenderFragment Html)
    {
        public string SensorName { get; set; } = SensorName;
        public RenderFragment Html { get; set; } = Html;
    }
}
