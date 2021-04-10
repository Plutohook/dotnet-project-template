﻿namespace PlutoNetCoreTemplate.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Aggregates.Permission;

    using Microsoft.EntityFrameworkCore;

    using PlutoData;

    public class PermissionGrantRepository : EfRepository<EfCoreDbContext, PermissionGrant>, IPermissionGrantRepository
    {
        /// <inheritdoc />
        public PermissionGrantRepository(EfCoreDbContext dbContext) : base(dbContext)
        {
        }

        /// <inheritdoc />
        public async Task<PermissionGrant> FindAsync(string name, string providerName, string providerKey, CancellationToken cancellationToken = default)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Name == name && x.ProviderName == providerName && x.ProviderKey == providerKey, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<PermissionGrant>> GetListAsync(string providerName, string providerKey, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(x => x.ProviderName == providerName && x.ProviderKey == providerKey).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<PermissionGrant>> GetListAsync(string[] names, string providerName, string providerKey,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(x => names.Contains(x.Name) && x.ProviderName == providerName && x.ProviderKey == providerKey).ToListAsync(cancellationToken);
        }
    }
}