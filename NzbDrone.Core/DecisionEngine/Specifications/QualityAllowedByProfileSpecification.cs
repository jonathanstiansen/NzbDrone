using NLog;
using NzbDrone.Core.Model;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.DecisionEngine.Specifications
{
    public class QualityAllowedByProfileSpecification : IDecisionEngineSpecification
    {
        private readonly Logger _logger;

        public QualityAllowedByProfileSpecification(Logger logger)
        {
            _logger = logger;
        }

        public string RejectionReason
        {
            get
            {
                return "Quality rejected by series profile";
            }
        }

        public virtual bool IsSatisfiedBy(RemoteEpisode subject)
        {
            _logger.Trace("Checking if report meets quality requirements. {0}", subject.ParsedEpisodeInfo.Quality);
            if (!subject.Series.QualityProfile.Value.Allowed.Contains(subject.ParsedEpisodeInfo.Quality.Quality))
            {
                _logger.Trace("Quality {0} rejected by Series' quality profile", subject.ParsedEpisodeInfo.Quality);
                return false;
            }

            return true;
        }
    }
}
