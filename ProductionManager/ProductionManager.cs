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
    /// Partial program containing the production manager.
    /// </summary>
    partial class Program
    {
        /// <summary>
        /// Get Block with name delegate.
        /// </summary>
        /// <param name="name">Block name.</param>
        /// <returns>First block found with the name.</returns>
        public delegate IMyTerminalBlock GetBlockWithName(string name);

        /// <summary>
        /// Searches the grid system for blocks containing the name.
        /// </summary>
        /// <param name="name">Name to search.</param>
        /// <param name="blocks">List of blocks to populate.</param>
        /// <param name="collect">Filter function.</param>
        public delegate void SearchBlocksWithName(string name, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null);

        /// <summary>
        /// Gets the first blocks group with the name
        /// </summary>
        /// <param name="name">Name of block group.</param>
        /// <returns>First block group found.</returns>
        public delegate IMyBlockGroup GetBlockGroupWithName(string name);

        /// <summary>
        /// Get blocks of type from the grid system.
        /// </summary>
        /// <typeparam name="T">Type of block.</typeparam>
        /// <param name="blocks">List of blocks to populate.</param>
        /// <param name="collect">Filter function.</param>
        public delegate void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null);

        /// <summary>
        /// Log function for displaying log data to the programmable block's log.
        /// </summary>
        /// <param name="message">Message string.</param>
        public delegate void Log(string message);

        /// <summary>
        /// Stdout function for displaying output messages.
        /// </summary>
        /// <param name="message">Message string.</param>
        public delegate void Stdout(string message);

        /// <summary>
        /// Production manager.
        /// </summary>
        public class ProductionManager
        {
            /// <summary>
            /// Get block with name delegate.
            /// </summary>
            private readonly GetBlockWithName GetBlock;

            /// <summary>
            /// Get block group with name delegate.
            /// </summary>
            private readonly GetBlockGroupWithName GetBlockGroup;

            /// <summary>
            /// Search blocks with name delegate.
            /// </summary>
            private readonly SearchBlocksWithName SearchBlocks;

            /// <summary>
            /// Get assembler type delegate.
            /// </summary>
            private readonly GetBlocksOfType<IMyAssembler> GetAssemblers;

            /// <summary>
            /// Get cargo type delegate.
            /// </summary>
            private readonly GetBlocksOfType<IMyCargoContainer> GetCargoContainers;

            /// <summary>
            /// Log delegate.
            /// </summary>
            private readonly Log Echo;

            /// <summary>
            /// Stdout delegate.
            /// </summary>
            private readonly Stdout stdout;

            /// <summary>
            /// Terminal block list.
            /// </summary>
            private readonly List<IMyTerminalBlock> terminalBlocks = new List<IMyTerminalBlock>();

            /// <summary>
            /// Panel group list.
            /// </summary>
            private readonly List<PanelGroup> PanelGroups = new List<PanelGroup>();

            /// <summary>
            /// Assembler list.
            /// </summary>
            private readonly List<IMyAssembler> Assemblers = new List<IMyAssembler>();

            /// <summary>
            /// Cargo list.
            /// </summary>
            private readonly List<IMyCargoContainer> Cargo = new List<IMyCargoContainer>();

            /// <summary>
            /// Programmable block.
            /// </summary>
            private readonly IMyProgrammableBlock Me;

            /// <summary>
            /// Main assembler.
            /// </summary>
            private IMyAssembler MainAssembler;

            /// <summary>
            /// Settings instance.
            /// </summary>
            private Settings settings;

            /// <summary>
            /// Creates a new production manager.
            /// </summary>
            /// <param name="getBlockGroup">Get block group delegate.</param>
            /// <param name="getBlock">Get block delegate.</param>
            /// <param name="searchBlocks">Search blocks delegates.</param>
            /// <param name="getAssemblers">Get assemblers delegates.</param>
            /// <param name="getCargoContainers">Get cargo containers delegates.</param>
            /// <param name="echo">Echo delegate.</param>
            /// <param name="stdout">Stdout delegate.</param>
            /// <param name="me">Programmable block.</param>
            public ProductionManager(
                GetBlockGroupWithName getBlockGroup, 
                GetBlockWithName getBlock, 
                SearchBlocksWithName searchBlocks,
                GetBlocksOfType<IMyAssembler> getAssemblers,
                GetBlocksOfType<IMyCargoContainer> getCargoContainers,
                Log echo,
                Stdout stdout,
                IMyProgrammableBlock me)
            {
                this.GetBlock = getBlock;
                this.GetBlockGroup = getBlockGroup;
                this.SearchBlocks = searchBlocks;
                this.GetAssemblers = getAssemblers;
                this.GetCargoContainers = getCargoContainers;
                this.Echo = echo;
                this.stdout = stdout;
                this.Me = me;
            }

            /// <summary>
            /// Runs the manager for each component type, yielding with each type to allow for coroutines across game ticks.
            /// </summary>
            /// <returns>IEnumerator to spread calculations across ticks.</returns>
            public IEnumerator<bool> Run()
            {
                foreach(ComponentType type in Enum.GetValues(typeof(ComponentType)))
                {
                    this.Run(type);
                    yield return true;
                }
            }

            /// <summary>
            /// Runs the manager for a single component type.
            /// </summary>
            /// <param name="componentType">Component Type.</param>
            private void Run(ComponentType componentType)
            {
                ComponentSettings settings = this.settings[componentType];
                MyFixedPoint min = settings.Min;
                MyFixedPoint max = settings.Max;
                MyFixedPoint count = this.GetInventoryCount(componentType);

            }

            /// <summary>
            /// Initializes the Manager.
            /// </summary>
            /// <param name="settings">Settings from the Programmable block.</param>
            public void Initialize(Settings settings)
            {
                this.settings = settings;
                this.InitializePanels();
                this.InitializeAssemblers();
                this.InitializeInventory();
            }

            /// <summary>
            /// Gets the inventory count of the component type throughout the ship.
            /// </summary>
            /// <param name="type">Component type.</param>
            /// <returns>My Fixed Point.</returns>
            private MyFixedPoint GetInventoryCount(ComponentType type)
            {
                MyFixedPoint count = MyFixedPoint.Zero;
                foreach(IMyCargoContainer cargo in this.Cargo)
                {
                    IMyInventory inventory = cargo.GetInventory();
                    count += inventory.GetItemAmount(ComponentSettings.MakeItemType(type));
                }

                return count;
            }

            /// <summary>
            /// Initializes the display panels.
            /// </summary>
            private void InitializePanels()
            {
                this.PanelGroups.Clear();
                foreach (PanelGroupSettings panelGroupSettings in this.settings.PanelGroupSettings)
                {
                    this.terminalBlocks.Clear();
                    if (panelGroupSettings.SearchType == SearchTypes.Block)
                    {
                        IMyTerminalBlock block = this.GetBlock(panelGroupSettings.Name);
                        if (block == null || !block.IsSameConstructAs(this.Me))
                        {
                            continue;
                        }

                        this.PanelGroups.Add(new PanelGroup(block));
                    }
                    else if (panelGroupSettings.SearchType == SearchTypes.Group)
                    {
                        IMyBlockGroup group = this.GetBlockGroup(panelGroupSettings.Name);
                        if (group == null)
                        {
                            continue;
                        }
                        group.GetBlocks(this.terminalBlocks, b => b.IsSameConstructAs(this.Me));
                        this.PanelGroups.Add(new PanelGroup(this.terminalBlocks));
                    }
                    else
                    {
                        this.SearchBlocks(panelGroupSettings.Name, this.terminalBlocks, b => b.IsSameConstructAs(this.Me));
                        this.PanelGroups.Add(new PanelGroup(this.terminalBlocks));
                    }
                }
            }

            /// <summary>
            /// Initializes assemblers.
            /// </summary>
            private void InitializeAssemblers()
            {
                this.terminalBlocks.Clear();
                if (this.settings.Cooperative)
                {
                    IMyTerminalBlock block = this.GetBlock(this.settings.ParentAssemblerName);
                    if (block == null || !block.IsSameConstructAs(this.Me) || !(block is IMyAssembler))
                    {
                        throw new Exception("Unable to find Main Assembler.");
                    }

                    this.MainAssembler = (IMyAssembler)block;
                }

                if (!string.IsNullOrWhiteSpace(this.settings.AssemblerSearchString))
                {
                    this.SearchBlocks(this.settings.AssemblerSearchString, this.terminalBlocks, b => b.IsSameConstructAs(this.Me) && b is IMyAssembler && b.EntityId != this.MainAssembler.EntityId);
                    foreach (IMyTerminalBlock block in this.terminalBlocks)
                    {
                        this.Assemblers.Add((IMyAssembler)block);
                    }
                }
                else
                {

                    this.GetAssemblers(this.terminalBlocks, b => b.IsSameConstructAs(this.Me) && b.EntityId != this.MainAssembler.EntityId);
                    foreach (IMyTerminalBlock block in this.terminalBlocks)
                    {
                        this.Assemblers.Add((IMyAssembler)block);
                    }
                }
            }

            /// <summary>
            /// Initializes inventory.
            /// </summary>
            private void InitializeInventory()
            {
                this.terminalBlocks.Clear();
                this.Cargo.Clear();

                if (this.settings.InventorySearchType == SearchTypes.Block && !string.IsNullOrWhiteSpace(this.settings.InventoryName))
                {
                    IMyTerminalBlock block = this.GetBlock(this.settings.InventoryName);

                    if (block != null && block is IMyCargoContainer && block.IsSameConstructAs(this.Me))
                    {
                        this.Cargo.Add((IMyCargoContainer)block);
                    }
                }
                else if (this.settings.InventorySearchType == SearchTypes.Group && !string.IsNullOrWhiteSpace(this.settings.InventoryName))
                {
                    IMyBlockGroup group = this.GetBlockGroup(this.settings.InventoryName);

                    if (group != null)
                    {
                        group.GetBlocksOfType<IMyCargoContainer>(this.terminalBlocks, b => b.IsSameConstructAs(this.Me));
                        foreach (IMyTerminalBlock block in this.terminalBlocks)
                        {
                            this.Cargo.Add((IMyCargoContainer)block);
                        }
                    }
                }
                else if (this.settings.InventorySearchType == SearchTypes.Search)
                {
                    this.SearchBlocks(this.settings.InventoryName, this.terminalBlocks, b => b is IMyCargoContainer && b.IsSameConstructAs(this.Me));
                    foreach (IMyTerminalBlock block in this.terminalBlocks)
                    {
                        this.Cargo.Add((IMyCargoContainer)block);
                    }
                }
                else
                {
                    this.GetCargoContainers(this.terminalBlocks, b => b.IsSameConstructAs(this.Me));
                    foreach (IMyTerminalBlock block in this.terminalBlocks)
                    {
                        this.Cargo.Add((IMyCargoContainer)block);
                    }
                }
            }
        }
    }
}
