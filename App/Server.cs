using System;
using Microsoft.Extensions.Configuration;

public static class Server
{
    //server properties
    public static DateTime ServerStart = DateTime.Now;
    public static double RequestCount = 0;
    public static double PageRequestCount = 0;
    public static double ApiRequestCount = 0;
    public static float RequestTime = 0;
    public static string Version = "1.0";

    //config properties
    public static IConfiguration Config;
    public static string SqlActive = "";
    public static string SqlConnectionString = "";
    public static int BcryptWorkFactor = 10;
    public static string Salt = "";
    public static bool HasAdmin = false; //no admin account exists
    public static bool ResetPass = false; //force admin to reset password
}
