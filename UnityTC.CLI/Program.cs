//  
// © 2016 Max the Cat Studios Ltd.
// 
// https://maxthecatstudios.com
// @maxthecatstudio
//  
// Created By Desmond Fernando 2016 06 30 12:22 PM
//   
//   
//  

using System.Linq;

namespace UnityTC.CLI
{
    class Program
    {
        static void Main( string[] args )
        {
            Unity.StartUnity( args[ 0 ], string.Join( " ", args, 1, args.Length - 1 ) );
        }
    }
}