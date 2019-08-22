﻿using System;
using Microwave;

namespace ServiceConfig
{
    public class ServiceConfiguration
    {
        public static ServiceBaseAddressCollection ServiceAdresses => new ServiceBaseAddressCollection
        {
            MatchHost,
            TeamHost,
            PlayerHost,
            SeasonHost,
            RaceHost
        };

        public static Uri MatchHost { get; } = new Uri("https://matcheshost.azurewebsites.net");
        public static Uri TeamHost { get; } = new Uri("https://teamshost.azurewebsites.net");
        public static Uri PlayerHost { get; }  = new Uri("https://players-host.herokuapp.com/");
        public static Uri SeasonHost { get; } = new Uri("https://seasons-host.herokuapp.com/");
        public static Uri RaceHost { get; } = new Uri("https://races-host.herokuapp.com/");

        public static Uri SeasonReadHost { get; } = new Uri("https://seasons-readhost.herokuapp.com/");
        public static Uri TeamReadHost { get; } = new Uri("https://teams-readhost.herokuapp.com/");


//        public static ServiceBaseAddressCollection ServiceAdresses => new ServiceBaseAddressCollection
//        {
//            new Uri("http://localhost:5001"),
//            new Uri("http://localhost:5002"),
//            new Uri("http://localhost:5003"),
//            new Uri("http://localhost:5004"),
//            new Uri("http://localhost:5007"),
//        };
//        public static Uri MatchHost { get; } = new Uri("http://localhost:5003");
//        public static Uri TeamHost { get; } = new Uri("http://localhost:5001");
//        public static Uri PlayerHost { get; }  = new Uri("http://localhost:5002");
//        public static Uri SeasonHost { get; } = new Uri("http://localhost:5004");
//        public static Uri RaceHost { get; } = new Uri("http://localhost:5007");
//
//        public static Uri SeasonReadHost { get; } = new Uri("http://localhost:5006");
//        public static Uri TeamReadHost { get; } = new Uri("http://localhost:5000");
    }
}