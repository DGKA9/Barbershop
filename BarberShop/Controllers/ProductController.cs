﻿using BarberShop.DTO;
using BarberShop.Entity;
using BarberShop.Unit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarberShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public ProductController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetProduct()
        {
            var product = _unitOfWork.ProductRepository.GetAll<ProductDto>();
            return Ok(product);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById<ProductDto>(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductDto productModel)
        {
            if (productModel == null)
                return BadRequest();

            var productEntity = _unitOfWork.Mapper.Map<Product>(productModel);
            _unitOfWork.ProductRepository.Add(productEntity);
            _unitOfWork.Commit();

            var productDto = _unitOfWork.Mapper.Map<ProductDto>(productEntity);

            return CreatedAtAction(nameof(GetProduct), new { id = productDto.proID }, productDto);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductDto updatedProductModel)
        {
            var existingProductEntity = _unitOfWork.ProductRepository.GetById<ProductDto>(id);

            if (existingProductEntity == null)
                return NotFound();

            _unitOfWork.ProductRepository.UpdateProperties(id, entity =>
            {
                entity.proName = updatedProductModel.proName;
                entity.proImage = updatedProductModel.proImage;
                entity.price = updatedProductModel.price;
                entity.quantity = updatedProductModel.quantity;
                entity.proDescription = updatedProductModel.proDescription;
                entity.producerID = updatedProductModel.producerID;
                entity.warehouseID = updatedProductModel.warehouseID;
                entity.category.cateID = updatedProductModel.cateID;

            });

            _unitOfWork.Commit();

            return Ok(new { message = "Cập nhật thành công" });
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var productEntity = _unitOfWork.ProductRepository.GetById<ProductDto>(id);

            if (productEntity == null)
                return NotFound();

            _unitOfWork.ProductRepository.Delete(id);
            _unitOfWork.Commit();

            return Ok(new { message = "Xóa thành công" });
        }


        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string productkey)
        {
            var productes = _unitOfWork.ProductRepository.Search<ProductDto>(product =>
                            product.proName != null &&
                            product.proName.Contains(productkey));
            return Ok(productes);
        }
    }
}
