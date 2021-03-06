﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLS.ServerSide.Business.Interfaces;
using FLS.ServerSide.Model.Scope;
using FLS.ServerSide.SharingObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FLS.ServerSide.API.Controllers
{
    [AllowAnonymous]
    [Route("api/product-subgroups")]
    public class ProductSubgroupController : Controller
    {
        IConfiguration config;
        IScopeContext context;
        IProductSubgroupBusiness busProductSubgroup;
        public ProductSubgroupController(IConfiguration _config, IScopeContext _scopeContext, IProductSubgroupBusiness _busProductSubgroup)
        {
            config = _config;
            context = _scopeContext;
            busProductSubgroup = _busProductSubgroup;
        }
        [HttpPost("")]
        public async Task<IActionResult> Search([FromBody]PageFilterModel _model)
        {
            var result = await busProductSubgroup.GetList(_model);
            return Ok(context.WrapResponse(result));
        }
        [HttpGet("{_id}")]
        public async Task<IActionResult> Get(int _id)
        {
            var result = await busProductSubgroup.GetDetail(_id);
            return Ok(context.WrapResponse(result));
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody]ProductSubgroupModel _model)
        {
            var result = await busProductSubgroup.Add(_model);
            return Ok(context.WrapResponse(result));
        }
        [HttpPut("{_id}/modify")]
        public async Task<IActionResult> Modify(int _id, [FromBody]ProductSubgroupModel _model)
        {
            var result = await busProductSubgroup.Modify(_id, _model);
            return Ok(context.WrapResponse(result));
        }
        [HttpDelete("{_id}/remove")]
        public async Task<IActionResult> Remove(int _id)
        {
            var result = await busProductSubgroup.Remove(_id);
            return Ok(context.WrapResponse(result));
        }
        [HttpPost("{_id}/products")]
        public async Task<IActionResult> ListProduct(int _id, [FromBody]PageFilterModel _model)
        {
            var result = await busProductSubgroup.GetProducts(_id, _model);
            return Ok(context.WrapResponse(result));
        }
    }
}
