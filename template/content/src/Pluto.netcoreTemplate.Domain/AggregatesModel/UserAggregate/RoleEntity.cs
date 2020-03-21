﻿using System.Collections.Generic;

namespace Pluto.netcoreTemplate.Domain.AggregatesModel.UserAggregate
{
    public class RoleEntity : BaseEntity<int>
    {

        public RoleEntity()
        { }

        public RoleEntity(string name)
        {
            this.RoleName = RoleName;
        }

        public string RoleName { get; set; }

        public IReadOnlyCollection<UserRoleEntity> Users { get; set; }
    }
}