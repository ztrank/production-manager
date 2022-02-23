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

    partial class Program
    {
        /// <summary>
        /// Individual block containing a panel.
        /// </summary>
        public class Panel
        {
            /// <summary>
            /// Block reference.
            /// </summary>
            private readonly IMyTerminalBlock Block;

            /// <summary>
            /// My Ini.
            /// </summary>
            private readonly MyIni ini = new MyIni();

            /// <summary>
            /// Panel Settings.
            /// </summary>
            private readonly PanelSettings panelSettings = new PanelSettings();

            /// <summary>
            /// Initialize the panel.
            /// </summary>
            /// <param name="block"></param>
            public Panel(IMyTerminalBlock block)
            {
                this.Block = block;
                
                if (this.ini.TryParse(block.CustomData))
                {
                    this.panelSettings.Initialize(this.ini);
                }
            }
        }

        /// <summary>
        /// Panel group.
        /// </summary>
        public class PanelGroup
        {
            /// <summary>
            /// List of panels.
            /// </summary>
            private readonly List<Panel> Panels = new List<Panel>();

            /// <summary>
            /// Creates a panel group with a single panel.
            /// </summary>
            /// <param name="block">Block with a panel.</param>
            public PanelGroup(IMyTerminalBlock block)
            {
                this.Panels.Add(new Panel(block));
            }

            /// <summary>
            /// Creates a panel group with multiple panels.
            /// </summary>
            /// <param name="blocks">Blocks with panels.</param>
            public PanelGroup(List<IMyTerminalBlock> blocks)
            {
                foreach(IMyTerminalBlock block in blocks)
                {
                    this.Panels.Add(new Panel(block));
                }
            }
        }
    }
}
