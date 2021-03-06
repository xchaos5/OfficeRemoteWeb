﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPt = Microsoft.Office.Interop.PowerPoint;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;

namespace TestConsole
{
    class Program
    {
        static PPt.Application pptApplication;
        static PPt.Presentation presentation;
        static PPt.Slides slides;
        static PPt.Slide slide;
        static int slidescount;
        static int slideIndex;

        static string GetContentType(string path)
        {
            string ext = "";
            int dotIndex = path.LastIndexOf('.');
            if (dotIndex >= 0)
                ext = path.Substring(dotIndex + 1);
            
            string contentType;
            switch (ext)
            {
                case "htm":
                    contentType = "text/html";
                    break;
                case "html":
                    contentType = "text/html";
                    break;
                case "js":
                    contentType = "text/javascript";
                    break;
                case "css":
                    contentType = "text/css";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                case "jpg":
                    contentType = "image/jpeg";
                    break;
                case "jpeg":
                    contentType = "image/jpeg";
                    break;
                default:
                    contentType = "text/plain";
                    break;
            }
            return contentType;
        }

        static void Main(string[] args)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                byte[] sendData = null;
                string dir = Environment.CurrentDirectory;  //Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                try
                {
                    HttpListener listener = new HttpListener();
                    listener.Prefixes.Add("http://+:80/");
                    listener.Start();
                    Console.WriteLine("Server started.");

                    while (true)
                    {
                        var context = listener.GetContext();
                        Console.WriteLine(string.Format("{0} {1}", context.Request.HttpMethod, context.Request.Url.AbsolutePath));
                        string relPath = context.Request.Url.LocalPath;
                        if (File.Exists(dir + relPath))
                            sendData = File.ReadAllBytes(dir + relPath);
                        else
                            sendData = System.Text.Encoding.Default.GetBytes("404 File not found");
                        context.Response.ContentType = GetContentType(context.Request.Url.LocalPath);
                        context.Response.Close(sendData, true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }));
            thread.IsBackground = true;
            thread.Start();

            Console.ReadLine();
        }
    }
}
