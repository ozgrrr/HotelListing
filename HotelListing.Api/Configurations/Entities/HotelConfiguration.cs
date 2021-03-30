using HotelListing.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Api.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Turkish Hotel",
                    Address = "Antalya",
                    CountryId = 1,
                    Rating = 4.8
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Polish Hotel",
                    Address = "Gdansk",
                    CountryId = 2,
                    Rating = 5.0
                },
                new Hotel
                {
                    Id = 3,
                    Name = "British Hotel",
                    Address = "London",
                    CountryId = 3,
                    Rating = 4.1
                });
        }
    }
}
