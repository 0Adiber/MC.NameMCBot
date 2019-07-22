using NameMCBot.Tasks;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameMCBot
{
    [Plugin(1, "NameMCBot", "This Plugin is a NameMC Bot.", "adiber.at")]
    public class Bot : IStartPlugin
    {

        // Must be overriden by every plugin.
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            //this.Setting.Add(new StringSetting("Authorised Users", "Users that are allowed to use this Plugin by the Chat command", "Adiber,MrHunh"));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            // Called once the plugin is ticked in the plugin tab.
            if (!botSettings.loadChat) return new PluginResponse(false, "'Load chat' must be enabled.");

            return new PluginResponse(true);
        }

        public override void OnDisable()
        {
            // Called once the plugin is unticked.
            // (Note: does not get called if the plugin is stopped from different sources, such as macros)
            Console.WriteLine("Plugin disabled");
        }

        public override void OnStart()
        {
            // This should be used to register the tasks for the bot, see below.
            // (Note: called after 'OnEnable')

            RegisterTask(new Query());

        }

        public override void OnStop()
        {
            // Called once the plugin is stopped.
            // (Note: unlike 'OnDisabled' this gets triggered from other sources, not only plugins tab)
        }
    }
}
