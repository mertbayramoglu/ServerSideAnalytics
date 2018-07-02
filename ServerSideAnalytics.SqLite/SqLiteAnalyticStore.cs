﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ServerSideAnalytics.SqLite
{
    public class SqLiteAnalyticStore : IAnalyticStore
    {
        private static readonly IMapper Mapper;
        private readonly string _connectionString;

        static SqLiteAnalyticStore()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WebRequest, SqliteWebRequest>();
                cfg.CreateMap<SqliteWebRequest, WebRequest>();
            });

            Mapper = config.CreateMapper();
        }

        public SqLiteAnalyticStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(WebRequest request)
        {
            using (var db = new SqLiteContext(_connectionString))
            {
                await db.Database.EnsureCreatedAsync();
                await db.WebRequest.AddAsync(Mapper.Map<SqliteWebRequest>(request));
                await db.SaveChangesAsync();
            }
        }

        public async Task AddAsync(SqliteWebRequest request)
        {
            using (var db = new SqLiteContext(_connectionString))
            {
                await db.Database.EnsureCreatedAsync();
                await db.WebRequest.AddAsync(request);
                await db.SaveChangesAsync();
            }
        }

        public Task<long> CountUniqueAsync(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return CountUniqueAsync(from, to);
        }

        public async Task<long> CountUniqueAsync(DateTime from, DateTime to)
        {
            using (var db = new SqLiteContext(_connectionString))
            {
                return await db.WebRequest.Where(x => x.Timestamp >= from && x.Timestamp <= to).GroupBy(x => x.Identity).CountAsync();
            }
        }

        public async Task<long> CountAsync(DateTime from, DateTime to)
        {
            using (var db = new SqLiteContext(_connectionString))
            {
                return await db.WebRequest.Where(x => x.Timestamp >= from && x.Timestamp <= to).CountAsync();
            }
        }

        public Task<IEnumerable<string>> IpAddresses(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return IpAddressesAsync(from, to);
        }

        public async Task<IEnumerable<string>> IpAddressesAsync(DateTime from, DateTime to)
        {
            using (var db = new SqLiteContext(_connectionString))
            {
                return await db.WebRequest.Where(x => x.Timestamp >= from && x.Timestamp <= to).Select(x => x.Identity)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<SqliteWebRequest>> RequestByIdentityAsync(string identity)
        {
            using (var db = new SqLiteContext(_connectionString))
            {
                return await db.WebRequest.Where(x => x.Identity == identity).ToListAsync();
            }
        }
    }
}