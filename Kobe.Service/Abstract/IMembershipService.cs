using Kobe.Domain.Collections;
using Kobe.Domain.Dtos;
using Kobe.Common.ViewModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kobe.Service.Abstract
{
    public interface IMembershipService
    {
        Task<ResponseViewModel> RegisterUser(UserDto user);
        ResponseViewModel Login(UserDto user);
        bool GetSingleByUsernameorEmail(string email);
        ResponseViewModel GetAllUser();
        ResponseViewModel GetUserById(UserDto user);
        Task<ResponseViewModel> DeleteUser(UserDto user);
        Task<ResponseViewModel> UpdateUser(UserDto user);
    }
}
