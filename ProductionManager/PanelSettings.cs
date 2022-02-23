namespace IngameScript
{
    using Sandbox.Game.EntityComponents;
    using Sandbox.ModAPI.Ingame;
    using Sandbox.ModAPI.Interfaces;
    using SpaceEngineers.Game.ModAPI.Ingame;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using VRage;
    using VRage.Collections;
    using VRage.Game;
    using VRage.Game.Components;
    using VRage.Game.GUI.TextPanel;
    using VRage.Game.ModAPI.Ingame;
    using VRage.Game.ModAPI.Ingame.Utilities;
    using VRage.Game.ObjectBuilders.Definitions;
    using VRageMath;

    /// <summary>
    /// Program class with Panel Settings
    /// </summary>
    partial class Program
    {
        /// <summary>
        /// Panel settings.
        /// </summary>
        public class PanelSettings
        {
            /// <summary>
            /// Creates new instance.
            /// </summary>
            public PanelSettings()
            {
            }

            /// <summary>
            /// Creates a new isntance of the panel settings class with the given ini.
            /// </summary>
            /// <param name="ini">My ini.</param>
            public PanelSettings(MyIni ini)
            {
                this.Initialize(ini);
            }

            /// <summary>
            /// Initializes the setting class.
            /// </summary>
            /// <param name="ini"></param>
            public void Initialize(MyIni ini)
            {
                this.DisplayIndex = ini.Get("production", "index").ToInt32(0);
                this.GroupIndex = ini.Get("production", "groupIndex").ToInt32(0);
            }

            /// <summary>
            /// Gets or sets the display index.
            /// </summary>
            public int DisplayIndex { get; set; }

            /// <summary>
            /// Gets or sets the index this panel is within the group.
            /// </summary>
            public int GroupIndex { get; set; }
        }
    }
}
