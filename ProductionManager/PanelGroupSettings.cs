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
    /// Program class wrapping Panel Group Settings.
    /// </summary>
    partial class Program
    {
        /// <summary>
        /// Panel Group settings for finding a group of panels.
        /// </summary>
        public class PanelGroupSettings
        {
            /// <summary>
            /// Gets or sets the search term.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the search type.
            /// </summary>
            public SearchTypes SearchType { get; set; }
        }
    }
}
