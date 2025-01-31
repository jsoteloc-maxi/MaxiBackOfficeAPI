using Maxi.BackOffice.Agent.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiBackOfficeAPI.Controllers
{
    [Authorize] // TODO Revisar la configuración de autorización
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Obtiene la imagen del cliente
        /// </summary>
        /// <param name="idCustomer"></param>
        /// <param name="imageType"></param>
        /// <returns></returns>
        [Route("Image")]
        public Byte[] GetImage(int idCustomer, string imageType = "")
        {
            return _customerService.GetImage(idCustomer, imageType);
        }
    }
}
