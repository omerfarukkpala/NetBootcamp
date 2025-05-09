﻿using System.Text.Encodings.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NetBootcamp.API.Controllers;
using NetBootcamp.API.DTOs;
using NetBootcamp.API.Products.DTOs;
using NetBootcamp.API.Products.ProductCreateUseCase;

namespace NetBootcamp.API.Products
{
    public class ProductsController : CustomBaseController
    {
        //private readonly IProductService _productService = ProductServiceFactory.GetService();

        private readonly IProductService _productService;

        private readonly IDataProtector _dataProtector;


        public ProductsController(IProductService productService, IDataProtectionProvider dataProtectionProvider)
        {
            _productService = productService;
            _dataProtector = dataProtectionProvider.CreateProtector("abc");
        }


        //baseUrl/api/products
        [HttpGet]
        public IActionResult GetAll([FromServices] PriceCalculator priceCalculator)
        {
            return Ok(_productService.GetAllWithCalculatedTax(priceCalculator));
        }

        [HttpGet("{productId:int}")]
        public IActionResult GetById(int productId, [FromServices] PriceCalculator priceCalculator)
        {
            return CreateActionResult(_productService.GetByIdWithCalculatedTax(productId, priceCalculator));
        }


        [HttpGet("page/{page:int}/pagesize/{pageSize:max(50)}")]
        public IActionResult GetAllByPage(int page, int pageSize, [FromServices] PriceCalculator priceCalculator)
        {
            return CreateActionResult(_productService.GetAllByPageWithCalculatedTax(priceCalculator, page, pageSize));
        }


        // complex type => class,record,struct => request body as Json
        // simple type => int,string,decimal => query string by default / route data

        [HttpPost]
        public IActionResult Create(ProductCreateRequestDto request)
        {
            var result = _productService.Create(request);

            return CreateActionResult(result, nameof(GetById), new { productId = result.Data });
        }


        [HttpPut("UpdateProductName")]
        public IActionResult UpdateProductName(ProductNameUpdateRequestDto request)
        {
            return CreateActionResult(_productService.UpdateProductName(request.Id, request.Name));
        }


        // PUT localhost/api/products/10
        [HttpPut("{productId:int}")]
        public IActionResult Update(int productId, ProductUpdateRequestDto request)
        {
            return CreateActionResult(_productService.Update(productId, request));
        }


        //// PUT api/products   
        //[HttpPut]
        //public IActionResult Update2(ProductUpdateRequestDto request)
        //{
        //    _productService.Update(request);

        //    return NoContent();
        //}

        [HttpDelete("{productId:int}")]
        public IActionResult Delete(int productId)
        {
            return CreateActionResult(_productService.Delete(productId));
        }

        [HttpGet("SifreleMustafa")]
        public IActionResult SifreleMustafa(string a)
        {
            var dataProtected = _dataProtector.ToTimeLimitedDataProtector();


            return Ok(Base64UrlEncoder.Encode(dataProtected.Protect(a, TimeSpan.FromSeconds(30))));
            // return Ok(_dataProtector.Protect(a));
        }


        [HttpGet("GetAllMustafa")]
        public IActionResult GetAllMustafa(string a)
        {
            var decode = Base64UrlEncoder.Decode(a);

            // var unProtectData = _dataProtector.Unprotect(a);
            var dataProtected = _dataProtector.ToTimeLimitedDataProtector();


            return Ok(dataProtected.Unprotect(decode));
        }
    }
}