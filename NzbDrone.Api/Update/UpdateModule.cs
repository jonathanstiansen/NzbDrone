﻿using System;
using System.Collections.Generic;
using NzbDrone.Api.REST;
using NzbDrone.Core.Update;
using NzbDrone.Api.Mapping;

namespace NzbDrone.Api.Update
{
    public class UpdateModule : NzbDroneRestModule<UpdateResource>
    {
        private readonly IUpdateService _updateService;

        public UpdateModule(IUpdateService updateService)
        {
            _updateService = updateService;
            GetResourceAll = GetAvailableUpdate;
        }

        private List<UpdateResource> GetAvailableUpdate()
        {
            var update = _updateService.AvailableUpdate();
            var response = new List<UpdateResource>();

            if (update != null)
            {
                response.Add(update.InjectTo<UpdateResource>());
            }

            return response;
        }
    }

    public class UpdateResource : RestResource
    {
        public Version Version { get; set; }
        public String FileName { get; set; }
        public String Url { get; set; }
    }
}