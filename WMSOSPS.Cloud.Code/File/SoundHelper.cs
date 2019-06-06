using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.File
{
    public  class SoundHelper
    {
        public static void AnyToWav(string filepath, string wavpath)
        {
            string fpath = AppDomain.CurrentDomain.BaseDirectory + "Sound\\ffmpeg.exe";
            string commandText = fpath + " -i " + filepath + " -ac 1 -acodec pcm_alaw -ar 8000 -f wav " + wavpath;
            Process prc = new Process();
            prc.StartInfo.FileName = "cmd.exe";
            prc.StartInfo.UseShellExecute = false;
            prc.StartInfo.RedirectStandardInput = true;
            prc.StartInfo.RedirectStandardOutput = true;
            prc.StartInfo.RedirectStandardError = true;
            prc.StartInfo.CreateNoWindow = true;
            prc.Start();
            prc.StandardInput.WriteLine(commandText);
            prc.StandardInput.Close();
            prc.StandardOutput.ReadToEnd();
        }
    }
}
