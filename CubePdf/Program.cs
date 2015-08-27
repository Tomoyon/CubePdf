﻿/* ------------------------------------------------------------------------- */
///
/// Program.cs
///
/// Copyright (c) 2009 CubeSoft, Inc. All rights reserved.
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as published
/// by the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.
///
/* ------------------------------------------------------------------------- */
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CubePdf
{
    static class Program
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Main
        /// 
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        [STAThread]
        static void Main(string[] args)
        {
            var exec = System.Reflection.Assembly.GetEntryAssembly();
            var dir = System.IO.Path.GetDirectoryName(exec.Location);
            var setting = new UserSetting(false);

            SetupLog(dir + @"\cubepdf.log");
            var edition = (IntPtr.Size == 4) ? "x86" : "x64";
            CubePdf.Message.Trace(string.Format("CubePDF {0} ({1})", setting.Version, edition));
            CubePdf.Message.Trace(Environment.OSVersion.ToString());
            CubePdf.Message.Trace(string.Format(".NET Framework {0}", Environment.Version.ToString()));
            var message = "Arguments";
            foreach (var s in args) message += string.Format("{0}\t{1}", Environment.NewLine, s);
            CubePdf.Message.Trace(message);

            SetupUserSetting(setting, args);
            CheckUpdate(setting);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(setting));

            Trace.Close();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetupUserSetting
        ///
        /// <summary>
        /// プログラムに指定された引数等を用いて、UserSetting オブジェクトを
        /// 初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static void SetupUserSetting(UserSetting setting, string[] args)
        {
            var cmdline = new CubePdf.Settings.CommandLine(args);

            var docname = cmdline.Options.ContainsKey("DocumentName") ? cmdline.Options["DocumentName"] : "";
            bool is_config = false;
            try
            {
                if (!string.IsNullOrEmpty(docname) &&
                    System.IO.Path.GetExtension(docname) == setting.Extension &&
                    System.IO.File.Exists(docname))
                {
                    is_config = true;
                }
            }
            catch (Exception err)
            {
                // docname に Windows のファイル名に使用できない記号が含まれる
                // 場合に例外が送出されるので、その対策。
                CubePdf.Message.Trace(err.ToString());
                is_config = false;
            }

            if (is_config) setting.Load(docname);
            else
            {
                setting.Load();
                var filename = DocumentName.CreateFileName(docname);
                if (filename != null)
                {
                    string ext = Parameter.GetExtension((Parameter.FileTypes)setting.FileType);
                    filename = System.IO.Path.ChangeExtension(filename, ext);
                    string dir = (setting.OutputPath.Length == 0 || System.IO.Directory.Exists(setting.OutputPath)) ?
                        setting.OutputPath : System.IO.Path.GetDirectoryName(setting.OutputPath);
                    setting.OutputPath = dir + '\\' + filename;
                }
            }

            setting.InputPath = cmdline.Options.ContainsKey("InputFile") ? cmdline.Options["InputFile"] : "";
            setting.DeleteOnClose = cmdline.Options.ContainsKey("DeleteOnClose");

            if (System.IO.Directory.Exists(setting.LibPath))
                System.Environment.SetEnvironmentVariable("TEMP", setting.LibPath, EnvironmentVariableTarget.Process);
            else CubePdf.Message.Trace("LibPath Not Found");
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetupLog
        ///
        /// <summary>
        /// トレースログを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static void SetupLog(string src)
        {
            try
            {
                if (System.IO.File.Exists(src))
                {
                    using (System.IO.StreamWriter stream = new System.IO.StreamWriter(src, false))
                    {
                        // 空の内容で上書きする
                    }
                }

                Trace.Listeners.Remove("Default");
                Trace.Listeners.Add(new TextWriterTraceListener(src));
                Trace.AutoFlush = true;
            }
            catch (Exception err) { CubePdf.Message.Trace(err.ToString()); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CheckUpdate
        ///
        /// <summary>
        /// アップデートの確認が必要であるかどうかを判断し、必要であれば
        /// 確認用のプログラムを実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static void CheckUpdate(UserSetting setting)
        {
            try
            {
                if (!setting.CheckUpdate ||
                    string.IsNullOrEmpty(setting.InstallPath) ||
                    DateTime.Now <= setting.LastCheckUpdate.AddDays(1)) return;
                var path = System.IO.Path.Combine(setting.InstallPath, "cubepdf-checker.exe");
                Process.Start(path);
            }
            catch (Exception err) { CubePdf.Message.Trace(err.ToString()); }
        }
    }
}
