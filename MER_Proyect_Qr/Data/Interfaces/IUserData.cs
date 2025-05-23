﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.DTOs.Mostrar;
using Entity.Model;

namespace Data.Interfaces
{
    //public interface IUserData
    //{
    //    Task<IEnumerable<MostrarUserDto>> GetAllAsyncSQL();
    //    Task<MostrarUserDto?> GetByIdAsync(int id);
    //    Task<User> CreateAsync(User user);
    //    Task<bool> UpdateAsync(User user);
    //    Task<bool> DeleteLogicAsync(int id);
    //    Task<bool> DeletePersistenceAsync(int id);
    //}

    public interface IUserData<TEntity, TDto>
    {
        Task<IEnumerable<TDto>> GetAllAsyncSQL();
        Task<TDto?> GetByIdAsync(int id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteLogicAsync(int id);
        Task<bool> DeletePersistenceAsync(int id);
    }

}
