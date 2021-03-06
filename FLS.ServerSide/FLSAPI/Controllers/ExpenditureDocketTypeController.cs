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
    [Route("api/expenditure-docket-types")]
    public class ExpenditureDocketTypeController : Controller
    {
        IConfiguration config;
        IScopeContext context;
        IExpenditureDocketTypeBusiness busExpenditureDocketType;
        public ExpenditureDocketTypeController(IConfiguration _config, IScopeContext _scopeContext, IExpenditureDocketTypeBusiness _busExpenditureDocketType)
        {
            config = _config;
            context = _scopeContext;
            busExpenditureDocketType = _busExpenditureDocketType;
        }
        [HttpPost("")]
        public async Task<IActionResult> Search([FromBody]PageFilterModel _model)
        {
            var result = await busExpenditureDocketType.GetList(_model);
            return Ok(context.WrapResponse(result));
        }
        [HttpGet("{_id}")]
        public async Task<IActionResult> Get(int _id)
        {
            var result = await busExpenditureDocketType.GetDetail(_id);
            return Ok(context.WrapResponse(result));
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody]ExpenditureDocketTypeModel _model)
        {
            var result = await busExpenditureDocketType.Add(_model);
            return Ok(context.WrapResponse(result));
        }
        [HttpPut("{_id}/modify")]
        public async Task<IActionResult> Modify(int _id, [FromBody]ExpenditureDocketTypeModel _model)
        {
            var result = await busExpenditureDocketType.Modify(_id, _model);
            return Ok(context.WrapResponse(result));
        }
        [HttpDelete("{_id}/remove")]
        public async Task<IActionResult> Remove(int _id)
        {
            var result = await busExpenditureDocketType.Remove(_id);
            return Ok(context.WrapResponse(result));
        }
    }
}
