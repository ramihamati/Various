namespace TracingAndApm.Controllers
{
    public interface IClientSecondApi
    {
        Task<ResponseCreateHub> CreateHub(RequestCreateHub request);
    }
}