﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BetterTriggers.Logging
{
    internal class ErrorDTO
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string TargetSite { get; set; }
        public Version AppVersion { get; set; }

        internal ErrorDTO(Exception e)
        {
            Message = e.Message;
            StackTrace = e.StackTrace;
            TargetSite = e.TargetSite != null ? e.TargetSite.Name : string.Empty;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            AppVersion = new Version(version);
        }
    }
}
