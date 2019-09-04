using AutoMapper;
using Kobe.Data.DBMapping;
using Kobe.Data.Repository;
using Kobe.Domain.Collections;
using Kobe.Domain.Dtos;
using Kobe.Service.Abstract;
using Kobe.Common.ViewModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Kobe.Common.Enums;

namespace Kobe.Service.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMongoDBRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<AppSettings> _appSettings;
        public MembershipService(IMongoDBRepository<User> userRepository, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _appSettings = appSettings;
        }

        #region CRUD
        public async Task<ResponseViewModel> DeleteUser(UserDto model)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var user = _mapper.Map<User>(model);
                var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
                Expression<Func<User, bool>> whereCondition = x => ((!string.IsNullOrEmpty(user.Id) && x.Id == user.Id));
                var resp = _userRepository.GetById(whereCondition).FirstOrDefault();
                resp.IsDeleted = true;
                var result = await _userRepository.Delete(filter, resp);
                if (result != null)
                {
                    model.IsDeleted = true;
                    response.Message = Constants.Delete;
                    response.ResponseData = model;
                    response.Status = (int)Numbers.One;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;

        }

        public bool GetSingleByUsernameorEmail(string email)
        {
            Expression<Func<User, bool>> whereCondition = x => ((!string.IsNullOrEmpty(email) && x.Email.ToLower() == email.ToLower()));
            return _userRepository.Exist(whereCondition);
        }

        public ResponseViewModel GetUserById(UserDto model)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var user = _mapper.Map<User>(model);
                Expression<Func<User, bool>> whereCondition = x => ((!string.IsNullOrEmpty(user.Id) && x.Id == user.Id));
                var result = _userRepository.GetById(whereCondition).AsQueryable().FirstOrDefault();
                if (result != null)
                {
                    var resultNew = _mapper.Map<UserDto>(result);
                    response.Message = Constants.Retreived;
                    response.ResponseData = resultNew;
                    response.Status = (int)Numbers.One;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<ResponseViewModel> RegisterUser(UserDto model)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var user = _mapper.Map<User>(model);
                var existingUser = GetSingleByUsernameorEmail(user.Email);
                if (!existingUser)
                {
                    if (model.Password != null)
                    {
                        byte[] passwordHash, passwordSalt;
                        CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);
                        user.HashedPassword = passwordHash; user.Salt = passwordSalt;
                        var result = await _userRepository.Add(user);
                        if (result != null)
                        {
                            model.Id = result.Id; model.CreatedDate = result.CreatedDate; model.ModifiedDate = result.ModifiedDate;
                            model.Password = null;
                            response.Message = "Registration successful";
                            response.ResponseData = model;
                            response.Status = (int)Numbers.One;
                        }
                        else
                        {
                            response.Message = Constants.Error;
                            response.Status = (int)Numbers.Zero;
                        }
                    }
                    else
                    {
                        response.Message = "Password is required";
                        response.Status = (int)Numbers.Zero;
                    }
                }
                else
                {
                    response.Message = "User already registered with email id provided";
                    response.Status = (int)Numbers.Zero;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public ResponseViewModel Login(UserDto model)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var user = _mapper.Map<User>(model);
                Expression<Func<User, bool>> whereCondition = x => ((!string.IsNullOrEmpty(user.Email)) && x.Email.ToLower()==user.Email.ToLower());
                var userObj = _userRepository.GetById(whereCondition).AsQueryable().FirstOrDefault();
                if (userObj == null)
                    response.Message = Constants.NotFound;
                if (!VerifyPasswordHash(model.Password, userObj.HashedPassword, userObj.Salt))
                    response.Message = "Invalid Password";
                else
                {
                    var result = _mapper.Map<UserDto>(userObj);
                    // Authenticate successful so generate jwt token
                    var userDto = BuildToken(result);
                    response.Message = Constants.Retreived;
                    response.Status = (int)Numbers.One;
                    response.ResponseData = userDto;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public ResponseViewModel GetAllUser()
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var result = _userRepository.GetAll().AsQueryable().ToList();
                if (result != null)
                {
                    response.Message = Constants.Retreived;
                    response.ResponseData = result;
                    response.Status = (int)Numbers.One;                    
                }              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<ResponseViewModel> UpdateUser(UserDto request)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var user = _mapper.Map<User>(request);
                var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
                Expression<Func<User, bool>> whereCondition = x => ((!string.IsNullOrEmpty(user.Id) && x.Id == user.Id));
                var model = _userRepository.GetById(whereCondition).FirstOrDefault();
                if (model != null)
                {
                    model.FirstName = user.FirstName ?? null;
                    model.LastName = user.LastName ?? null;
                    model.ModifiedDate = DateTime.UtcNow;
                    var result = await _userRepository.Update(filter, model);
                    if (result.ModifiedCount == (int)Numbers.One)
                    {
                        response.Message = Constants.Success;
                        response.ResponseData = request;
                        response.Status = (int)Numbers.One;
                    }
                    else if (result.ModifiedCount < (int)Numbers.One)
                    {
                        response.Message = Constants.Error;
                        response.Status = (int)Numbers.Zero;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        #endregion

        #region PasswordEncryption
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }

        #endregion

        #region JWT Token
        private UserDto BuildToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Value.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            return user;
        }

        #endregion
    }
}
