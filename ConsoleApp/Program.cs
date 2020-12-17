﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using ConsoleApp.Maps;

namespace ConsoleApp
{
    class Program
    {
        private static UserConsole _userConsole;
        private static BootStrapper _bootstrapper;

        static void Main(string[] args)
        {
            _bootstrapper = BootStrapper.BootstrapSystem(new CoreModule());
            var simResults = _bootstrapper.Resolve<ISimulationResults>();
            _userConsole = new UserConsole();
            Initialization();
            simResults.WriteResults();
            GenerateImages();
            Console.WriteLine("Program Completed");
        }

       

        private static void Initialization()
        {
            _userConsole.PrintStartMenu();
            var input = UserConsole.GetUserInput();
            if (input == "1")
            {
                var (x,y) = _userConsole.GetMapDimensions();
                var minePercentage = UserConsole.GetMinePercentage();
                RunSimulation(x, y, minePercentage);
            }
            else
            {
                UserConsole.PrintInvalidInput();
                Initialization();
            }
        }

        private static void RunSimulation(int x, int y, double minePercentage)
        {
            var mapFactory = _bootstrapper.Resolve<IMapFactory>();
            var simRunner = _bootstrapper.Resolve<ISimRunner>();
            
            mapFactory.GenerateMaps(x, y, minePercentage);
            simRunner.Run();
        }
        
        private static void GenerateImages()
        {
            var file = Path.Combine("./",Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"HexPlot.py");
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "python3",
                Arguments = file,
                UseShellExecute = true
            };
            Process proc = new Process()
            {
                StartInfo = startInfo,
            };
            proc.Start();
            while (!proc.HasExited)
            {
                Thread.Sleep(500);
            }
        }

    }
}