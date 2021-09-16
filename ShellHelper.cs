using System;
using System.Diagnostics;

public static class ShellHelper
{
    public static void Bash(this string cmd)
    {
        var escapedArgs = cmd.Replace(@"\", @"/");
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"/bin/bash",
                Arguments = @$"{escapedArgs}",
            }
        };
        process.Start();
    }
}