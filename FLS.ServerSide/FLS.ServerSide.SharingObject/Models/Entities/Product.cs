﻿using System;
using System.Collections.Generic;

namespace FLS.ServerSide.SharingObject
{
    public partial class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductGroupId { get; set; }
        public int ProductSubgroupId { get; set; }
        public int DefaultUnitId { get; set; }
        public int TaxPercent { get; set; }
        public string Description { get; set; }
    }
}
