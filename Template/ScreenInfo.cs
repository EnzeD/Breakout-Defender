using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnzedSpace
{
    public class ScreenInfo
    {
        private GraphicsDeviceManager _GDM;

        public int Width
        {
            get
            {
                return _GDM.PreferredBackBufferWidth;
            }
        }
        public int Height
        {
            get
            {
                return _GDM.PreferredBackBufferHeight;
            }
        }
        public ScreenInfo(GraphicsDeviceManager pGDM) 
        {
            _GDM = pGDM;
        }

        public int targetW = 1280;
        public int targetH = 720;
    }
}
