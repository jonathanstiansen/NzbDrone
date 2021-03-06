﻿"use strict";
define(['app'], function () {
    NzbDrone.Series.SeasonModel = Backbone.Model.extend({

        mutators: {
            seasonTitle: function () {
                var seasonNumber = this.get('seasonNumber');

                if (seasonNumber === 0) {
                    return "Specials";
                }

                return "Season " + seasonNumber;
            }
        },

        defaults: {
            seasonNumber: 0
        }
    });
});

