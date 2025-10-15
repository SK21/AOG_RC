using GMap.NET.WindowsForms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

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

            ImageFormat format = ImageFormat.Png;
            compositeImage.Save(Name, format);
            Props.ShowMessage("Image saved successfully.", "Save Image", 5000);
        }

        /// <summary>
        /// Creates a composite image from the current GMapControl and the legend panel.
        /// The legend is always drawn at the top-left corner (with a fixed margin) of the map image.
        /// </summary>
        private void CreateCompositeMapImage()
        {
            Bitmap mapBitmap = (Bitmap)MapControl.ToImage();
            compositeImage = new Bitmap(mapBitmap.Width, mapBitmap.Height);

            using (Graphics g = Graphics.FromImage(compositeImage))
            {
                g.DrawImage(mapBitmap, 0, 0);

                if (LegendPanel.Visible)
                {
                    Bitmap legendBitmap = new Bitmap(LegendPanel.Width, LegendPanel.Height);
                    LegendPanel.DrawToBitmap(legendBitmap, new Rectangle(0, 0, LegendPanel.Width, LegendPanel.Height));

                    Point legendLocation = new Point(10, 10);
                    g.DrawImage(legendBitmap, legendLocation);
                }
            }
        }
    }
}