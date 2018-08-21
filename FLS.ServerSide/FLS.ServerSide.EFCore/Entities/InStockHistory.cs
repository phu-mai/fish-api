﻿using System;
using System.Collections.Generic;

namespace FLS.ServerSide.EFCore.Entities
{
    public partial class InStockHistory
    {
        public int Id { get; set; }
        public DateTime MonthCode { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public decimal AmountExpect { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedUser { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
