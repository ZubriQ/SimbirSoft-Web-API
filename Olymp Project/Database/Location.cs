﻿namespace Olymp_Project.Database
{
    public class Location
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(long id, double latitude, double longitude)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}