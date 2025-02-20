﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides the core Target functions.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class TargetCore
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum eTarget
        {
            File,
            NetPrinter,     // Port 9100 network printer
            WinPrinter,     // Windows printer instance
            Max
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static eTarget _targetType;
        private static ReportCore.eRptFileFmt _rptFileFmt;
        private static ReportCore.eRptChkMarks _rptChkMarks;

        private static Int32 _netPrinterPort;

        private static Int32 _netPrinterTimeoutSend;
        private static Int32 _netPrinterTimeoutReceive;

        private static String _netPrinterAddress;
        private static String _winPrinterName;

        private static String _crntFilename;
        private static String _saveFilename;

        private static Stream _opStream = null;
        private static BinaryWriter _binWriter = null;

        private static Boolean _flagOptRptWrap;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current target type.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static eTarget getType()
        {
            return _targetType;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e S e t t i n g s                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load current target metrics data from regisry.                     //
        // Note that 'capture file' data is not relevant for those tools      //
        // which don't output a printer ready job.                            // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void initialiseSettings()
        {
            _targetType = eTarget.File;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d F i l e C a p t                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load current target File capture metrics data.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void metricsLoadFileCapt(
            ToolCommonData.eToolIds crntToolId,
            ToolCommonData.eToolSubIds crntToolSubId,
            ToolCommonData.ePrintLang crntPDL)
        {
            _saveFilename = "";
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d F i l e R p t                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load current target report file metrics data.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void metricsLoadFileRpt(
            ToolCommonData.eToolIds crntToolId)
        {
            Int32 tmpFmt = 0,
                  tmpChkMarks = 0;

            Byte indxFmtNA = (Byte)ReportCore.eRptFileFmt.NA;
            Byte indxOptChkNA = (Byte)ReportCore.eRptChkMarks.NA;

            Boolean flagNA = false;

            //----------------------------------------------------------------//

            if (crntToolId == ToolCommonData.eToolIds.PrnAnalyse)
                ToolPrnAnalysePersist.loadDataRpt(ref tmpFmt);
            else
                flagNA = true;

            //----------------------------------------------------------------//

            if (flagNA)
                tmpFmt = indxFmtNA;
            else
            {
                if (tmpFmt >= indxFmtNA)
                    tmpFmt = 0;

                if (tmpChkMarks >= indxOptChkNA)
                    tmpChkMarks = 0;
            }

            _rptFileFmt = (ReportCore.eRptFileFmt)tmpFmt;
            _rptChkMarks = (ReportCore.eRptChkMarks)tmpChkMarks;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s R e t u r n F i l e C a p t                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load and return current target File capture metrics data.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void metricsReturnFileCapt(
            ToolCommonData.eToolIds crntToolId,
            ToolCommonData.eToolSubIds crntToolSubId,
            ToolCommonData.ePrintLang crntPDL,
            ref String saveFilename)
        {
            metricsLoadFileCapt(crntToolId, crntToolSubId, crntPDL);

            saveFilename = _saveFilename;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s R e t u r n F i l e R p t                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load and return current report file metrics data.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void metricsReturnFileRpt(
            ToolCommonData.eToolIds crntToolId,
            ref ReportCore.eRptFileFmt rptFileFmt,
            ref ReportCore.eRptChkMarks rptChkMarks,
            ref Boolean flagOptWrap)
        {
            metricsLoadFileRpt(crntToolId);

            rptFileFmt = _rptFileFmt;
            rptChkMarks = _rptChkMarks;
            flagOptWrap = _flagOptRptWrap;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e F i l e C a p t                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current target File capture metrics data.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void metricsSaveFileCapt(
            ToolCommonData.eToolIds crntToolId,
            ToolCommonData.eToolSubIds crntToolSubId,
            ToolCommonData.ePrintLang crntPDL,
            String saveFilename)
        {
            _targetType = eTarget.File;

            _saveFilename = saveFilename;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e F i l e R p t                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current target report file metrics data.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void metricsSaveFileRpt(
            ToolCommonData.eToolIds crntToolId,
            ReportCore.eRptFileFmt rptFileFmt,
            ReportCore.eRptChkMarks rptChkMarks,
            Boolean flagOptRptWrap)
        {
            Int32 tmpFmt = (Int32)rptFileFmt;
            Int32 tmpChkMarks = (Int32)rptChkMarks;

            if (crntToolId == ToolCommonData.eToolIds.PrnAnalyse)
                ToolPrnAnalysePersist.saveDataRpt(tmpFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e W i n P r i n t e r                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current target windows printer metrics data.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void metricsSaveWinPrinter(String printerName)
        {
            _targetType = eTarget.WinPrinter;

            _winPrinterName = printerName;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e T y p e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current target type index.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void metricsSaveType(eTarget type)
        {
            _targetType = type;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e q u e s t S t r e a m O p e n                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open target stream for print job / request.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void requestStreamOpen(
            ref BinaryWriter binWriter,
            ToolCommonData.eToolIds crntToolId,
            ToolCommonData.eToolSubIds crntToolSubId,
            ToolCommonData.ePrintLang crntPDL)
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Create output file.                                            //
            //                                                                //
            //----------------------------------------------------------------//

            if (_targetType == eTarget.File)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Invoke 'Save As' dialogue.                                 //
                //                                                            //
                //------------------------------------------------------------//

                SaveFileDialog saveDialog;

                Int32 ptr,
                      len;

                String saveDirectory;

                ptr = _saveFilename.LastIndexOf("\\");

                if (ptr <= 0)
                {
                    saveDirectory = "";
                    _crntFilename = _saveFilename;
                }
                else
                {
                    len = _saveFilename.Length;

                    saveDirectory = _saveFilename.Substring(0, ptr);
                    _crntFilename = _saveFilename.Substring(ptr + 1,
                                                           len - ptr - 1);
                }

                saveDialog = new SaveFileDialog();

                saveDialog.Filter = "Print Files | *.prn";
                saveDialog.DefaultExt = "prn";
                saveDialog.RestoreDirectory = true;
                saveDialog.InitialDirectory = saveDirectory;
                saveDialog.OverwritePrompt = true;
                saveDialog.FileName = _crntFilename;

                Nullable<Boolean> dialogResult = saveDialog.ShowDialog();

                if (dialogResult == true)
                {
                    _saveFilename = saveDialog.FileName;
                    _crntFilename = _saveFilename;

                    metricsSaveFileCapt(crntToolId, crntToolSubId, crntPDL,
                                         _saveFilename);
                }
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // The print file is created in the folder associated with    //
                // the TMP environment variable.                              //
                //                                                            //
                //------------------------------------------------------------//

                _crntFilename = Environment.GetEnvironmentVariable("TMP") +
                                "\\" +
                                DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") +
                                ".dia";
            }

            try
            {
                _opStream = File.Create(_crntFilename);
            }

            catch (IOException e)
            {
                MessageBox.Show("IO Exception:\r\n" +
                                 e.Message + "\r\n" +
                                 "Creating file '" + _crntFilename,
                                 "Target file",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }

            if (_opStream != null)
            {
                _binWriter = new BinaryWriter(_opStream);
                binWriter = _binWriter;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e q u e s t S t r e a m W r i t e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write print job / request stream to target device.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void requestStreamWrite(Boolean keepNetConnect)
        {
            _binWriter.Close();
            _opStream.Close();

            if (_targetType != eTarget.File)
            {
                try
                {
                    File.Delete(_crntFilename);
                }

                catch (IOException e)
                {
                    MessageBox.Show("IO Exception:\r\n" +
                                     e.Message + "\r\n" +
                                     "Deleting file '" + _crntFilename,
                                     "Target stream",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s p o n s e C l o s e C o n n e c t i o n                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close connection (after having read response block(s)).            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void responseCloseConnection()
        {

        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s p o n s e R e a d B l o c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read response block into supplied buffer.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static Boolean responseReadBlock(Int32 offset,
                                                 Int32 bufRem,
                                                 ref Byte[] replyData,
                                                 ref Int32 blockLen)
        {
            Boolean OK = true;

            return OK;
        }
    }
}
