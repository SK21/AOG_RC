using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using GMap.NET.WindowsForms;
using GMap.NET;

namespace RateController.Classes
{
    public class MapImageSaver
    {
        private Bitmap compositeImage;
        public Panel LegendPanel { get; set; }
        public GMapControl MapControl { get; set; }

        public void SaveCompositeImageToFile(string Name)
        {
            CreateCompositeMapImage();

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Files|*.png|JPEG Files|*.jpg|Bitmap Files|*.bmp";
                sfd.Title = "Save Composite Map Image";
                sfd.FileName = Name;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format = ImageFormat.Png;
                    string ext = System.IO.Path.GetExtension(sfd.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".jpeg")
                    {
                        format = ImageFormat.Jpeg;
                    }
                    else if (ext == ".bmp")
                    {
                        format = ImageFormat.Bmp;
                    }

                    compositeImage.Save(sfd.FileName, format);
                    MessageBox.Show("Image saved successfully.", "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Creates a composite image from the current GMapControl and the legend panel.
        /// The legend is always drawn at the top-left corner (with a fixed margin) of the map image.
        /// </summary>
        private void CreateCompositeMapImage()
        {
            Bitmap mapBitmap = (Bitmap)MapControl.ToImage();

            Bitmap legendBitmap = new Bitmap(LegendPanel.Width, LegendPanel.Height);
            LegendPanel.DrawToBitmap(legendBitmap, new Rectangle(0, 0, LegendPanel.Width, LegendPanel.Height));

            compositeImage = new Bitmap(mapBitmap.Width, mapBitmap.Height);

            using (Graphics g = Graphics.FromImage(compositeImage))
            {
                g.DrawImage(mapBitmap, 0, 0);

                Point legendLocation = new Point(10, 10);
                g.DrawImage(legendBitmap, legendLocation);
            }
        }
    }
}