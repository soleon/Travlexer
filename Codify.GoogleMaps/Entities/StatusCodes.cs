namespace Codify.GoogleMaps.Entities
{
    public enum StatusCodes : byte
    {
        UNKNOWN_ERROR = 0,
        OK,
        ZERO_RESULTS,
        OVER_QUERY_LIMIT,
        REQUEST_DENIED,
        INVALID_REQUEST,
        MAX_WAYPOINTS_EXCEEDED,
        NOT_FOUND
    }
}