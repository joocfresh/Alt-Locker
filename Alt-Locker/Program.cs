using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AltLocker
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!IsRunAsAdmin())
            {
                // 관리자 권한 아님
                DialogResult result = MessageBox.Show(
                    "이 프로그램은 관리자 권한이 필요합니다.\n관리자 권한으로 다시 실행하시겠습니까?\nThis program requires administrator privileges.\nDo you want to restart it with administrator privileges?",
                    "권한 필요\nPermission Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var exeName = Process.GetCurrentProcess().MainModule.FileName;
                        var startInfo = new ProcessStartInfo(exeName)
                        {
                            UseShellExecute = true,
                            Verb = "runas" // 관리자 권한 요청
                        };
                        Process.Start(startInfo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("관리자 권한 실행 실패(Administrator run failed): " + ex.Message);
                    }
                }
                // 재실행 안 하거나 실패하면 종료
                return;
            }

            // 관리자 권한일 때만 진입
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form());
        }

        private static bool IsRunAsAdmin()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
