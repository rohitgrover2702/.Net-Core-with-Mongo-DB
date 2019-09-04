using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kobe.Domain.Collections;
using Kobe.Domain.Dtos;
using Kobe.Service.Abstract;
using Kobe.Common.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using static Kobe.Common.Enums;

namespace Kobe.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        public AccountController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        #region CRUD
        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var result = _membershipService.GetAllUser();
            return getCommonStatus(result);

        }

        [HttpPost]
        [Route("GetUserById")]
        public IActionResult GetUserById([FromBody] UserDto model)
        {
            var result = _membershipService.GetUserById(model);
            return getCommonStatus(result);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto model)
        {
            var result = await _membershipService.RegisterUser(model);
            return getCommonStatus(result);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] UserDto model)
        {
            var result = _membershipService.Login(model);
            return getCommonStatus(result);
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] UserDto user)
        {
            var result = await _membershipService.DeleteUser(user);
            return getCommonStatus(result);
        }
        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto user)
        {
            var result = await _membershipService.UpdateUser(user);
            return getCommonStatus(result);
        }

        private IActionResult getCommonStatus(ResponseViewModel response)
        {
            if (response.Status == (int)Numbers.One)
                return Ok(response);
            else
                return BadRequest(response);
        }
        #endregion

    }
}