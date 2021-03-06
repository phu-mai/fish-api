﻿using AutoMapper;
using FLS.ServerSide.Business.Interfaces;
using FLS.ServerSide.EFCore.Entities;
using FLS.ServerSide.EFCore.Services;
using FLS.ServerSide.Model;
using FLS.ServerSide.Model.Scope;
using FLS.ServerSide.SharingObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLS.ServerSide.Business.Biz
{
    public class StockIssueDocketBusiness : IStockIssueDocketBusiness
    {
        private static FLSDbContext context;
        private static IScopeContext scopeContext;
        private readonly IStockIssueDocketService svcStockIssueDocket;
        private readonly IStockIssueDocketDetailService svcStockIssueDocketDetail;
        private readonly IStockIssueDocketTypeService svcStockIssueDocketType;
        private readonly IExpenditureDocketService svcExpenditureDocket;
        private readonly IExpenditureDocketDetailService svcExpenditureDocketDetail;
        private readonly ICurrentInStockService svcCurrentInStock;
        private readonly IMapper iMapper;
        public StockIssueDocketBusiness(
            FLSDbContext _context,
            IScopeContext _scopeContext,
            IStockIssueDocketService _svcStockIssueDocket,
            IStockIssueDocketDetailService _svcStockIssueDocketDetail,
            IStockIssueDocketTypeService _svcStockIssueDocketType,
            IExpenditureDocketService _svcExpenditureDocket,
            IExpenditureDocketDetailService _svcExpenditureDocketDetail,
            ICurrentInStockService _svcCurrentInStock,
            IMapper _iMapper)
        {
            context = _context;
            scopeContext = _scopeContext;
            svcStockIssueDocket = _svcStockIssueDocket;
            svcStockIssueDocketDetail = _svcStockIssueDocketDetail;
            svcStockIssueDocketType = _svcStockIssueDocketType;
            svcExpenditureDocket = _svcExpenditureDocket;
            svcExpenditureDocketDetail = _svcExpenditureDocketDetail;
            svcCurrentInStock = _svcCurrentInStock;
            iMapper = _iMapper;
        }
        public async Task<PagedList<StockIssueDocketModel>> GetList(PageFilterModel _model)
        {
            return iMapper.Map<PagedList<StockIssueDocketModel>>(await svcStockIssueDocket.GetList(_model));
        }
        public async Task<ExportStockDetailModel> GetDetail(int _id)
        {
            ExportStockDetailModel result = new ExportStockDetailModel();
            StockIssueDocketModel issueDocket = iMapper.Map<StockIssueDocketModel>(await svcStockIssueDocket.GetDetail(_id));
            if (issueDocket == null)
            {
                scopeContext.AddError("Mã phiếu xuất không tồn tại");
                return null;
            }
            result.IssueDocket = issueDocket;
            List<StockIssueDocketDetailModel> details = iMapper.Map<List<StockIssueDocketDetailModel>>(await svcStockIssueDocketDetail.GetDetailsByDocketId(_id));
            result.Details = details;
            return result;
        }
        public async Task<int> Add(ExportStockModel _model)
        {
            if (_model == null || _model.IssueDocket == null || _model.DocketDetails == null)
            {
                scopeContext.AddError("Lỗi dữ liệu đầu vào");
                return 0;
            }
            if (_model.IssueDocket.StockIssueDocketTypeId <= 0)
            {
                scopeContext.AddError("Chưa chọn loại phiếu nhập");
                return 0;
            }
            if (_model.IssueDocket.WarehouseId <= 0)
            {
                scopeContext.AddError("Chưa chọn kho nhập");
                return 0;
            }
            // lấy loại phiếu thu
            var issueDocketType = await svcStockIssueDocketType.GetDetail(_model.IssueDocket.StockIssueDocketTypeId);
            var receiptType = 0;
            if (issueDocketType != null && issueDocketType.ReceiptNeeded)
                receiptType = issueDocketType.ReceiptTypeId;
                
            // bắt đầu tạo phiếu
            using (var transaction = context.Database.BeginTransaction())
            {
                StockIssueDocket issueDocket = iMapper.Map<StockIssueDocket>(_model.IssueDocket);
                issueDocket.CustomerId = _model.Receipt.PartnerId;
                issueDocket.CustomerName = _model.Receipt.PartnerName;
                ExpenditureDocket receipt = null;
                if (receiptType > 0)
                {
                    receipt = iMapper.Map<ExpenditureDocket>(_model.Receipt);
                    receipt.Amount = issueDocket.Amount;
                    receipt.Vat = issueDocket.Vat;
                    receipt.WarehouseId = issueDocket.WarehouseId;
                    receipt.CreatedUser = scopeContext.UserCode;
                    receipt.IsReceipt = true;
                }
                List<StockIssueDocketDetail> docketDetails = iMapper.Map<List<StockIssueDocketDetail>>(_model.DocketDetails);
                List<ExpenditureDocketDetail> expendDetails = new List<ExpenditureDocketDetail>();
                decimal orderVAT = 0;
                decimal orderAmount = 0;
                decimal orderTotalAmount = 0;

                List<ProductInstockModel> productInstock = new List<ProductInstockModel>();
                foreach (var item in docketDetails)
                {
                    if (receiptType > 0)
                    {
                        item.Amount = item.Quantity * item.UnitPrice;
                        item.Vat = item.Amount * (item.VatPercent / (decimal)100);
                        item.TotalAmount = item.Amount + item.Vat;
                        ExpenditureDocketDetail exDetail = new ExpenditureDocketDetail
                        {
                            ExpenditureDocketId = receipt.Id,
                            Amount = item.Amount,
                            Vat = item.Vat,
                            TotalAmount = item.TotalAmount
                        };
                        expendDetails.Add(exDetail);
                        orderVAT += item.Vat;
                        orderAmount += item.Amount;
                        orderTotalAmount += item.TotalAmount;
                    }
                    else
                    {
                        item.UnitPrice = 0;
                        item.Amount = 0;
                        item.Vat = 0;
                        item.TotalAmount = 0;
                    }

                    #region Trừ vào danh sách tồn - Tạm thời chưa chuyển đổi sang số lượng theo đơn vị tính chuẩn
                    var idx = productInstock.FindIndex(p => p.ProductId == item.ProductId);
                    if (idx >= 0)
                        productInstock[idx].Quantity += item.Quantity;
                    else
                    {
                        productInstock.Add(new ProductInstockModel()
                        {
                            ProductId = item.ProductId,
                            ProductName = "",
                            Quantity = item.Quantity,
                            ProductUnitId = item.ProductUnitId
                        });
                    }
                    #endregion
                }
                // insert
                if (receiptType > 0)
                {
                    issueDocket.Vat = orderVAT;
                    issueDocket.Amount = orderAmount;
                    issueDocket.TotalAmount = orderTotalAmount;
                    issueDocket.Id = await svcStockIssueDocket.Add(issueDocket);

                    receipt.Vat = orderVAT;
                    receipt.Amount = orderAmount;
                    receipt.TotalAmount = orderTotalAmount;
                    receipt.StockDocketId = issueDocket.Id;
                    receipt.Id = await svcExpenditureDocket.Add(receipt);

                }
                else
                {
                    issueDocket.Vat = 0;
                    issueDocket.Amount = 0;
                    issueDocket.TotalAmount = 0;
                    issueDocket.Id = await svcStockIssueDocket.Add(issueDocket);
                }
                foreach (var item in docketDetails)
                {
                    item.StockIssueDocketId = issueDocket.Id;
                    item.Id = await svcStockIssueDocketDetail.Add(item);
                }
                foreach (var item in expendDetails)
                {
                    item.ExpenditureDocketId = receipt.Id;
                    item.Id = await svcExpenditureDocketDetail.Add(item);
                }

                #region Cập nhật vào bảng tồn
                foreach (var current in productInstock)
                {
                    var instock = await svcCurrentInStock.GetList(issueDocket.WarehouseId, current.ProductId);
                    if (instock == null || instock.Count == 0)
                    {
                        CurrentInStock cis = new CurrentInStock()
                        {
                            Amount = 0 - current.Quantity,
                            AmountExpect = 0 - current.Quantity,
                            ProductId = current.ProductId,
                            ProductUnitId = current.ProductUnitId,
                            WarehouseId = issueDocket.WarehouseId
                        };
                        cis.Id = await svcCurrentInStock.Add(cis);
                    }
                    else
                    {
                        CurrentInStock cis = instock[0];
                        cis.Amount -= current.Quantity;
                        cis.AmountExpect -= current.Quantity;
                        await svcCurrentInStock.Modify(cis);
                    }
                }
                #endregion
                transaction.Commit();
                return issueDocket.Id;
            }
        }
        public async Task<bool> Modify(int _id, StockIssueDocketModel _model)
        {
            StockIssueDocket entity = await svcStockIssueDocket.GetDetail(_id);
            if (entity == null) return false;
            entity = iMapper.Map(_model, entity);
            return await svcStockIssueDocket.Modify(entity);
        }
        public async Task<bool> Remove(int _id)
        {
            return await svcStockIssueDocket.Remove(_id);
        }
    }
}
