using BepInEx.Configuration;
using Nautilus.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stats_Tracker
{
    public class OptionsMenu : ModOptions
    {
        public OptionsMenu() : base(Main.MODNAME)
        {
            AddItem(ConfigMenu.modEnabled.ToModToggleOption());
            AddItem(ConfigMenu.biomeName.ToModToggleOption());

            if (Language.main == null)
                return;
            else if (Language.main.currentLanguage == "English")
            {
                AddItem(ConfigMenu.fahrenhiet.ToModToggleOption());
                AddItem(ConfigMenu.miles.ToModToggleOption());
                AddItem(ConfigMenu.pounds.ToModToggleOption());
            }

        }
    }
}
