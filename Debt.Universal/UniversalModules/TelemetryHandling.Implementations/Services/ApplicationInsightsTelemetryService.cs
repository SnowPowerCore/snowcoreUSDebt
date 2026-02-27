using Debt.TelemetryHandling.Interfaces;
using Microsoft.ApplicationInsights;

namespace Debt.TelemetryHandling.Implementations.Services;

public class ApplicationInsightsTelemetryService(TelemetryClient telemetryImpl) : ITelemetryService
{
    public void TrackException(Exception exception, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null) =>
        telemetryImpl.TrackException(exception, properties, metrics);

    public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null) =>
        telemetryImpl.TrackEvent(eventName, properties, metrics);
}