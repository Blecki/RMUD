﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace Space
{
	public class settings : RMUD.Settings
	{
        public settings()
        {
            NewPlayerStartRoom = "Start";
            PlayerBaseObject = "Player";
        }
	}
}
