using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Commands;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ReadSide.Products;
using ReadSide.Products.Repositories;
using Xer.Delegator;

namespace AspNetCoreApp.Controllers
{
    /// <summary>
    /// Products controller.
    /// </summary>
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IMessageDelegator _commandDelegator;
        private readonly IProductReadSideRepository _productRepository;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="commandDispatcher">Command dispatcher.</param>
        /// <param name="productReadSideRepository">Product read side repository.</param>
        public ProductsController(IMessageDelegator commandDispatcher, IProductReadSideRepository productReadSideRepository)
        {
            _productRepository = productReadSideRepository;
            _commandDelegator = commandDispatcher;
        }

        // GET api/products/{productId}
        /// <summary>
        /// Get a product by ID.
        /// </summary>
        /// <param name="productId">ID of product to retrieve.</param>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            // Get from read side.
            ProductReadModel product = await _productRepository.GetProductByIdAsync(productId);
            if(product != null)
            {
                return Ok(product);
            }

            return NotFound();
        }

        // POST api/products
        /// <summary>
        /// Register a new product.
        /// </summary>
        /// <param name="model">Product model.</param>
        [HttpPost]
        public async Task<IActionResult> RegisterProduct([FromBody]RegisterProductCommandDto model)
        {
            RegisterProductCommand command = model.ToDomainCommand();
            
            await _commandDelegator.SendAsync(command);
            return Ok();
        }
        
        // PUT api/products/{productId}
        /// <summary>
        /// Modify product.
        /// </summary>
        /// <param name="productId">ID of the product.</param>
        /// <param name="operation">
        /// <para>Operation to perform to the product:</para>
        /// <para>Valid values are:</para>
        /// <para>- ActivateProduct</para>
        /// <para>- DeactivateProduct</para>
        /// </param>
        /// <param name="payload">JSON payload, if available.</param>
        [HttpPut("{productId}")]
        public Task<IActionResult> ModifyProduct(int productId, [FromHeader]string operation, [FromBody]JObject payload)
        {
            switch(operation)
            {
                case ProductOperations.ActivateProduct:
                    return InternalActivateProduct(productId);
                case ProductOperations.DeactivateProduct:
                    return InternalDeactivateProduct(productId);
                default:
                    ModelState.AddModelError(nameof(operation), "Invalid operation header.");
                    return Task.FromResult<IActionResult>(BadRequest(ModelState));
            }
        }

        private async Task<IActionResult> InternalActivateProduct(int productId)
        {
            await _commandDelegator.SendAsync(new ActivateProductCommand(productId));
            return Ok();
        }

        private async Task<IActionResult> InternalDeactivateProduct(int productId)
        {
            await _commandDelegator.SendAsync(new DeactivateProductCommand(productId));
            return Ok();
        }

        /// <summary>
        /// RegisterProductCommandDto
        /// </summary>
        public class RegisterProductCommandDto
        {
            /// <summary>
            /// Product ID.
            /// </summary>
            public int ProductId { get; set; }

            /// <summary>
            /// Product Name.
            /// </summary>
            public string ProductName { get; set; }

            /// <summary>
            /// Convert DTO into a domain command.
            /// </summary>
            /// <returns>Instance of RegisterProductCommand.</returns>
            internal RegisterProductCommand ToDomainCommand()
            {
                return new RegisterProductCommand(ProductId, ProductName);
            }
        }

        class ProductOperations
        {
            public const string ActivateProduct = nameof(ActivateProduct);
            public const string DeactivateProduct = nameof(DeactivateProduct);
        }
    }
}