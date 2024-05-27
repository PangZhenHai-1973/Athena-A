using System;
using System.Windows.Forms;
using System.Text;

namespace Athena_A
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ApplicationConfiguration.Initialize();
            Application.SetDefaultFont(new System.Drawing.Font("SimSun", 9F));
            Application.Run(new mainform());
        }
    }
}
