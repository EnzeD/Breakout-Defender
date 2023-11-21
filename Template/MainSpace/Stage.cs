using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MainSpace
{
    // To do: utiliser un fichier JSON pour charger les levels
    public class Stage
    {
        // Service Locator
        private static readonly AssetManager am = ServiceLocator.GetService<AssetManager>();
        private static readonly ScreenInfo si = ServiceLocator.GetService<ScreenInfo>();

        public int NbStartingRows { get; private set; }
        public int Number { get; private set; }
        public int NbTotalRows { get; private set; }
        public float PercentBricksToDisplay { get; private set; }
        public float BrickSpeed {  get; private set; }
        public Color Color;

        public Stage(int number, int nbStartingRows, int nbTotalRows, float percentBricksToDisplay, float brickSpeed, Color color)
        {
            Number = number;
            NbStartingRows = nbStartingRows;
            NbTotalRows = nbTotalRows;
            PercentBricksToDisplay = percentBricksToDisplay;
            BrickSpeed = brickSpeed;
            Color = color;
        }
    }
}
