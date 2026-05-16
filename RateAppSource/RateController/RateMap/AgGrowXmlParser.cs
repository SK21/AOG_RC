using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RateController.RateMap
{
    public static class AgGrowXmlParser
    {
        private const int MaxLayers = 4;  // ProductA..D; AgGrow supports 4 layers

        // Returns true if the file looks like an AgGrow VRT XML.
        public static bool IsAgGrowFile(string filePath)
        {
            try
            {
                var root = XDocument.Load(filePath).Root;
                return root?.Name.LocalName == "Vrt";
            }
            catch { return false; }
        }

        public static List<MapZone> Parse(string filePath, out string[] layerNames)
        {
            layerNames = new string[MaxLayers];
            var result = new List<MapZone>();
            try
            {
                var root = XDocument.Load(filePath).Root;
                if (root == null || root.Name.LocalName != "Vrt")
                {
                    Props.WriteErrorLog("AgGrowXmlParser: not an AgGrow VRT file.");
                    return result;
                }

                // UTM zone from <General><Zone>
                int utmZone = 0;
                int.TryParse(root.Element("General")?.Element("Zone")?.Value, out utmZone);
                if (utmZone == 0)
                {
                    Props.WriteErrorLog("AgGrowXmlParser: missing or invalid <Zone>.");
                    return result;
                }

                // Applicant names from <General><Layers><Layer><Applicant>, keyed by layer ID.
                var generalLayers = root.Element("General")?.Element("Layers")?.Elements("Layer");
                if (generalLayers != null)
                {
                    foreach (var gl in generalLayers)
                    {
                        int id = 0;
                        int.TryParse(gl.Element("ID")?.Value, out id);
                        string applicant = gl.Element("Applicant")?.Value ?? string.Empty;
                        int idx = id - 1;
                        if (idx >= 0 && idx < MaxLayers && !string.IsNullOrEmpty(applicant))
                            layerNames[idx] = applicant;
                    }
                }

                // Grid layers are in <Vrt><Layers><Layer> (not <General><Layers>)
                var gridLayers = root.Element("Layers")?.Elements("Layer").ToList();
                if (gridLayers == null || gridLayers.Count == 0)
                {
                    Props.WriteErrorLog("AgGrowXmlParser: no grid layers found.");
                    return result;
                }

                // Grid dimensions from first layer — all layers share the same grid.
                var first = gridLayers[0];
                double north = 0, west = 0, cellX = 0, cellY = 0;
                int cellsX = 0, cellsY = 0;
                double.TryParse(first.Element("North")?.Value, out north);
                double.TryParse(first.Element("West")?.Value, out west);
                double.TryParse(first.Element("MetresPerCellX")?.Value, out cellX);
                double.TryParse(first.Element("MetresPerCellY")?.Value, out cellY);
                int.TryParse(first.Element("CellsX")?.Value, out cellsX);
                int.TryParse(first.Element("CellsY")?.Value, out cellsY);

                if (cellsX <= 0 || cellsY <= 0 || cellX <= 0 || cellY <= 0)
                {
                    Props.WriteErrorLog("AgGrowXmlParser: invalid grid dimensions.");
                    return result;
                }

                // Remove the 10,000,000 false northing AgGrow applies universally.
                if (north > 10_000_000) north -= 10_000_000;

                int total = cellsX * cellsY;
                int numProducts = Math.Min(MaxLayers, ZoneFields.Products.Length);

                double[][] rates = new double[total][];
                for (int i = 0; i < total; i++)
                    rates[i] = new double[numProducts];

                int layerCount = Math.Min(gridLayers.Count, MaxLayers);
                for (int layerIdx = 0; layerIdx < layerCount; layerIdx++)
                {
                    var layer = gridLayers[layerIdx];
                    int layerID = 0;
                    int.TryParse(layer.Element("ID")?.Value, out layerID);

                    // Layer ID 1 → ProductA (index 0), ID 2 → ProductB, etc.
                    int productIdx = layerID - 1;
                    if (productIdx < 0 || productIdx >= numProducts) continue;

                    // <Rates> is a palette of distinct rate values (not per-cell data).
                    double[] palette = layer.Element("Rates")
                        ?.Elements("Rate")
                        .Select(e => { double.TryParse(e.Value, out double v); return v; })
                        .ToArray() ?? new double[0];

                    if (palette.Length == 0) continue;

                    // <Rows><Row> contains the per-cell palette indices.
                    // Each <Row> is a string of (rowLength / cellsX) chars per cell,
                    // representing the decimal palette index with leading zeros.
                    var rowElements = layer.Element("Rows")?.Elements("Row").ToList();
                    if (rowElements == null || rowElements.Count == 0) continue;

                    // Derive chars-per-cell from the first non-empty row.
                    string firstRow = rowElements[0].Value ?? string.Empty;
                    int charsPerCell = cellsX > 0 && firstRow.Length >= cellsX
                        ? firstRow.Length / cellsX
                        : 2;

                    int rowCount = Math.Min(rowElements.Count, cellsY);
                    for (int row = 0; row < rowCount; row++)
                    {
                        string rowData = rowElements[row].Value ?? string.Empty;
                        int colCount = Math.Min(cellsX, rowData.Length / charsPerCell);
                        for (int col = 0; col < colCount; col++)
                        {
                            string indexStr = rowData.Substring(col * charsPerCell, charsPerCell);
                            if (int.TryParse(indexStr, System.Globalization.NumberStyles.HexNumber,
                                    System.Globalization.CultureInfo.InvariantCulture, out int paletteIdx)
                                && paletteIdx >= 0 && paletteIdx < palette.Length)
                            {
                                rates[row * cellsX + col][productIdx] = palette[paletteIdx];
                            }
                        }
                    }
                }

                var grid = new VrtGrid
                {
                    OriginEasting  = west,
                    OriginNorthing = north,
                    CellSizeX      = cellX,
                    CellSizeY      = cellY,
                    CellsX         = cellsX,
                    CellsY         = cellsY,
                    UtmZone        = utmZone,
                    Rates          = rates
                };

                result = VrtGridImporter.ImportCells(grid);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("AgGrowXmlParser/Parse: " + ex.Message);
            }
            return result;
        }
    }
}
