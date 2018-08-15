﻿using System;
using System.Collections.Generic;

namespace FLS.ServerSide.EFCore.Entities
{
    public partial class CapitalCost
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime Month { get; set; }
        public decimal? PreviousCost { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedUser { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}