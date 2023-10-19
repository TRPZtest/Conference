﻿using Conferency.Data.Db;
using System.Linq.Dynamic.Core;
using Conferency.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Conference.Data
{
    public class ConferanceRepository : IConferanceRepository
    {
        private readonly AppDbContext _context;

        public ConferanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public async Task<List<Region>> GetRegionsAsync()
        {
            var regions = await _context.Regions
                .AsNoTracking()
                .ToListAsync();

            return regions;
        }

        public async Task<List<User>> GetUsersAsync(int page = 0, int pageSize = 0)
        {
            var query = _context.Users.
                Include(x => x.Region).OrderBy(x => x.Id)
                .Skip(page * pageSize);

            if (pageSize != 0)
                query = query.Take(pageSize);

            var users = await query
                .AsNoTracking()
                .ToListAsync();

            return users;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(string email, string password)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email && x.Password == password);

            return user;
        }

        public async Task<List<SortingProperty>> GetSortingPropertiesAsync()
        {
            var properties = await _context.SortingProperties
                .AsNoTracking()
                .ToListAsync();

            return properties;
        }

        public async Task<SortingProperty> GetSortingPropertyAsync(long id)
        {
            var property = await _context.SortingProperties
                .Include(x => x.SortingColumn)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return property;
        }

        
        public async Task<List<UsersView>> GetUsersViewAsync(string orderByProperty = "", bool isDiscending = false)
        {
            if (string.IsNullOrEmpty(orderByProperty))
                return await _context.UsersViews.ToListAsync();
            if (isDiscending)
                return await _context.UsersViews
                    .OrderByDescendingDynamic(x => $"x.{orderByProperty}")
                    .ToListAsync();
            else 
                return await _context.UsersViews
                .OrderByDynamic(x => $"x.{orderByProperty}")
                .ToListAsync();

        }

        public Task<List<UsersView>> GetSortedUsersViewAsync(string orderByProperty = "", bool isDiscending = false)
        {
            throw new NotImplementedException();
        }
    }
}
