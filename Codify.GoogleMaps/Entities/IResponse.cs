namespace Codify.GoogleMaps.Entities
{
    public interface IResponse<T>
    {
        StatusCodes Status { get; set; }

        T Result { get; set; }
    }
}