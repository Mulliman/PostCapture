using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageTools.Core.Selection
{
    public class ProcessSteps
    {
        public ProcessSteps(IEnumerable<IApplier> steps)
        {
            Steps = steps.ToList();
        }

        public List<IApplier> Steps { get; }
    }
}