using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public interface ArnoldVinkLocation
    {
        //Get location coordinates
        Task<string[]> GetLocationCoordinates();
    }
}