﻿using System;
using System.IO;
using System.Windows;
using clawSoft.clawPDF.Core.Actions;
using clawSoft.clawPDF.Core.Jobs;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Shared.ViewModels;
using clawSoft.clawPDF.Shared.Views;
using clawSoft.clawPDF.Utilities;
using clawSoft.clawPDF.Utilities.Registry;

namespace clawSoft.clawPDF.Workflow
{
    /// <summary>
    ///     The autosave workflow implements the workflow steps with auto save. Most interaction requests are answered with
    ///     default values or are rejected, as no interaction is possible.
    /// </summary>
    internal class AutoSaveWorkflow : ConversionWorkflow
    {
        /// <summary>
        ///     Create a new Workflow object with the given job info
        /// </summary>
        /// <param name="job">Job to use for the conversion</param>
        /// <param name="settings">Settings to use during the conversion workflow</param>
        public AutoSaveWorkflow(IJob job, clawPDFSettings settings)
        {
            WorkflowStep = WorkflowStep.Init;

            JobInfo = job.JobInfo;
            Job = job;
            Settings = settings;
        }

        protected override void QueryTargetFile()
        {
            var outputPath = RegistryUtility.ReadRegistryValue(@"Software\clawSoft\clawPDF\Batch", "OutputPath") ?? "";
            var tr = Job.TokenReplacer;
            string outputFolder;

            if (!string.IsNullOrEmpty(outputPath))
            {
                outputFolder = FileUtil.Instance.MakeValidFolderName(tr.ReplaceTokens(outputPath));
                RegistryUtility.DeleteRegistryValue(@"Software\clawSoft\clawPDF\Batch", "OutputPath");
            }
            else
            {
                outputFolder = FileUtil.Instance.MakeValidFolderName(tr.ReplaceTokens(Job.Profile.AutoSave.TargetDirectory));
            }
                
            var filePath = Path.Combine(outputFolder, Job.ComposeOutputFilename());

            try
            {
                filePath = FileUtil.Instance.EllipsisForTooLongPath(filePath);
                Logger.Debug("FilePath after ellipsis: " + filePath);
            }
            catch (ArgumentException)
            {
                Logger.Error(
                    "Autosave filepath is only a directory or the directory itself is already too long to append a filename under the limits of Windows (max "
                    + FileUtil.MAX_PATH + " characters): " + filePath);
                Cancel = true;
                return;
            }

            Job.OutputFilenameTemplate = filePath;
        }

        protected override bool QueryEmailSmtpPassword()
        {
            Job.Passwords.SmtpPassword = Job.Profile.EmailSmtp.Password;

            return true;
        }

        protected override bool QueryFtpPassword()
        {
            Job.Passwords.FtpPassword = Job.Profile.Ftp.Password;

            return true;
        }

        protected override bool QueryInboundConnectApiKey()
        {
            if (!string.IsNullOrEmpty(Job.Profile.InboundConnect.ApiKey))
            {
                Job.Passwords.InboundConnectApiKey = Job.Profile.InboundConnect.ApiKey;
                return true;
            }

            var pwWindow = new InboundConnectApiKeyWindow(InboundConnectApiKeyMiddleButton.Skip);
            pwWindow.ShowDialogTopMost();

            if (pwWindow.Response == InboundConnectApiKeyResponse.OK)
            {
                Job.Passwords.InboundConnectApiKey = pwWindow.InboundConnectApiKey;
                return true;
            }

            if (pwWindow.Response == InboundConnectApiKeyResponse.Skip)
            {
                Job.Profile.PdfSettings.Signature.Enabled = false;
                Logger.Info("User skipped Inbound Connect Api Key. Upload disabled.");
                return true;
            }

            Cancel = true;
            Logger.Warn("Cancelled the Inbound Connect Api Key dialog. No PDF will be created.");
            WorkflowStep = WorkflowStep.AbortedByUser;
            return false;
        }

        protected override bool CaptureInboundConnectBookingNumber()
        {
            var pwWindow = new InboundConnectBookingNumberWindow();
            pwWindow.ShowDialogTopMost();

            if (pwWindow.Response == InboundConnectBookingNumberResponse.OK)
            {
                Job.Passwords.InboundConnectBookingNumber = pwWindow.InboundConnectBookingNumber;
                return true;
            }

            Cancel = true;
            Logger.Warn("Cancelled the Inbound Connect Api Key dialog. No PDF will be created.");
            WorkflowStep = WorkflowStep.AbortedByUser;
            return false;
        }

        protected override bool QueryEncryptionPasswords()
        {
            Job.Passwords.PdfOwnerPassword = Job.Profile.PdfSettings.Security.OwnerPassword;
            Job.Passwords.PdfUserPassword = Job.Profile.PdfSettings.Security.UserPassword;

            return true;
        }

        protected override bool QuerySignaturePassword()
        {
            Job.Passwords.PdfSignaturePassword = Job.Profile.PdfSettings.Signature.SignaturePassword;

            return true;
        }

        protected override void NotifyUserAboutFailedJob()
        {
            // maybe do something here?
        }

        protected override bool EvaluateActionResult(ActionResult actionResult)
        {
            return actionResult.Success;
        }

        protected override void RetypeSmtpPassword(object sender, QueryPasswordEventArgs e)
        {
            Logger.Error("No retype smtp password event in auto save mode.");
            e.Cancel = true;
        }
    }
}