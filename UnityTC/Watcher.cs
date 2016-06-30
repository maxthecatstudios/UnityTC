//  
// © 2016 Max the Cat Studios Ltd.
// 
// https://maxthecatstudios.com
// @maxthecatstudio
//  
// Created By Desmond Fernando 2016 06 30 11:53 AM
//   
//   
//  

using System;
using System.IO;
using System.Threading;

namespace UnityTC
{
    /// <summary>
    /// Watches the Unity log file and redirects it to standard output
    /// </summary>
    public class Watcher
    {
        private const string progressBarMarker = "DisplayProgressbar: ";
        private static long previousLogSize;

        /// <summary>
        /// Indicates if this thread should stop
        /// </summary>
        private volatile bool shouldStop;

        /// <summary>
        /// Creates a new Watcher that will read the log from the specified path
        /// </summary>
        /// <param name="logPath"></param>
        public Watcher( string logPath )
        {
            LogPath = logPath;
        }

        /// <summary>
        /// Path to the log file
        /// </summary>
        public string LogPath { get; }

        /// <summary>
        /// Gets the full log text
        /// </summary>
        public string FullLog { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public void Run()
        {
            Console.WriteLine( $"##teamcity[progressStart 'Unity Log {LogPath}']" );

            while ( true )
            {
                if ( File.Exists( LogPath ) )
                {
                    using ( var stream = new FileStream( LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
                    {
                        stream.Position = previousLogSize;
                        previousLogSize = stream.Length;

                        using ( var reader = new StreamReader( stream ) )
                        {
                            var newText = reader.ReadToEnd();
                            LogProgressMessages( newText );
                            FullLog += newText;
                            Console.Write( newText );
                        }
                    }
                }

                if ( shouldStop ) break;

                Thread.Sleep( 1000 );
            }

            Console.WriteLine( $"##teamcity[progressFinish 'Unity Log {LogPath}']" );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            shouldStop = true;
        }

        /// <summary>
        /// Searches for progress bar messages and forwards them to TeamCity
        /// </summary>
        /// <param name="text"></param>
        private static void LogProgressMessages( string text )
        {
            var lines = text.Split( Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
            for ( var i = 0; i < lines.Length; i++ )
            {
                if ( lines[ i ].StartsWith( progressBarMarker ) )
                {
                    var progressName = lines[ i ].Substring( progressBarMarker.Length );
                    Console.WriteLine();
                    Console.WriteLine( "##teamcity[progressMessage '" + progressName + "']" );
                }
            }
        }
    }
}