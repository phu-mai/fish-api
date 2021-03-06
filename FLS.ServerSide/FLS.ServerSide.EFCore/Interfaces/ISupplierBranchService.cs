﻿using FLS.ServerSide.EFCore.Entities;
using FLS.ServerSide.SharingObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLS.ServerSide.EFCore.Services
{
    public interface ISupplierBranchService
    {
        Task<PagedList<SupplierBranch>> GetList(PageFilterModel _model);
        Task<SupplierBranch> GetDetail(int _id);
        Task<int> Add(SupplierBranch _model);
        Task<bool> Modify(SupplierBranch _model);
        Task<bool> Remove(int _id);
    }
}
