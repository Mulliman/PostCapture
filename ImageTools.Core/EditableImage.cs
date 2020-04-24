using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageTools.Core
{
    public class EditableImage : ImageFile
    {
        public EditableImage(string path, Bitmap bitmap) : base (path, bitmap)
        {
        }

        public new static EditableImage FromFilePath(string path)
        {
            return new EditableImage(path, GetImage(path));
        }

        public virtual void Save()
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            Encoder myEncoder = Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);

            myEncoderParameters.Param[0] = myEncoderParameter;

            Bitmap.Save(Path, jpgEncoder, myEncoderParameters);
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