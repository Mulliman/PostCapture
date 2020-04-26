using ImageTools.Core.Selection;
using System;

namespace ImageTools.Core.Builder
{
    public class ApplierBuilder
    {
        public IApplier CreateApplier(ProcessStepConfiguration config)
        {
            if(config.Id == PercentageBorderApplier.IdConst)
            {
                PercentageBorderApplierParams args = CreatePercentageBorderApplierParams(config);

                return new PercentageBorderApplier(args);
            }

            if (config.Id == WatermarkApplier.IdConst)
            {
                WatermarkApplierParams args = CreateWatermarkApplierParams(config);

                return new WatermarkApplier(args);
            }

            throw new ArgumentNullException(config.Id + " not a valid operation ID");
        }

        private static WatermarkApplierParams CreateWatermarkApplierParams(ProcessStepConfiguration config)
        {
            return new WatermarkApplierParams()
            {
                Location = new WatermarkLocation
                {
                    ImageMarginPercentage = double.Parse(config.Parameters["ImageMarginPercentage"]),
                    ImageSizePercentage = double.Parse(config.Parameters["ImageSizePercentage"]),
                    Location = Enum.Parse<Location>(config.Parameters["Location"])
                },
                WatermarkImage = new ImageFile(config.Parameters["WatermarkImage"])
            };
        }

        private static PercentageBorderApplierParams CreatePercentageBorderApplierParams(ProcessStepConfiguration config)
        {
            return new PercentageBorderApplierParams()
            {
                BorderWidthPercentage = double.Parse(config.Parameters["BorderWidthPercentage"]),
                R = int.Parse(config.Parameters["R"]),
                G = int.Parse(config.Parameters["G"]),
                B = int.Parse(config.Parameters["B"])
            };
        }
    }
}