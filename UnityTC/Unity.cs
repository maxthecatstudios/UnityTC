//  
// © 2016 Max the Cat Studios Ltd.
// 
// https://maxthecatstudios.com
// @maxthecatstudio
//  
// Created By Desmond Fernando 2016 06 30 12:08 PM
//   
//   
//  

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace UnityTC
{
    public class Unity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unityPath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int StartUnity( string unityPath, string args )
        {
            var sb = new StringBuilder();
            var watcher = new Watcher( Path.GetTempFileName() );
            var watcherThread = new Thread( watcher.Run );
            watcherThread.Start();

            var unity = new Process { StartInfo = new ProcessStartInfo( unityPath ) };

            sb.Append( $"-logFile \"{watcher.LogPath}\"" );
            sb.Append( $" {args}" );
            //for ( var i = 1; i < args.Length; i++ )
            //    sb.Append( $" \"{args[ i ]}\"" );

            unity.StartInfo.Arguments = sb.ToString();
            Console.WriteLine( $"##teamcity[progressMessage 'Starting Unity {unity.StartInfo.Arguments}']" );

            unity.Start();

            Console.WriteLine( $"##teamcity[setParameter name='unityPID' value='{unity.Id}']" );

            unity.WaitForExit();
            watcher.Stop();
            watcherThread.Join();

            if ( watcher.FullLog.Contains( "Successful build ~0xDEADBEEF" ) )
            {
                Console.WriteLine( "##teamcity[progressMessage 'Success']" );
                Environment.Exit( 0 );
            }
            else
            {
                Console.WriteLine( "##teamcity[progressMessage 'Failed']" );
                Environment.Exit( 1 );
            }

            return unity.ExitCode;
        }
    }
}