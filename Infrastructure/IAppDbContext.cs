﻿using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public interface IAppDbContext
{
    DbSet<Comment> Comments { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
