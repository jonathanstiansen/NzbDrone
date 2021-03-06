﻿using System.Collections.Generic;
using AutoMapper;
using Nancy;
using NzbDrone.Api.Extensions;
using NzbDrone.Core.Qualities;

namespace NzbDrone.Api.Qualities
{
    public class QualityProfilesModule : NzbDroneApiModule
    {
        private readonly QualityProfileService _qualityProvider;

        public QualityProfilesModule(QualityProfileService qualityProvider)
            : base("/QualityProfiles")
        {
            _qualityProvider = qualityProvider;
            Get["/"] = x => OnGet();
            Get["/{Id}"] = x => OnGet((int)x.Id);
            Put["/"] = x => OnPut();
            Delete["/{Id}"] = x => OnDelete((int)x.Id);
        }

        private Response OnGet()
        {
            var profiles = _qualityProvider.All();
            return Mapper.Map<List<QualityProfile>, List<QualityProfileResource>>(profiles).AsResponse();
        }

        private Response OnGet(int id)
        {
            var profile = _qualityProvider.Get(id);
            return Mapper.Map<QualityProfile, QualityProfileResource>(profile).AsResponse();
        }

        private Response OnPost()
        {
            var request = Request.Body.FromJson<QualityProfileResource>();
            var profile = Mapper.Map<QualityProfileResource, QualityProfile>(request);
            request.Id = _qualityProvider.Add(profile).Id;

            return request.AsResponse();
        }

        //Update
        private Response OnPut()
        {
            var request = Request.Body.FromJson<QualityProfileResource>();
            var profile = Mapper.Map<QualityProfileResource, QualityProfile>(request);
            _qualityProvider.Update(profile);

            return request.AsResponse();
        }

        private Response OnDelete(int id)
        {
            _qualityProvider.Delete(id);
            return new Response();
        }
    }
}