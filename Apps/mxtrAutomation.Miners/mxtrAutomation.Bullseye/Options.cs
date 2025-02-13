using System;
using CommandLine;
using CommandLine.Text;

namespace mxtrAutomation.Bullseye
{
    public class Options
    {
        [ValueOption(0)]
        public string Mode { get; set; }

        [Option('s', "start", Required = false, HelpText = "The start datetime (utc).")]
        public DateTime Start { get; set; }
        [Option('e', "end", Required = false, HelpText = "The end datetime (utc).")]
        public DateTime End { get; set; }
        [Option('a', "account", Required = false, HelpText = "Specify an account by ID.")]
        public String AccountId { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("Bullseye Data Miner"),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("Usage: Bullseye Data Miner <mode-type>");
            help.AddPreOptionsLine("Mode Types: allaccounts, single");

            help.AddOptions(this);
            return help;
        }
    }
}
