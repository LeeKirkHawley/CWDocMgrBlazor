using Microsoft.AspNetCore.Mvc;

namespace CWDocMgrBlazor.Api.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
