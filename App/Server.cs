using System;
using Microsoft.Extensions.Configuration;

public static class Server
{
    //server properties
    public static string Name { get; set; } = "Kandu";
    public static DateTime ServerStart { get; set; } = DateTime.Now;
    public static double RequestCount { get; set; } = 0;
    public static double PageRequestCount { get; set; } = 0;
    public static double ApiRequestCount { get; set; } = 0;
    public static float RequestTime { get; set; } = 0;
    public static string Version { get; set; } = "1.0";

    //config properties
    public static IConfiguration Config { get; set; }
    public static string SqlActive { get; set; } = "";
    public static string SqlConnectionString { get; set; } = "";
    public static int BcryptWorkFactor { get; set; } = 10;
    public static string Salt { get; set; } = "";
    public static bool HasAdmin { get; set; } = false; //no admin account exists
    public static bool ResetPass { get; set; } = false; //force admin to reset password
}
