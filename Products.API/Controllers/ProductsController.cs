using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductsController(ILogger<ProductsController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get([FromQuery] string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                var categoryID = _categoryRepository.GetAll().FirstOrDefault(x => x.Name.ToLower() == categoryName.ToLower()).ID;

                return Ok(_productRepository.GetAll().Where(x => x.CategoryID == categoryID));
            }

            return Ok(_productRepository.GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Product> Get(Guid id)
        {
            return Ok(_productRepository.GetSingle(u => u.ID == id));
        }

        [HttpPost]
        public ActionResult<Product> Create([FromBody] Product product)
        {
            return Ok(_productRepository.Add(product));
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<Product> Update([FromBody] Product product)
        {
            return Ok(_productRepository.Edit(product));
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult Remove(Guid id)
        {
            _productRepository.Delete(_productRepository.GetSingle(u => u.ID == id));

            return NoContent();
        }
    }
}
