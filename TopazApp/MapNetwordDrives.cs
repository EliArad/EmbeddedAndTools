﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetworkDrivesApi
{
    public static class NetworkDrives
    {
        public static bool MapDrive(string DriveLetter, string Path, string Username, string Password)
        {

            bool ReturnValue = false;

            try
            {
                DisconnectDrive(DriveLetter);
            }
            catch (Exception err)
            {

            }
            if (System.IO.Directory.Exists(DriveLetter + ":\\"))
            {
                return true;
            }
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.FileName = "net.exe";
            p.StartInfo.Arguments = " use " + DriveLetter + ": " + '"' + Path + '"' + " " + Password + " /user:" + Username + " /y";
            p.Start();
            p.WaitForExit();

            string ErrorMessage = p.StandardError.ReadToEnd();
            string OuputMessage = p.StandardOutput.ReadToEnd();
            if (ErrorMessage.Length > 0)
            {
                throw new Exception("Error:" + ErrorMessage);
            }
            else
            {
                ReturnValue = true;
            }
            return ReturnValue;
        }
        public static bool DisconnectDrive(string DriveLetter)
        {
            bool ReturnValue = false;
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.FileName = "net.exe";
            p.StartInfo.Arguments = " use " + DriveLetter + ": /DELETE";
            p.Start();
            p.WaitForExit();

            string ErrorMessage = p.StandardError.ReadToEnd();
            string OuputMessage = p.StandardOutput.ReadToEnd();
            if (ErrorMessage.Length > 0)
            {
                throw new Exception("Error:" + ErrorMessage);
            }
            else
            {
                ReturnValue = true;
            }
            return ReturnValue;
        }

    }

}