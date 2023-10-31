using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for MainForm.xaml
    /// 
    /// This is the main form of the PCLParaphernalia application.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    [System.Reflection.ObfuscationAttribute(Feature = "renaming",
                                            ApplyToMembers = true)]

    public partial class MainForm : Window
    {
        public const String _regMainKey = "Software\\PCLParaphernalia";

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        //    private ToolPatternGenerate     _subFormToolPatternGenerate     = null;
        private ToolPrnAnalyse _subFormToolPrnAnalyse = null;

        private ToolCommonData.eToolIds _crntToolId =
            ToolCommonData.eToolIds.Min;

        private ToolCommonData.ePrintLang _crntPDL =
            ToolCommonData.ePrintLang.Unknown;

        private ToolCommonData.eToolSubIds _crntSubId =
            ToolCommonData.eToolSubIds.None;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // M a i n f o r m                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public MainForm(String filename)
        {
            InitializeComponent();

            Int32 mwLeft = -1,
                  mwTop = -1,
                  mwHeight = -1,
                  mwWidth = -1,
                  mwScale = 100;

            Int32 versionMajorOld = -1;
            Int32 versionMinorOld = -1;
            Int32 versionBuildOld = -1;
            Int32 versionRevisionOld = -1;

            Int32 versionMajorCrnt = -1;
            Int32 versionMinorCrnt = -1;
            Int32 versionBuildCrnt = -1;
            Int32 versionRevisionCrnt = -1;

            Double windowScale = 1.0;

            //----------------------------------------------------------------//
            //                                                                //
            // Load window state values from registry.                        //
            //                                                                //
            //----------------------------------------------------------------//

            MainFormPersist.loadWindowData(ref mwLeft,
                                           ref mwTop,
                                           ref mwHeight,
                                           ref mwWidth,
                                           ref mwScale);

            if ((mwLeft == -1) || (mwTop == -1) ||
                (mwHeight == -1) || (mwWidth == -1))
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.Width = 801;
                this.Height = 842;
            }
            else
            {
                this.WindowStartupLocation = WindowStartupLocation.Manual;

                this.Left = mwLeft;
                this.Top = mwTop;
                this.Height = mwHeight;
                this.Width = mwWidth;
            }

            if ((mwScale < 25) || (mwScale > 1000))
            {
                mwScale = 100;
            }

            windowScale = ((Double)mwScale / 100);

            MainFormData.WindowScale = windowScale;

            zoomSlider.Value = windowScale;

            //----------------------------------------------------------------//
            //                                                                //
            // Check for version-specific updates.                            //
            //                                                                //
            //----------------------------------------------------------------//

            Assembly assembly = Assembly.GetExecutingAssembly();

            AssemblyName assemblyName = assembly.GetName();

            versionMajorCrnt = (Int32)assemblyName.Version.Major;
            versionMinorCrnt = (Int32)assemblyName.Version.Minor;
            versionBuildCrnt = (Int32)assemblyName.Version.Build;
            versionRevisionCrnt = (Int32)assemblyName.Version.Revision;

            MainFormData.setVersionData(true, versionMajorCrnt,
                                               versionMinorCrnt,
                                               versionBuildCrnt,
                                               versionRevisionCrnt);

            MainFormPersist.loadVersionData(ref versionMajorOld,
                                             ref versionMinorOld,
                                             ref versionBuildOld,
                                             ref versionRevisionOld);

            MainFormData.setVersionData(false, versionMajorOld,
                                                versionMinorOld,
                                                versionBuildOld,
                                                versionRevisionOld);

            if ((versionMajorCrnt != versionMajorOld) ||
                (versionMinorCrnt != versionMinorOld) ||
                (versionBuildCrnt != versionBuildOld) ||
                (versionRevisionCrnt != versionRevisionOld))
            {
                MainFormData.VersionChange = true;
            }
            else
            {
                MainFormData.VersionChange = false;
            }

            if (versionMajorOld == -1)
            {
                //----------------------------------------------------------------//
                //                                                                //
                // First run of post 2.5.0.0 version.                             //
                // Invoke default working folder dialogue.                        //
                //                                                                //
                //----------------------------------------------------------------//

                WorkFolder workFolder = new WorkFolder();

                Nullable<Boolean> dialogResult = workFolder.ShowDialog();
            }

            MainFormPersist.saveVersionData(versionMajorCrnt,
                                             versionMinorCrnt,
                                             versionBuildCrnt,
                                             versionRevisionCrnt);

            ToolCommonData.loadWorkFoldername();

            //----------------------------------------------------------------//
            //                                                                //
            // Load Target state values from registry.                        //
            //                                                                //
            //----------------------------------------------------------------//

            TargetCore.initialiseSettings();

            if (TargetCore.getType() == TargetCore.eTarget.File)
            {
                menuItemTargetFile.IsChecked = true;
                menuItemTargetNetPrinter.IsChecked = false;
                menuItemTargetWinPrinter.IsChecked = false;
            }
            else if (TargetCore.getType() == TargetCore.eTarget.NetPrinter)
            {
                menuItemTargetFile.IsChecked = false;
                menuItemTargetNetPrinter.IsChecked = true;
                menuItemTargetWinPrinter.IsChecked = false;
            }
            else if (TargetCore.getType() == TargetCore.eTarget.WinPrinter)
            {
                menuItemTargetFile.IsChecked = false;
                menuItemTargetNetPrinter.IsChecked = false;
                menuItemTargetWinPrinter.IsChecked = true;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Load tool.                                                     //
            // If a command-line parameter is present, load the               //
            // 'PRN File Analyse' tool, and pass the parameter which          //
            // identifies the file to be analysed.                            //
            // Otherwise, load the tool in use when the application was last  //
            // closed.                                                        // 
            //                                                                //
            //----------------------------------------------------------------//

            ToolCommonData.eToolIds startToolId;

            _crntToolId = ToolCommonData.eToolIds.Min;

            if (filename != "")
            {
                //------------------------------------------------------------//
                //                                                            //
                // Load 'PRN File Analyse' tool and pass in file name.        //
                //                                                            //
                //------------------------------------------------------------//

                startToolId = ToolCommonData.eToolIds.PrnAnalyse;

                toolPrnAnalyse_Selected(this, null);

                if (filename != "")
                    _subFormToolPrnAnalyse.prnFileProcess(filename);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Load Tool state values from registry.                      //
                //                                                            //
                //------------------------------------------------------------//

                Int32 crntToolIndex = 0;

                ToolCommonPersist.loadData(ref crntToolIndex);

                if ((crntToolIndex > (Int32)ToolCommonData.eToolIds.Min) &&
                    (crntToolIndex < (Int32)ToolCommonData.eToolIds.Max))
                    startToolId = (ToolCommonData.eToolIds)crntToolIndex;
                else
                    startToolId = ToolCommonData.eToolIds.PrintLang;
                
                if (startToolId ==
                    ToolCommonData.eToolIds.PrnAnalyse)
                    toolPrnAnalyse_Selected(this, null);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l R e s e t P D L                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve the current PDL selected within the current tool.         //
        // This is so that if TargetFile is configured, any new value is      //
        // stored in the appropriate PDL-specific registry key.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void crntToolResetPDL()
        {
            if (_crntToolId ==
                ToolCommonData.eToolIds.PrnAnalyse)
                _subFormToolPrnAnalyse.giveCrntPDL(ref _crntPDL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l R e s e t S u b I d                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve the current sub-identifier (if any) selected within the   //
        // current tool.                                                      //
        // This is so that if TargetFile is configured, any new value is      //
        // stored in the appropriate PDL-specific registry key.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void crntToolResetSubId()
        {
            _crntSubId = ToolCommonData.eToolSubIds.None;

        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l R e s e t T a r g e t                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset 'current tool' button details (where necessary) after Target //
        // changed.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void crntToolResetTarget()
        {
            if (_crntToolId ==
                ToolCommonData.eToolIds.PrnAnalyse)
                _subFormToolPrnAnalyse.resetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l S a v e M e t r i c s                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save metrics for last active subform.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void crntToolSaveMetrics()
        {
            if (_crntToolId != ToolCommonData.eToolIds.Min)
                ToolCommonPersist.saveData((Int32)_crntToolId);
            else if (_crntToolId ==
                ToolCommonData.eToolIds.PrnAnalyse)
                _subFormToolPrnAnalyse.metricsSave();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l U n c h e c k A l l                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called whenever current tool is selected/changed.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void crntToolUncheckAll()
        {
            //  menuItemToolPatternGenerate.IsChecked = false;
            menuItemToolPrnAnalyse.IsChecked = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f i l e E x i t _ C l i c k                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Application shutdown.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void fileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h e l p A b o u t _ C l i c k                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Help | About' menu item is selected.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void helpAbout_Click(object sender, RoutedEventArgs e)
        {
            String deploymentVersion = "";
            String assemblyVersion = "";
            String crntVersion = "";

            if (ApplicationDeployment.IsNetworkDeployed)
                deploymentVersion =
                    ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            else
                deploymentVersion = "Stand-alone";

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            assemblyVersion = assemblyName.Version.ToString();

            if (deploymentVersion == assemblyVersion)
                crntVersion = "Version " + deploymentVersion;
            else
                crntVersion = "Deployment Version: " +
                              deploymentVersion + "\r\n" +
                              "Assembly Version: " +
                              assemblyVersion;

            MessageBox.Show("PCL Paraphernalia\r\n\r\n" +
                             crntVersion + "\r\n\r\n" +
                             "To report errors, please open an issue on\r\n" +
                             "https://github.com/michaelknigge/pclparaphernalia/issues\r\n\r\n" +
                             "Source code is available on GitHub, see\r\n" +
                             "https://github.com/michaelknigge/pclparaphernalia",
                             "Help About",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h e l p C o n t e n t s _ C l i c k                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Help | Contents' menu item is selected.           //
        // Note that WPF does not have the Help class as per WinForms.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void helpContents_Click(object sender, RoutedEventArgs e)
        {
            String appStartPath = Path.GetDirectoryName(
                Process.GetCurrentProcess().MainModule.FileName);

            String helpFile = appStartPath + @"\PCLParaphernalia.chm";

            if (File.Exists(helpFile))
            {
                Process.Start(helpFile);
            }
            else
            {
                MessageBox.Show("Help file '" + helpFile +
                                "' does not exist.",
                                "Help file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t F i l e S e l e c t _ C l i c k                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | File | Select' item is selected.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetFileSelect_Click(object sender, RoutedEventArgs e)
        {
            menuItemTargetFile.IsChecked = true;
            menuItemTargetNetPrinter.IsChecked = false;
            menuItemTargetWinPrinter.IsChecked = false;

            TargetCore.metricsSaveType(TargetCore.eTarget.File);

            crntToolResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t F i l e C o n f i g u r e _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | File | Configure' item is selected.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetFileConfigure_Click(object sender,
                                               RoutedEventArgs e)
        {
            crntToolResetPDL();

            crntToolResetSubId();

            TargetFile targetFile = new TargetFile(_crntToolId, _crntSubId,
                                                    _crntPDL);

            Nullable<Boolean> dialogResult = targetFile.ShowDialog();

            if (dialogResult == true)
            {
                menuItemTargetFile.IsChecked = true;
                menuItemTargetNetPrinter.IsChecked = false;
                menuItemTargetWinPrinter.IsChecked = false;

                TargetCore.metricsSaveType(TargetCore.eTarget.File);

                crntToolResetTarget();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t N e t P r i n t e r S e l e c t _ C l i c k            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Network Printer | Select' item is        //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetNetPrinterSelect_Click(object sender,
                                                   RoutedEventArgs e)
        {
            menuItemTargetFile.IsChecked = false;
            menuItemTargetNetPrinter.IsChecked = true;
            menuItemTargetWinPrinter.IsChecked = false;

            TargetCore.metricsSaveType(TargetCore.eTarget.NetPrinter);

            crntToolResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t N e t P r i n t e r C o n f i g u r e _ C l i c k      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Network Printer | Configure' item is     //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetNetPrinterConfigure_Click(object sender,
                                                      RoutedEventArgs e)
        {
            TargetNetPrintConfig targetNetPrintConfig =
                new TargetNetPrintConfig();

            Nullable<Boolean> dialogResult = targetNetPrintConfig.ShowDialog();

            if (dialogResult == true)
            {
                menuItemTargetFile.IsChecked = false;
                menuItemTargetNetPrinter.IsChecked = true;
                menuItemTargetWinPrinter.IsChecked = false;

                TargetCore.metricsSaveType(TargetCore.eTarget.NetPrinter);

                crntToolResetTarget();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t R p t F i l e C o n f i g u r e _ C l i c k            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Report File | Configure' item is         //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetRptFileConfigure_Click(object sender,
                                                   RoutedEventArgs e)
        {
            crntToolResetPDL();

            crntToolResetSubId();

            TargetRptFile targetRptFile = new TargetRptFile(_crntToolId,
                                                             _crntSubId,
                                                             _crntPDL);

            Nullable<Boolean> dialogResult = targetRptFile.ShowDialog();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t W i n P r i n t e r S e l e c t _ C l i c k            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Windows Printer | Select' item is        //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetWinPrinterSelect_Click(object sender,
                                                   RoutedEventArgs e)
        {
            menuItemTargetFile.IsChecked = false;
            menuItemTargetNetPrinter.IsChecked = false;
            menuItemTargetWinPrinter.IsChecked = true;

            TargetCore.metricsSaveType(TargetCore.eTarget.WinPrinter);

            crntToolResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t W i n P r i n t e r C o n f i g u r e _ C l i c k      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Printer | Configure' item is selected.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetWinPrinterConfigure_Click(object sender,
                                                      RoutedEventArgs e)
        {
            TargetWinPrintConfig targetWinPrintConfig =
                new TargetWinPrintConfig();

            Nullable<Boolean> dialogResult = targetWinPrintConfig.ShowDialog();

            if (dialogResult == true)
            {
                menuItemTargetFile.IsChecked = false;
                menuItemTargetNetPrinter.IsChecked = false;
                menuItemTargetWinPrinter.IsChecked = true;

                TargetCore.metricsSaveType(TargetCore.eTarget.WinPrinter);

                crntToolResetTarget();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l P r n A n a l y s e _ S e l e c t e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Prn Analyse' item is selected.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolPrnAnalyse_Selected(object sender,
                                             RoutedEventArgs e)
        {
            crntToolSaveMetrics();
            crntToolUncheckAll();

            menuItemToolPrnAnalyse.IsChecked = true;

            _crntToolId = ToolCommonData.eToolIds.PrnAnalyse;

            _subFormToolPrnAnalyse = new ToolPrnAnalyse(ref _crntPDL);

            TargetCore.metricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolPrnAnalyse.Content;

            _subFormToolPrnAnalyse.Content = null;
            _subFormToolPrnAnalyse.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // W i n d o w _ C l o s i n g                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store target and window metrics.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void Window_Closing(object sender,
                                    System.ComponentModel.CancelEventArgs e)
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Save data from last active subform.                            //
            //                                                                //
            //----------------------------------------------------------------//

            crntToolSaveMetrics();

            //----------------------------------------------------------------//
            //                                                                //
            // Store current window metrics.                                  //
            //                                                                //
            //----------------------------------------------------------------//

            MainFormPersist.saveWindowData(
                (Int32)this.Left,
                (Int32)this.Top,
                (Int32)this.Height,
                (Int32)this.Width,
                (Int32)(MainFormData.WindowScale * 100));
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // z o o m S l i d e r _ V a l u e C h a n g e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'zoomSlider' object is changed.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void zoomSlider_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            MainFormData.WindowScale = zoomSlider.Value;
        }

        /*
        private void Form1_Load(object sender, EventArgs e)
        {
            // set F1 help topic for this form
            helpProvider1.HelpNamespace = Application.StartupPath + @"\" + sHTMLHelpFileName;
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, @"/Garden/garden.htm");
            helpProvider1.SetHelpNavigator(this.btnStart, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this.btnStart, @"/Garden/flowers.htm");
            helpProvider1.SetHelpNavigator(this.btnExit, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this.btnExit, @"/Garden/tree.htm");
            helpProvider1.SetHelpNavigator(this.chkMain, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this.chkMain, @"/HTMLHelp_Examples/jump_to_anchor.htm#AnchorSample");
        }
        */
    }
}
