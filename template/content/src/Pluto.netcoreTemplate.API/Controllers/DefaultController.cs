﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pluto.netcoreTemplate.API.Models.Requests;
using Pluto.netcoreTemplate.Application.Commands;
using Pluto.netcoreTemplate.Application.Queries.UserQueries;
using Pluto.netcoreTemplate.Domain.Events.UserEvents;
using Pluto.netcoreTemplate.Infrastructure.Providers;
using Serilog.Context;

namespace Pluto.netcoreTemplate.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DefaultController> _logger;
        private readonly EventIdProvider _eventIdProvider;
        private readonly UserQuery userQuery;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        /// <param name="eventIdProvider"></param>
        public DefaultController(
            IMediator mediator, 
            ILogger<DefaultController> logger, 
            EventIdProvider eventIdProvider, 
            UserQuery userQuery)
        {
            _mediator = mediator;
            _logger = logger;
            _eventIdProvider = eventIdProvider;
            this.userQuery = userQuery;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserRequest request)
        {
            _logger.LogInformation(_eventIdProvider.EventId, "CreateUser请求。request={@request}", request);
            var res = await _mediator.Send(new CreateUserCommand(request.UserName,request.Tel));
            _logger.LogInformation(_eventIdProvider.EventId, "CreateUser结果。result={@result}",res);
            if (res)
            {
                return Ok(new { IsError = false , Msg = "创建成功" }) ;
            }
            return Ok(new {IsError = true, Msg = "创建失败"});
        }


        /// <summary>
        /// 获取一个
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(int id)
        {
            var res = await userQuery.GetByIdAsync(id);
            return Ok(new { IsError = false, Data = res });
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<IActionResult> GetList(int index=1,int size=20)
        {
            var res= await userQuery.GetListAsync(index, size);
            return Ok(new { IsError = false, total = res.total, Data = res.list });
        }
    }

}