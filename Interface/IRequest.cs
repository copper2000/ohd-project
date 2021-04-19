using OHD.Models;
using OHD.Dto;

namespace OHD.Interface
{
    public interface IRequest
    {
        Request AddOrEditRequest(UpsertRequest req);            
    }
}
