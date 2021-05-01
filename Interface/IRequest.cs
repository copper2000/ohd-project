using OHD.Models;
using OHD.Dto;
using System.Linq;
using OHD.Models.ViewModels;

namespace OHD.Interface
{
    public interface IRequest
    {
        Request AddRequest(RequestViewModel requestViewModel, int requestorId);
        Request EditRequest(int requestId, RequestViewModel requestViewModel);
        bool CloseRequest(int requestId, int requestorId, RequestViewModel requestViewModel);
        bool DeleteRequest(int requestId);
        IQueryable<ListRequestResponse> GetListRequest(int requestorId);
        IQueryable<ListRequestResponse> GetListHistoryRequest(int requestorId);
    }
}
