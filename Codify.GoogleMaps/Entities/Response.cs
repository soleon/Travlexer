namespace Codify.GoogleMaps.Entities
{
    public class Response<T> : IResponse<T>
    {
        public StatusCodes Status { get; set; }

        public T Result { get; set; }
    }
}