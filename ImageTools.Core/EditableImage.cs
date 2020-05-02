using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageTools.Core
{
    public class EditableImage : ImageFile
    {
        public EditableImage(string path) : base (path)
        {
        }

        public EditableImage(string path, MagickImage image) : base(path, image)
        {
        }

        public new static EditableImage FromFilePath(string path)
        {
            return new EditableImage(path);
        }

        public virtual void Save()
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            Encoder myEncoder = Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);

            myEncoderParameters.Param[0] = myEncoderParameter;

            Image.Write(Path);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
    }
}