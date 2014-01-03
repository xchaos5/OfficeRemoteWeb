using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using Microsoft.Office.Core;
using PPt = Microsoft.Office.Interop.PowerPoint;
using System.Runtime.InteropServices;

namespace OfficeRemoteWeb.Handlers
{
    /// <summary>
    /// Summary description for OfficeRemoteProxy
    /// </summary>
    public class OfficeRemoteProxy : IHttpHandler
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int extraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short MapVirtualKey(int wCode, int wMapType);

        // constants for the mouse_input() API function
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        public const int KEYBDEVENTF_KEYDOWN = 0;
        public const int KEYBDEVENTF_KEYUP = 2;


        public void ProcessRequest(HttpContext context)
        {    
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            string gesType = context.Request["type"].ToString();
            string gesDirection = context.Request["direction"] == null ? "" : context.Request["direction"].ToString();
            double offsetX = 0;
            Double.TryParse(context.Request["offset_x"].ToString(), out offsetX);
            double offsetY = 0;
            Double.TryParse(context.Request["offset_y"].ToString(), out offsetY);

            PPt.Application pptApplication = null;
            PPt.Presentation presentation = null;
            PPt.Slides slides = null;
            PPt.Slide slide = null;
            int slidescount = 0;
            int slideIndex = 0;

            if (gesType == "swipe")
            {
                try
                {
                    // Get Running PowerPoint Application object 
                    pptApplication = System.Runtime.InteropServices.Marshal.GetActiveObject("PowerPoint.Application") as PPt.Application;
                }
                catch
                {

                }
                if (pptApplication != null)
                {
                    // Get Presentation Object 
                    presentation = pptApplication.ActivePresentation;
                    // Get Slide collection object 
                    slides = presentation.Slides;
                    // Get Slide count 
                    slidescount = slides.Count;
                    // Get current selected slide  
                    try
                    {
                        // Get selected slide object in normal view 
                        slide = slides[pptApplication.ActiveWindow.Selection.SlideRange.SlideNumber];
                    }
                    catch
                    {
                        // Get selected slide object in reading view 
                        slide = pptApplication.SlideShowWindows[1].View.Slide;
                    }
                }

                if (gesDirection == "left")
                {
                    slideIndex = slide.SlideIndex - 1;
                    if (slideIndex >= 1)
                    {
                        try
                        {
                            slide = slides[slideIndex];
                            slides[slideIndex].Select();
                        }
                        catch
                        {
                            pptApplication.SlideShowWindows[1].View.Previous();
                            slide = pptApplication.SlideShowWindows[1].View.Slide;
                        }
                    }
                }
                else if (gesDirection == "right")
                {
                    slideIndex = slide.SlideIndex + 1;
                    if (slideIndex > slidescount)
                    {

                    }
                    else
                    {
                        try
                        {
                            slide = slides[slideIndex];
                            slides[slideIndex].Select();
                        }
                        catch
                        {
                            pptApplication.SlideShowWindows[1].View.Next();
                            slide = pptApplication.SlideShowWindows[1].View.Slide;
                        }
                    } 
                }
                else if (gesDirection == "up")
                {
                    try
                    {
                        // Call Select method to select first slide in normal view 
                        slides[1].Select();
                        slide = slides[1];
                    }
                    catch
                    {
                        // Transform to first page in reading view 
                        pptApplication.SlideShowWindows[1].View.First();
                        slide = pptApplication.SlideShowWindows[1].View.Slide;
                    }
                }
                else if (gesDirection == "down")
                {
                    try
                    {
                        slides[slidescount].Select();
                        slide = slides[slidescount];
                    }
                    catch
                    {
                        pptApplication.SlideShowWindows[1].View.Last();
                        slide = pptApplication.SlideShowWindows[1].View.Slide;
                    }
                }
            }
            else if (gesType == "tap")
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            }
            else if (gesType == "drag")
            {
                mouse_event(MOUSEEVENTF_MOVE, (int)offsetX, (int)offsetY, 0, 0);
            }
            else if (gesType == "run")
            {
                try
                {
                    pptApplication = System.Runtime.InteropServices.Marshal.GetActiveObject("PowerPoint.Application") as PPt.Application;
                }
                catch
                {

                }
                if (pptApplication != null)
                {
                    presentation = pptApplication.ActivePresentation;
                    if (presentation != null)
                        presentation.SlideShowSettings.Run();
                }
            }
            else if (gesType == "stop")
            {
                try
                {
                    pptApplication = System.Runtime.InteropServices.Marshal.GetActiveObject("PowerPoint.Application") as PPt.Application;
                }
                catch
                {

                }
                if (pptApplication != null)
                {
                    presentation = pptApplication.ActivePresentation;
                    if (presentation != null)
                        presentation.SlideShowWindow.View.Exit();
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}