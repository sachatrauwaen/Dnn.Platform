// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Services.GeneratedImage
{
    // using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;

    /// <summary>An abstract ImageTransform class.</summary>
    public abstract class ImageTransform
    {
        // REVIEW: should this property be abstract?

        /// <summary>Gets provides an Unique String for the image transformation.</summary>
        public virtual string UniqueString => this.GetType().FullName;

        /// <summary>Gets or sets the interpolation mode used for resizing images. The default is <see cref="InterpolationMode.HighQualityBicubic"/>.</summary>
        public InterpolationMode InterpolationMode { get; set; }

        /// <summary>Gets or sets the smoothing mode used for resizing images. The default is <see cref="SmoothingMode.HighQuality"/>.</summary>
        public SmoothingMode SmoothingMode { get; set; }

        /// <summary>Gets or sets the pixel offset mode used for resizing images. The default is <see cref="PixelOffsetMode.HighQuality"/>.</summary>
        public PixelOffsetMode PixelOffsetMode { get; set; }

        /// <summary>Gets or sets the compositing quality used for resizing images. The default is <see cref="CompositingQuality.HighQuality"/>.</summary>
        public CompositingQuality CompositingQuality { get; set; }

        /// <summary>Process an input image applying the image transformation.</summary>
        /// <param name="image">Input image.</param>
        /// <returns>Image processed.</returns>
        public abstract Image ProcessImage(Image image);

        /// <summary>Creates a new image from stream. The created image is independent of the stream.</summary>
        /// <param name="imgStream">Le flux d'entrée contenant les données de l'image.</param>
        /// <returns>Image object.</returns>
        public virtual SixLabors.ImageSharp.Image CopyImage(Stream imgStream)
        {
            using (var srcImage = SixLabors.ImageSharp.Image.Load(imgStream))
            {
                var destImage = new Image<Rgba32>(srcImage.Width, srcImage.Height);
                destImage.Mutate(x => x.DrawImage(srcImage, new Point(0, 0), 1f));
                return destImage;
            }
        }
    }
}
