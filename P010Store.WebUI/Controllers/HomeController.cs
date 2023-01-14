using Microsoft.AspNetCore.Mvc;
using P010Store.Entities;
using P010Store.Service.Abstract;
using P010Store.WebUI.Models;
using System.Diagnostics;

namespace P010Store.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IService<Product> _service;
        private readonly IService<Carousel> _serviceCar;
        private readonly IService<Brand> _serviceBrand;

        public HomeController(IService<Product> service, IService<Carousel> serviceCar, IService<Brand> serviceBrand)
        {
            _service = service;
            _serviceCar = serviceCar;
            _serviceBrand = serviceBrand;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var model=new HomeViewModel()
            {
                Carousels=await _serviceCar.GetAllAsync(),
                Products=await _service.GetAllAsync(p=>p.IsHome),
                Brands=await _serviceBrand.GetAllAsync()
            };
            //var model=await _service.GetAllAsync(p=>p.IsHome);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}