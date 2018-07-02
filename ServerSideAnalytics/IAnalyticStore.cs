﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerSideAnalytics
{
    public interface IAnalyticStore
    {
        Task AddAsync(WebRequest request);

        Task<long> CountUniqueAsync(DateTime day);

        Task<long> CountUniqueAsync(DateTime from, DateTime to);

        Task<long> CountAsync(DateTime from, DateTime to);

        Task<IEnumerable<string>> IpAddressesAsync(DateTime day);

        Task<IEnumerable<string>> IpAddressesAsync(DateTime from, DateTime to);

        Task<IEnumerable<WebRequest>> RequestByIdentityAsync(string identity);
    }
}