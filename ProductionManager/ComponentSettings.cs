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
    /// Program class to wrap Component Settings
    /// </summary>
    partial class Program
    {
        /// <summary>
        /// Type of component.
        /// </summary>
        public enum ComponentType
        {
            BulletproofGlass,
            Canvas,
            Computer,
            Construction,
            Detector,
            Display,
            Explosives,
            Girder,
            GravityGenerator,
            InteriorPlate,
            LargeTube,
            Medical,
            MetalGrid,
            Motor,
            PowerCell,
            RadioCommunication,
            Reactor,
            SmallTube,
            SolarCell,
            SteelPlate,
            Superconductor,
            Thrust,
            ZoneChip
        }


        /// <summary>
        /// Settings for a single component type.
        /// </summary>
        public class ComponentSettings
        {
            private const string ComponentBuilderPrefix = "MyObjectBuilder_Component";

            public static MyItemType MakeItemType(ComponentType type)
            {
                return MyItemType.MakeComponent($"{ComponentBuilderPrefix}/{type}");
            }
            /// <summary>
            /// Gets the Component Type.
            /// </summary>
            public ComponentType Type { get; }

            /// <summary>
            /// Gets or sets the component setting's display name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the max value.
            /// </summary>
            public int Max { get; set; }

            /// <summary>
            /// Gets or sets the min value.
            /// </summary>
            public int Min { get; set; }

            /// <summary>
            /// Creates a new instance of the component settings class.
            /// </summary>
            /// <param name="type">Component type.</param>
            public ComponentSettings(ComponentType type) : 
                this(type, type.ToString())
            {
            }

            /// <summary>
            /// Creates a new instance of the component settings class.
            /// </summary>
            /// <param name="type">Component type.</param>
            /// <param name="name">Display name.</param>
            public ComponentSettings(ComponentType type, string name)
            {
                this.Type = type;
                this.Name = name;
            }
        }
    }
}
