using OHD.Dto;
using OHD.Models;
using OHD.Models.ViewModels;
using System.Linq;

namespace OHD.Interface
{
    public interface IImplementer
    {
        IQueryable<ListRequestResponse> GetListAssignedRequest(int userId);
        Request UpdateRequestStatus(int requestId, RequestViewModel requestViewModel);
    }
}
