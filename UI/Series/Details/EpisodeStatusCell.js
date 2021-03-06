"use strict";

define(['app', 'Episode/Layout'], function () {
    NzbDrone.Series.Details.EpisodeStatusCell = Backgrid.Cell.extend({

        className: 'episode-status-cell',

        render: function () {
            this.$el.empty();

            if (this.model) {

                var icon;

                if (this.model.get('episodeFile')) {
                    icon = 'icon-ok';

                }
                else {
                    if (this.model.get('hasAired')) {
                        icon = 'icon-warning-sign';
                    }
                    else {
                        icon = 'icon-time';
                    }
                }

                this.$el.html('<i class="{0}"/>'.format(icon));
            }

            return this;
        }
    });
});
