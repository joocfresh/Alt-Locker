using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AltLocker
{
    public partial class Form : System.Windows.Forms.Form
    {
        private static readonly byte[] BinaryData =
        {
            // 헤더(8바이트): 00 00 00 00 00 00 00 00
            0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,

            // 매핑 개수(DWORD, 종료 포함): 3
            0x03,0x00,0x00,0x00,

            // 매핑1: 0x38(Left Alt) -> 0x00 (Disable)
            // 형식: TO(00 00) FROM(38 00)  => 00 00 38 00
            0x00,0x00,0x38,0x00,

            // 매핑2: 0xE038(Right Alt/AltGr) -> 0x00 (Disable)
            // 형식: TO(00 00) FROM(38 E0)  => 00 00 38 E0
            0x00,0x00,0x38,0xE0,

            // 종료 엔트리
            0x00,0x00,0x00,0x00
        };

        public Form()
        {
            InitializeComponent();
            this.Load += Form_Load;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            if (IsAltBlocked())
            {
                // Block 상태 → Button1 강조
                button1.BackColor = SystemColors.Highlight;
                button1.ForeColor = Color.White;
                button2.BackColor = SystemColors.Control;
                button2.ForeColor = Color.Black;
            }
            else
            {
                // Unblock 상태 → Button2 강조
                button2.BackColor = SystemColors.Highlight;
                button2.ForeColor = Color.White;
                button1.BackColor = SystemColors.Control;
                button1.ForeColor = Color.Black;
            }
        }

        private bool IsAltBlocked()
        {
            using (var regKey =
                Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layout", writable: false))
            {
                if (regKey == null) return false;

                var value = regKey.GetValue("Scancode Map") as byte[];
                if (value == null) return false;

                // BinaryData와 동일하면 블록 상태
                return value.SequenceEqual(BinaryData);
            }
        }

        private void alt차단풀기_MouseClick(object sender, EventArgs e)
        {
            {
                using (var regKey =
                   Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layout", writable: true))
                {
                    regKey?.DeleteValue("Scancode Map", false);
                }

                MessageBox.Show("해제 완료. 재부팅(또는 로그오프) 후 반영됩니다.\nDisabled successfully. Changes will take effect after a reboot (or log off).");
                Form_Load(null, null); // 버튼 색 갱신
            }
        }

        private void alt차단하기_MouseClick(object sender, EventArgs e)
        {
            using (var regKey =
                   Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layout", writable: true)
                   ?? Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layout"))
            {
                regKey.SetValue("Scancode Map", BinaryData, RegistryValueKind.Binary);
            }

            MessageBox.Show("적용 완료. 재부팅(또는 로그오프) 후 반영됩니다.\nApplied successfully. Changes will take effect after a reboot (or log off).");
            Form_Load(null, null); // 버튼 색 갱신
        }
    }
}
