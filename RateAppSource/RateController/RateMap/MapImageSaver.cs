using GMap.NET.WindowsForms;
using System.Drawing;
using System.Drawing.Imaging;

namespace RateController.Classes
{
    public class MapImageSaver
    {
        public GMapControl MapControl { get; set; }

        public void SaveCompositeImageToFile(string Name)
        {
            if (MapControl == null)
            {
                Props.ShowMessage("Map control is not available.", "Save Image", 5000, true);
                return;
            }

            using (var mapImage = MapControl.ToImage()) // includes overlays if visible
            using (var mapBitmap = new Bitmap(mapImage))
            {
                mapBitmap.Save(Name, ImageFormat.Png);
            }

            Props.ShowMessage("Image saved successfully.", "Save Image", 5000);
        }
    }
}