using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SearchFunction.Models;

    public class SearchFunctionContext : DbContext
    {
        public SearchFunctionContext (DbContextOptions<SearchFunctionContext> options)
            : base(options)
        {
        }

        public DbSet<SearchFunction.Models.Classroom> Classroom { get; set; } = default!;

public DbSet<SearchFunction.Models.Student> Student { get; set; } = default!;
    }
