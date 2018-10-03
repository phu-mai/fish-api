﻿using FLS.ServerSide.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FLS.ServerSide.SharingObject;
using FLS.ServerSide.Model.Scope;

namespace FLS.ServerSide.EFCore.Services
{
    public class ProductSubgroupService : IProductSubgroupService
    {
        private static FLSDbContext context;
        private static IScopeContext scopeContext;
        public ProductSubgroupService(FLSDbContext _context, IScopeContext _scopeContext)
        {
            context = _context;
            scopeContext = _scopeContext;
        }
        public async Task<PagedList<ProductSubgroup>> GetList(PageFilterModel _model)
        {
            _model.Key = string.IsNullOrWhiteSpace(_model.Key) ? null : _model.Key.Trim();
            int filter = 0;
            if (_model.Filters != null && _model.Filters.Count > 0 && _model.Filters[0].Key == FilterEnum.ProductGroup)
            {
                int.TryParse(_model.Filters[0].Value + "", out filter);
            }
            var items = await context.ProductSubgroup.Where(i => 
                        i.IsDeleted == false
                        && (_model.Key == null || i.Name.Contains(_model.Key))
                        && (filter == 0 || i.ProductGroupId == filter)
                    ).OrderByDescending(i => i.UpdatedDate.HasValue ? i.UpdatedDate : i.CreatedDate).GetPagedList(_model.Page, _model.PageSize);
            return items;
        }
        public async Task<ProductSubgroup> GetDetail(int _id)
        {
            var item = await context.ProductSubgroup
                        .FirstOrDefaultAsync(x => 
                            x.Id == _id 
                            && x.IsDeleted == false
                        );
            return item;
        }
        public async Task<int> Add(ProductSubgroup _model)
        {
            _model.CreatedUser = scopeContext.UserCode;
            _model.CreatedDate = DateTime.Now;
            context.Add(_model);
            await context.SaveChangesAsync();
            return _model.Id;
        }
        public async Task<bool> Modify(ProductSubgroup _model)
        {
            _model.UpdatedUser = scopeContext.UserCode;
            _model.UpdatedDate = DateTime.Now;
            context.Update(_model);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Remove(int _id)
        {
            ProductSubgroup item = await context.ProductSubgroup
                                    .Where(i => 
                                        i.Id == _id 
                                        && i.IsDeleted == true
                                    ).FirstOrDefaultAsync();
            if (item == default(ProductSubgroup))
                return false;
            item.IsDeleted = true;
            context.Entry(item).Property(x => x.IsDeleted).IsModified = true;
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<PagedList<Product>> GetProducts(int _subgroupId, PageFilterModel _model)
        {
            _model.Key = string.IsNullOrWhiteSpace(_model.Key) ? null : _model.Key.Trim();
            var items = await context.Product.Where(i => 
                        i.ProductSubgroupId == _subgroupId 
                        && i.IsDeleted == false
                        && (_model.Key == null || i.Name.Contains(_model.Key))
                    ).OrderByDescending(i => i.UpdatedDate.HasValue ? i.UpdatedDate : i.CreatedDate).GetPagedList(_model.Page, _model.PageSize);
            return items;
        }
        public async Task<List<ProductSubgroup>> GetCache()
        {
            var items = await context.ProductSubgroup.Where(i => i.IsDeleted == false).ToListAsync();
            return items;
        }
    }
}
