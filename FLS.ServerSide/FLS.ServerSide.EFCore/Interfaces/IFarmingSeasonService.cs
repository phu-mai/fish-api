﻿using FLS.ServerSide.EFCore.Entities;
using FLS.ServerSide.SharingObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLS.ServerSide.EFCore.Services
{
    public interface IFarmingSeasonService
    {
        Task<PagedList<FarmingSeason>> GetList(PageFilterModel _model);
        Task<FarmingSeason> GetDetail(int _id);
        Task<int> Add(FarmingSeason _model, bool _isSaveChange = true);
        Task<bool> Modify(FarmingSeason _model, bool _isSaveChange = true);
        Task<bool> Remove(int _id, bool _isSaveChange = true);
    }
}