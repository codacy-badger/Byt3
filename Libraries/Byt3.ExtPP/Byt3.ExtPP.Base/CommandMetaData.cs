﻿using System;
using System.Xml.Serialization;

namespace Byt3.ExtPP.Base
{
    /// <summary>
    /// A Struct that contains all the information about the plugin
    /// </summary>
    [Serializable]
    public class CommandMetaData
    {
        /// <summary>
        /// The help text of the Command
        /// </summary>
        [XmlElement]
        public string HelpText { get; set; }

        /// <summary>
        /// The shortcut for the command
        /// Can be accessed with -
        /// </summary>
        [XmlElement]
        public string ShortCut { get; set; }

        /// <summary>
        /// If this parameter can be set by a global prefix
        /// </summary>
        [XmlElement]
        public bool IncludeGlobal { get; set; }

        /// <summary>
        /// The command.
        /// Can be accessed with --
        /// </summary>
        [XmlElement]
        public string Command { get; set; }

        public CommandMetaData()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="shortcut"></param>
        /// <param name="helpText"></param>
        /// <param name="global"></param>
        public CommandMetaData(string command, string shortcut, string helpText, bool global)
        {

            Command = command;
            HelpText = helpText;
            ShortCut = shortcut;
            IncludeGlobal = global;
        }

        /// <summary>
        /// Writes the meta data as readable text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Command + "(" + ShortCut + "): " + HelpText;
        }
    }
}