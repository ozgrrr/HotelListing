using HotelListing.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Api.Configurations.Entities
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
                new Country
                {
                    Id = 1,
                    Name = "Turkey",
                    ShortName = "TR",
                },
                new Country
                {
                    Id = 2,
                    Name = "Poland",
                    ShortName = "PL",
                },
                new Country
                {
                    Id = 3,
                    Name = "United Kingdom",
                    ShortName = "UK",
                });
        }
    }
}
