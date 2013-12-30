using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPt = Microsoft.Office.Interop.PowerPoint;
using System.Runtime.InteropServices;

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

        static void Main(string[] args)
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
        }
    }
}
