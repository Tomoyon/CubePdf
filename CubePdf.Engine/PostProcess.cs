﻿/* ------------------------------------------------------------------------- */
///
/// PostProcess.cs
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CubePdf
{
    /* --------------------------------------------------------------------- */
    ///
    /// PostProcess
    ///
    /// <summary>
    /// CubePDF による変換処理が終了した後に、指定されたポストプロセスを
    /// 実行するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    class PostProcess
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// PostProcess (constructor)
        ///
        /// <summary>
        /// 既定の値でオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public PostProcess()
        {
            _messages = new List<CubePdf.Message>();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// PostProcess (constructor)
        ///
        /// <summary>
        /// 引数に指定されたメッセージ格納コンテナを用いて、オブジェクトを
        /// 初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public PostProcess(List<CubePdf.Message> messages)
        {
            _messages = messages;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Verb
        /// 
        /// <summary>
        /// ポストプロセスの種類を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Parameter.PostProcesses Verb
        {
            get { return _verb; }
            set { _verb = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UserName
        /// 
        /// <summary>
        /// ユーザー名を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string UserName
        {
            set { _username = value; }
            get { return _username; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UserProgram
        /// 
        /// <summary>
        /// ユーザプログラム名を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string UserProgram
        {
            get { return _program; }
            set { _program = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UserArguments
        /// 
        /// <summary>
        /// ユーザ引数を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string UserArguments
        {
            get { return _args; }
            set { _args = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FileName
        /// 
        /// <summary>
        /// 変換に成功したファイル名（パス）を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// EmergencyMode
        /// 
        /// <summary>
        /// プロセスが EmergencyMode で実行されているかどうかを表す値を
        /// 取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool EmergencyMode
        {
            get { return _em; }
            set { _em = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Messages
        ///
        /// <summary>
        /// メッセージ一覧を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public List<CubePdf.Message> Messages
        {
            get { return _messages; }
        }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Run
        ///
        /// <summary>
        /// 引数にしていされているユーザ設定に従って、プロセスを実行します。
        /// </summary>
        /// 
        /// <remarks>
        /// 例外発生時はエラー内容を Messages に記録するのみで、外部に
        /// 例外が伝播しないようにします。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void Run()
        {
            if (Verb == Parameter.PostProcesses.None) return;

            try
            {
                var path = GetNormalizedPath();
                if (!System.IO.File.Exists(path)) return;

                switch (Verb)
                {
                    case Parameter.PostProcesses.Open:
                        RunOpen(path);
                        break;
                    case Parameter.PostProcesses.OpenFolder:
                        RunOpenFolder(path);
                        break;
                    case Parameter.PostProcesses.UserProgram:
                        RunUserProgram(path);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception err) { AddMessage(err); }
        }

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// CreateProcessStartInfo
        ///
        /// <summary>
        /// ProcessStartInfo を初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private System.Diagnostics.ProcessStartInfo CreateProcessStartInfo()
        {
            var dest = new System.Diagnostics.ProcessStartInfo();
            dest.CreateNoWindow = false;
            dest.UseShellExecute = true;
            dest.LoadUserProfile = false;
            dest.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            return dest;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RunOpen
        ///
        /// <summary>
        /// ファイルを開くポストプロセスを実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void RunOpen(string path)
        {
            var info = CreateProcessStartInfo();
            info.FileName = path;

            var process = new System.Diagnostics.Process();
            process.StartInfo = info;
            process.Start();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RunOpenFolder
        ///
        /// <summary>
        /// フォルダを開くポストプロセスを実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void RunOpenFolder(string path)
        {
            var info = CreateProcessStartInfo();
            info.FileName = "explorer.exe";
            info.Arguments = "\"" + System.IO.Path.GetDirectoryName(path) + "\"";

            var process = new System.Diagnostics.Process();
            process.StartInfo = info;
            process.Start();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RunUserProgram
        ///
        /// <summary>
        /// ユーザプログラムを実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void RunUserProgram(string path)
        {
            var info = CreateProcessStartInfo();
            info.FileName = UserProgram;
            if (!string.IsNullOrEmpty(UserArguments))
            {
                var replaced = "\"" + path + "\"";
                info.Arguments = UserArguments.Replace("%%FILE%%", replaced);
            }

            var process = new System.Diagnostics.Process();
            process.StartInfo = info;
            process.Start();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetNormalizedPath
        ///
        /// <summary>
        /// 変換に成功したファイル名（パス）を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private string GetNormalizedPath()
        {
            if (System.IO.File.Exists(FileName)) return FileName;
            var directory = System.IO.Path.GetDirectoryName(FileName);
            var basename  = System.IO.Path.GetFileNameWithoutExtension(FileName) + "-001";
            var extension = System.IO.Path.GetExtension(FileName);
            return System.IO.Path.Combine(directory, basename + extension);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// AddMessage
        ///
        /// <summary>
        /// デバッグ用メッセージを追加します。
        /// </summary>
        /// 
        /// <remarks>
        /// 現状では、関連付けされておらずファイルを開く事に失敗した場合、
        /// エラーメッセージは表示させないようにしています。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        private void AddMessage(Exception err)
        {
            var isopen  = (Verb == Parameter.PostProcesses.Open);
            var descr   = isopen ? Properties.Resources.FileNotRelated : err.Message;
            var message = string.Format("{0}{1}({2})", descr, Environment.NewLine, Properties.Resources.AnnotWithPostProcess);
            var level   = isopen ? Message.Levels.Debug : Message.Levels.Error;
            _messages.Add(new Message(level, message));
            _messages.Add(new Message(Message.Levels.Debug, err));
        }

        #endregion

        #region Variables
        List<CubePdf.Message> _messages = null;
        Parameter.PostProcesses _verb = Parameter.PostProcesses.None;
        private string _filename = string.Empty;
        private string _username = string.Empty;
        private string _program = string.Empty;
        private string _args = string.Empty;
        private bool _em = false;
        #endregion
    }
}
