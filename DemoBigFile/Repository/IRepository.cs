﻿using DemoBigFile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBigFile.Repository
{
    public interface IRepository
    {
        void Create(DonationViewModel model);
        void MergeTable();
        Task DemoRelationalDataInsert(DataTable product, DataTable tableProductVariant);
    }
}
