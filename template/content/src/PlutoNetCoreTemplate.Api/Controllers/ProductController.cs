﻿namespace PlutoNetCoreTemplate.Api.Controllers
{
    using Application.AppServices.ProductAppServices;
    using Application.Command;
    using Application.Models.ProductModels;
    using Application.Permissions;

    using Domain.Aggregates.TenantAggregate;

    using Infrastructure.Commons;

    using MediatR;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(ProductPermission.Product.Default)]
    public class ProductController : BaseController<ProductController>
    {

        private readonly IProductAppService _productAppService;
        private readonly ICurrentTenant _currentTenant;

        public ProductController(IMediator mediator, ILogger<ProductController> logger, IProductAppService productAppService, ICurrentTenant currentTenant) : base(mediator, logger)
        {
            _productAppService = productAppService;
            _currentTenant = currentTenant;
        }

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ServiceResponse<List<ProductModels>>> GetAsync()
        {
            var list = await _productAppService.GetListAsync();
            return ServiceResponse<List<ProductModels>>.Success(list);
        }

        /// <summary>
        /// 获取产品
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ServiceResponse<ProductModels>> GetAsync(string id)
        {
            var model = await _productAppService.GetAsync(id);
            return ServiceResponse<ProductModels>.Success(model);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Authorize(ProductPermission.Product.Create)]
        public async Task<ServiceResponse<bool>> Post([FromBody] CreateProductCommand request)
        {
            await _mediator.Send(request);
            return ServiceResponse<bool>.Success(true);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        [Authorize(ProductPermission.Product.Edit)]
        public ServiceResponse<bool> Put(int id, [FromBody] string value)
        {
            return ServiceResponse<bool>.Success(true);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        [Authorize(ProductPermission.Product.Delete)]
        public ServiceResponse<bool> Delete(string id)
        {
            return ServiceResponse<bool>.Success(true);
        }

    }
}
