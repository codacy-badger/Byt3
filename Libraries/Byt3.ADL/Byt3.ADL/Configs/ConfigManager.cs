﻿using System;
using System.IO;
using System.Xml.Serialization;

namespace Byt3.ADL.Configs
{
    /// <summary>
    ///     Contains the code for saving and loading Config files in this project
    /// </summary>
    public static class ConfigManager
    {
        private static readonly ADLLogger<LogType> Logger = new ADLLogger<LogType>("ConfigManager");

        /// <summary>
        ///     The field of the serializer that gets used.
        /// </summary>
        private static XmlSerializer Serializer;

        public static T GetDefault<T>() where T : AbstractADLConfig
        {
            return (T) Activator.CreateInstance<T>().GetStandard();
        }

        /// <summary>
        ///     Reads a config of type T from file.
        /// </summary>
        /// <typeparam name="T">Type of Config</typeparam>
        /// <param name="path">Path to config</param>
        /// <returns>Deserialized Config File.</returns>
        public static T ReadFromFile<T>(string path) where T : AbstractADLConfig
        {
            T ret;
            Serializer = new XmlSerializer(typeof(T));
            if (!File.Exists(path))
            {
                Logger.Log(LogType.Warning, "Config Manager: File" + path + "does not exist");
                return GetDefault<T>();
            }

            try
            {
                FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
                ret = (T) Serializer.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
                ret = GetDefault<T>();
                Logger.Log(LogType.Warning,
                    "Config Manager: Failed to deserialize XML file. Either XML file is corrupted or file access is denied.");
            }

            return ret;
        }

        /// <summary>
        ///     Saves the specified Config File of type T at the supplied filepath
        /// </summary>
        /// <typeparam name="T">Type of config</typeparam>
        /// <param name="path">path to config file.</param>
        /// <param name="data">config object></param>
        public static void SaveToFile<T>(string path, T data) where T : AbstractADLConfig
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                Serializer = new XmlSerializer(typeof(T));
                FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write);
                Serializer.Serialize(fs, data);
                fs.Close();
            }
            catch (Exception)
            {
                Logger.Log(LogType.Warning,
                    "Config Manager: Failed to save xml file. Directory exists? Access to Write to directory?");
            }
        }
    }
}