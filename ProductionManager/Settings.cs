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
    /// Partial Program class to wrap the settings class.
    /// </summary>
    partial class Program
    {
        /// <summary>
        /// Search Types.
        /// </summary>
        public enum SearchTypes
        {
            /// <summary>
            /// Single block
            /// </summary>
            Block,

            /// <summary>
            /// Multiple blocks.
            /// </summary>
            Search,

            /// <summary>
            /// Blocks within a group.
            /// </summary>
            Group,

            /// <summary>
            /// Blocks with a type.
            /// </summary>
            Type
        }

        /// <summary>
        /// Handle parsing and creating the settings.
        /// </summary>
        public class Settings
        {
            /// <summary>
            /// Component settings dictionary.
            /// </summary>
            private readonly Dictionary<ComponentType, ComponentSettings> componentSettings = new Dictionary<ComponentType, ComponentSettings>();

            /// <summary>
            /// Panel Group settings list.
            /// </summary>
            private readonly List<PanelGroupSettings> panelGroupSettings = new List<PanelGroupSettings>();

            /// <summary>
            /// Section names list.
            /// </summary>
            private readonly List<string> sectionNames = new List<string>();
            
            /// <summary>
            /// Creates ne instance of the settings class.
            /// </summary>
            /// <param name="ini">My Ini.</param>
            public Settings(MyIni ini)
            {
                this.Initialize(ini);
            }

            /// <summary>
            /// Gets the settings for a specific type.
            /// </summary>
            /// <param name="type">Component type.</param>
            /// <returns>Component setttings.</returns>
            public ComponentSettings this[ComponentType type]
            {
                get
                {
                    return this.componentSettings[type];
                }
            }

            /// <summary>
            /// Gets a value indicating whether the assemblers should be in cooperative mode.
            /// </summary>
            public bool Cooperative { get; private set; }

            /// <summary>
            /// Gets the parent assembler name for use with cooperative mode.
            /// </summary>

            public string ParentAssemblerName { get; private set; }

            /// <summary>
            /// Gets the search string for other assemblers. Uses all assemblers on the grid if this is blank.
            /// </summary>

            public string AssemblerSearchString { get; private set; }

            /// <summary>
            /// Gets the inventory name or search string.
            /// </summary>
            public string InventoryName { get; private set; }

            /// <summary>
            /// Gets the inventory search type.
            /// </summary>
            public SearchTypes InventorySearchType { get; private set; }

            /// <summary>
            /// Gets the panel group settings list.
            /// </summary>
            public List<PanelGroupSettings> PanelGroupSettings
            {
                get
                {
                    return this.panelGroupSettings;
                }
            }

            /// <summary>
            /// Initializes the setting class or resets it with new values.
            /// </summary>
            /// <param name="ini"></param>
            public void Initialize(MyIni ini)
            {
                /**
                 * [bulletproofglass]
                 * name=Bulletproof Glass
                 * max=1000
                 * min=500
                 * ...
                 * 
                 * [assemblers]
                 * cooperative=true
                 * name=Main Assembler Name
                 * search=<optional>Search String for child assemblers
                 * 
                 * OR
                 * 
                 * [assemblers]
                 * search=<optional>Search String for Assemblers
                 * 
                 * [inventory (optional)]
                 * name=Name, Group or Search term
                 * type=block | search | group
                 * 
                 * [blockNameSearchOrGroup]
                 * type=block | search | group
                 * 
                 * 
                 */
                this.sectionNames.Clear();
                ini.GetSections(this.sectionNames);
                this.InitializeComponentSettings(ini);
                this.InitializeAssemblers(ini);
                this.InitializeInventory(ini);
                this.InitializeDisplayPanels(ini);
            }

            /// <summary>
            /// Initializes the Display Panels.
            /// </summary>
            /// <param name="ini">My Ini</param>
            private void InitializeDisplayPanels(MyIni ini)
            {
                this.panelGroupSettings.Clear();
                ComponentType type;
                foreach(string section in this.sectionNames)
                {
                    if (section != "assemblers" && section != "inventory" && !Enum.TryParse(section, true, out type))
                    {
                        SearchTypes searchType;
                        if (Enum.TryParse(ini.Get(section, "type").ToString(""), true, out searchType))
                        {
                            this.panelGroupSettings.Add(new PanelGroupSettings()
                            {
                                Name = section,
                                SearchType = searchType
                            });
                        }
                    }
                }
            }

            /// <summary>
            /// Initializes the inventory.
            /// </summary>
            /// <param name="ini">My Ini.</param>
            private void InitializeInventory(MyIni ini)
            {
                if (ini.ContainsSection("inventory"))
                {
                    this.InventoryName = ini.Get("inventory", "name").ToString(null);

                    SearchTypes searchTypes;
                    if (Enum.TryParse(ini.Get("inventory", "type").ToString("type"), true, out searchTypes)) 
                    {
                        this.InventorySearchType = searchTypes;
                    }
                    else
                    {
                        this.InventorySearchType = SearchTypes.Type;
                    }
                }
                else
                {
                    this.InventoryName = null;
                    this.InventorySearchType = SearchTypes.Type;
                }
            }

            /// <summary>
            /// Initializes the Assemblers
            /// </summary>
            /// <param name="ini">My INI</param>
            private void InitializeAssemblers(MyIni ini)
            {
                this.Cooperative = ini.Get("assemblers", "cooperative").ToBoolean(false);

                if (this.Cooperative)
                {
                    this.ParentAssemblerName = ini.Get("assemblers", "name").ToString(null);

                    if (string.IsNullOrWhiteSpace(this.ParentAssemblerName))
                    {
                        throw new Exception("Name is required for cooperative assemblers");
                    }
                }

                this.AssemblerSearchString = ini.Get("assemblers", "search").ToString(null);
            }

            /// <summary>
            /// Initializes the component settings dictionary.
            /// </summary>
            /// <param name="ini">My INI</param>
            private void InitializeComponentSettings(MyIni ini)
            {

                foreach (string section in this.sectionNames)
                {
                    ComponentType type;
                    if (Enum.TryParse(section, true, out type))
                    {
                        string displayName = ini.Get(section, "name").ToString(type.ToString());
                        int max = ini.Get(section, "max").ToInt32(0);
                        int min = ini.Get(section, "min").ToInt32(0);
                        if (this.componentSettings.ContainsKey(type))
                        {
                            this.componentSettings[type].Name = displayName;
                            this.componentSettings[type].Max = max;
                            this.componentSettings[type].Min = min;
                        }
                        else
                        {
                            this.componentSettings.Add(type, new ComponentSettings(type, displayName)
                            {
                                Max = max,
                                Min = min
                            });
                        }
                    }
                }

                foreach (ComponentType type in Enum.GetValues(typeof(ComponentType)))
                {
                    if (!this.componentSettings.ContainsKey(type))
                    {
                        this.componentSettings.Add(type, new ComponentSettings(type));
                    }
                }
            }
        }
    }
}
