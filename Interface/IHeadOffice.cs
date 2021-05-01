using OHD.Dto;
using OHD.Models;
using OHD.Models.ViewModels;
using System.Linq;

namespace OHD.Interface
{
    public interface IHeadOffice
    {
        IQueryable<ListRequestResponse> GetListIncomingRequest(int requestorId);
        Request AssignRequest(int requestId, RequestViewModel requestViewModel);
    }
}
