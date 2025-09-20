# Rate Limiting for CSP Reports

## Overview

Rate limiting has been implemented for the CSP Report action to prevent abuse and reduce noise in logs from excessive CSP violation reports.

## Implementation

The rate limiting is implemented using a custom `RateLimitAttribute` that:

- Tracks requests globally (not per IP address)
- Uses in-memory storage for high performance
- Supports configuration through web.config appSettings only
- Automatically cleans up old tracking data

## Configuration

Rate limiting is configured exclusively through web.config appSettings.

### Web.config appSettings

Add these settings to your `web.config` file in the `<appSettings>` section:

```xml
<appSettings>
  <add key="CSPReportRateLimitEnabled" value="true" />
  <add key="CSPReportRateLimitMaxRequests" value="5" />
  <add key="CSPReportRateLimitTimeWindowMinutes" value="1" />
</appSettings>
```

### Available Settings

- `CSPReportRateLimitEnabled` (Boolean, default: true) - Enable/disable rate limiting
- `CSPReportRateLimitMaxRequests` (Integer, default: 5) - Maximum requests per time window
- `CSPReportRateLimitTimeWindowMinutes` (Integer, default: 1) - Time window in minutes

## Default Settings

- **Enabled**: Yes
- **Max Requests**: 5 requests
- **Time Window**: 1 minute

This means by default, the system can receive at most 5 CSP reports per minute globally. After exceeding this limit, subsequent requests will receive a 429 (Too Many Requests) response.

## Storage

Rate limiting data is stored in memory using static concurrent collections for high performance. Old entries are automatically cleaned up after 1 hour, and the cleanup occurs periodically during request processing.

## Usage Example

```csharp
[HttpPost]
[RateLimit()] // Uses CSPReport configuration by default
public ActionResult Report()
{
    // Your action implementation
}

// Or with custom settings
[HttpPost]
[RateLimit(maxRequests: 10, timeWindowMinutes: 5)]
public ActionResult CustomAction()
{
    // Your action implementation
}
```

## Error Handling

- If there are any errors during rate limit checking, the request is allowed (fail-open approach)
- All errors are logged for monitoring
- Memory-based storage eliminates file system related errors
