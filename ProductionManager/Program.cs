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

    partial class Program : MyGridProgram
    {
        /// <summary>
        /// My Ini read from the programmable block's CustomData.
        /// </summary>
        private readonly MyIni ini = new MyIni();

        /// <summary>
        /// Command line instance.
        /// </summary>
        private readonly MyCommandLine commandLine = new MyCommandLine();

        /// <summary>
        /// Settings class.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Production Manager instance.
        /// </summary>
        private readonly ProductionManager productionManager;

        /// <summary>
        /// Dictionary of commands.
        /// </summary>
        private readonly Dictionary<string, Action<string, UpdateType>> Comands = new Dictionary<string, Action<string, UpdateType>>();

        private IEnumerator<bool> productionRunner;

        /// <summary>
        /// Instantiates the program.
        /// </summary>
        public Program()
        {
            if (this.ini.TryParse(this.Me.CustomData))
            {
                this.settings = new Settings(this.ini);
                this.productionManager = new ProductionManager(
                    this.GridTerminalSystem.GetBlockGroupWithName,
                    this.GridTerminalSystem.GetBlockWithName,
                    this.GridTerminalSystem.SearchBlocksOfName,
                    this.GridTerminalSystem.GetBlocksOfType,
                    this.GridTerminalSystem.GetBlocksOfType,
                    (string message) => this.Echo(message),
                    (string message) => this.Echo(message),
                    this.Me);

                this.Reinitialize();
            }
            else
            {
                throw new Exception("Unable to parse data.");
            }

            this.Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        /// <summary>
        /// Recreates the settings from the block's data and reintializes.
        /// </summary>
        public void Reinitialize()
        {
            if(this.ini.TryParse(this.Me.CustomData))
            {
                this.settings.Initialize(this.ini);
                this.productionManager.Initialize(this.settings);
            }
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (this.commandLine.TryParse(argument))
            {
                if (this.commandLine.Switch("update"))
                {
                    this.Reinitialize();
                    return;
                }
            }
            
            if ((updateSource & updateSource) == UpdateType.Once)
            {
                this.Continue();
            }
            else
            {
                this.productionRunner = this.productionManager.Run();
                this.Runtime.UpdateFrequency |= UpdateFrequency.Once;
            }
            
        }

        private void Continue()
        {
            if (this.productionRunner != null)
            {
                bool hasMoreSteps = this.productionRunner.MoveNext();

                if (hasMoreSteps)
                {
                    this.Runtime.UpdateFrequency |= UpdateFrequency.Once;
                }
                else
                {
                    this.productionRunner.Dispose();
                    this.productionRunner = null;
                }
            }
        }
    }
}
