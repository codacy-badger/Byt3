﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Byt3.ADL;
using Byt3.ExtPP.Base.Interfaces;
using Byt3.ExtPP.Base.Plugins;
using Byt3.ExtPP.Base.settings;
using Utils = Byt3.ExtPP.Base.Utils;

namespace Byt3.ExtPP
{
    /// <summary>
    /// A class that keeps track on what scripts are loaded and their processing state.
    /// This class also defines a Compute Scheme to alter the keys the file gets matched with, to enable loading the same file multiple times.
    /// </summary>
    public class SourceManager : ALoggable<PPLogType>, ISourceManager
    {
        /// <summary>
        /// List of Scripts that are included in this Processing run
        /// </summary>
        private readonly List<ISourceScript> sources = new List<ISourceScript>();

        /// <summary>
        /// The processing states of the scripts included.
        /// </summary>
        private readonly List<ProcessStage> doneState = new List<ProcessStage>();

        /// <summary>
        /// The compute scheme that is used to assign keys to scripts(or instances of scripts)
        /// </summary>
        private DelKeyComputingScheme computeScheme;

        /// <summary>
        /// Empty Constructor
        /// Sets the compute scheme to the default(the file name)
        /// </summary>
        public SourceManager(List<AbstractPlugin> pluginChain)
        {
            SetComputingScheme(ComputeFileNameAndKey_Default);
        }


        /// <summary>
        /// Sets the computing scheme to a custom scheme that will then be used to assign keys to scripts
        /// </summary>
        /// <param name="scheme">The delegate that will be used to determine the key and path in the source manager</param>
        public void SetComputingScheme(DelKeyComputingScheme scheme)
        {
            if (scheme == null)
            {
                return;
            }
            computeScheme = scheme;
            Logger.Log(PPLogType.Log, Verbosity.Level2, "Changed Computing Scheme to: {0}", scheme.Method.Name);
        }

        /// <summary>
        /// Returns the Queued items that are waiting for computation
        /// </summary>
        /// <returns>Size of the internal queue</returns>
        public int GetTodoCount()
        {
            return doneState.Count(x => x == ProcessStage.Queued);
        }

        /// <summary>
        /// Returns the computing scheme
        /// </summary>
        /// <returns>the computing scheme</returns>
        public DelKeyComputingScheme GetComputingScheme()
        {
            return computeScheme;
        }

        /// <summary>
        /// The default implementation of the key matching calculation
        /// </summary>
        /// <param name="vars">The import string in a source script</param>
        /// <param name="currentPath">the current path of the preprocessor</param>
        /// <returns>A result object.</returns>
        private ImportResult ComputeFileNameAndKey_Default(string[] vars, string currentPath)
        {
            ImportResult ret = new ImportResult();

            if (!Utils.TryResolvePathIncludeParameter(vars))
            {
                return ret;
            }

            string rel = Path.Combine(currentPath, vars[0]);
            string key = Path.GetFullPath(rel);


            ret.SetValue("filename", key);


            ret.SetValue("key", key);
            ret.SetResult(true);

            return ret;
        }

        /// <summary>
        /// Returns the next item that can be processed
        /// if no items left returns null
        /// </summary>
        public ISourceScript NextItem
        {
            get
            {
                for (int i = 0; i < doneState.Count; i++)
                {
                    if (doneState[i] == ProcessStage.Queued)
                    {
                        return sources[i];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Fixes the order of the file tree if a script was being loaded and is now referenced (again)
        /// by removing it from the lower position and readding it at the top
        /// </summary>
        /// <param name="script">The script that got referenced.</param>
        public void FixOrder(ISourceScript script)
        {
            Logger.Log(PPLogType.Log, Verbosity.Level3, "Fixing Build Order of file: {0}",
                Path.GetFileName(script.GetFileInterface().GetKey()));
            int idx = IndexOfFile(script.GetKey());
            ISourceScript a = sources[idx];
            ProcessStage ab = doneState[idx];
            doneState.RemoveAt(idx);
            doneState.Add(ab);
            sources.RemoveAt(idx);
            AddFile(a, true);
        }


        /// <summary>
        /// Returns true if the scripts key is contained in the manager
        /// </summary>
        /// <param name="script">The script to check for</param>
        /// <returns>True if the script is included.</returns>
        public bool IsIncluded(ISourceScript script)
        {
            return sources.Any(x => x.GetKey() == script.GetKey());
        }

        /// <summary>
        /// Adds a script to the to do list of the source manager.
        /// Will do nothing if already included
        /// </summary>
        /// <param name="script">The script to enqueue for computation</param>
        public void AddToTodo(ISourceScript script)
        {
            if (!IsIncluded(script))
            {
                Logger.Log(PPLogType.Log, Verbosity.Level3, "Adding Script to Todo List: {0}",
                    Path.GetFileName(script.GetFileInterface().GetKey()));
                AddFile(script, false);
                doneState.Add(ProcessStage.Queued);
            }
        }

        /// <summary>
        /// Sets the processing state of the script to done
        /// it will not be returned by the NextItem property.
        /// </summary>
        /// <param name="script">The script to set the stage for</param>
        public void SetState(ISourceScript script, ProcessStage stage)
        {
            if (IsIncluded(script))
            {
                doneState[IndexOfFile(script.GetKey())] = stage;

                Logger.Log(PPLogType.Log, Verbosity.Level3, "Finished Script: {0}",
                    Path.GetFileName(script.GetFileInterface().GetKey()));
            }
        }

        /// <summary>
        /// Returns the List of Scripts that are in this Source Manager object
        /// </summary>
        /// <returns>The internal list of all scripts.</returns>
        public List<ISourceScript> GetList()
        {
            return sources;
        }


        /// <summary>
        /// Adds a file to a list while checking for the key
        /// </summary>
        /// <param name="script">The file to be added.</param>
        /// <param name="checkForExistingKey">A flag to optionally check if the key of the file is already existing</param>
        private void AddFile(ISourceScript script, bool checkForExistingKey)
        {
            if (checkForExistingKey && ContainsFile(script.GetKey()))
            {
                return;
            }
            sources.Add(script);

        }

        /// <summary>
        /// Returns true when the source manager contains a script with the key specified
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <returns>true if the file is contained in the source manager</returns>
        private bool ContainsFile(string key)
        {
            return IndexOfFile(key) != -1;
        }

        /// <summary>
        /// Returns the index of the file with the matching key
        /// returns -1 when the key is not present
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <returns>the index of the file or -1 if not found</returns>
        public int IndexOfFile(string key)
        {
            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i].GetKey() == key)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool lockScriptCreation = true;

        public void SetLock(bool state)
        {
            lockScriptCreation = state;
        }

        /// <summary>
        /// Convenience wrapper to create a source script without knowing the actual type of the script.
        /// </summary>
        /// <param name="separator">the separator used.</param>
        /// <param name="file">the path of the file</param>
        /// <param name="key">the key of the file</param>
        /// <param name="importInfo">the import info of the key and path importation</param>
        /// <returns>the success state of the operation</returns>
        public bool TryCreateScript(out ISourceScript script, string separator, IFileContent file,
            ImportResult importInfo)
        {
            if (lockScriptCreation)
            {
                script = null;
                Logger.Log(PPLogType.Warning, Verbosity.Level3,
                    "A Plugin is trying to add a file outside of the main stage. Is the configuration correct?");
                return false;
            }

            script = new SourceScript(separator, file, importInfo);
            return true;
        }
    }
}