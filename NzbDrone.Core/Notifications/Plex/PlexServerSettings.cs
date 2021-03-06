﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NzbDrone.Core.Annotations;

namespace NzbDrone.Core.Notifications.Plex
{
    public class PlexServerSettings : INotifcationSettings
    {
        [FieldDefinition(0, Label = "Host", HelpText = "Plex Server Host (IP or Hostname)")]
        public String Host { get; set; }

        [FieldDefinition(1, Label = "Update Library", HelpText = "Update Library on Download/Rename")]
        public Boolean UpdateLibrary { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Host);
            }
        }
    }
}
