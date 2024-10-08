using clawSoft.clawPDF.Core.Jobs;
using clawSoft.clawPDF.Core.Settings;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using SystemWrapper.IO;
using SystemWrapper.Net;

namespace clawSoft.clawPDF.Core.Actions
{
    public class InboundConnectAction : IAction, ICheckable
    {
        private const int ActionId = 66;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Upload all output files with ftp
        /// </summary>
        /// <param name="job">The job to process</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(IJob job)
        {
            Logger.Debug("Launched InboundConnect-Action");
            try
            {
                var result = InboundConnectUpload(job);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while upload file to InboundConnect:\r\n" + ex.Message);
                return new ActionResult(ActionId, 999);
            }
        }

        /// <summary>
        ///     Check if the profile is configured properly for this action
        /// </summary>
        /// <param name="profile">The profile to check</param>
        /// <returns>ActionResult with configuration problems</returns>
        public ActionResult Check(ConversionProfile profile)
        {
            var actionResult = new ActionResult();
            if (profile.InboundConnect.Enabled)
            {
                if (string.IsNullOrEmpty(profile.InboundConnect.Api))
                {
                    Logger.Error("No API Endpoint specified.");
                    actionResult.Add(ActionId, 100);
                }

                if (string.IsNullOrEmpty(profile.InboundConnect.UserName))
                {
                    Logger.Error("No InboundConnect username specified.");
                    actionResult.Add(ActionId, 101);
                }

                if (profile.AutoSave.Enabled)
                    if (string.IsNullOrEmpty(profile.InboundConnect.ApiKey))
                    {
                        Logger.Error("Automatic saving without InboundConnect apikey.");
                        actionResult.Add(ActionId, 109);
                    }
            }

            return actionResult;
        }

        private ActionResult InboundConnectUpload(IJob job)
        {
            var actionResult = Check(job.Profile);
            if (!actionResult)
            {
                Logger.Error("Canceled Inbound Connect upload action.");
                return actionResult;
            }

            if (string.IsNullOrEmpty(job.Passwords.InboundConnectApiKey))
            {
                Logger.Error("No Inbound Connect ApiKey specified in action");
                return new ActionResult(ActionId, 102);
            }

            if (string.IsNullOrEmpty(job.Passwords.InboundConnectBookingNumber))
            {
                Logger.Error("No Inbound Connect Booking Number specified in action");
                return new ActionResult(ActionId, 120);
            }

            var httpWebRequestWrapFactory = new HttpWebRequestWrapFactory();

            foreach (var file in job.OutputFiles)
            {
                try
                {
                    var httpWebRequest = httpWebRequestWrapFactory.Create(job.Profile.InboundConnect.Api);

                    var base64EncodedFile = Convert.ToBase64String(File.ReadAllBytes(file));

                    httpWebRequest.Method = "POST";
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Headers.Add("Inbound-API-Key", job.Profile.InboundConnect.UserName + ":" + job.Passwords.InboundConnectApiKey);

                    using (var streamWriter = new StreamWriterFactory().Create(httpWebRequest.GetRequestStream()))
                    {
                        var fileName = Path.GetFileName(file);
                        string json = "{\"bookingNumber\":\"" + job.Passwords.InboundConnectBookingNumber + "\"," +
                                      //"\"attachmentTypeCode\":\"bla\"," +  Don't specify and leave attachment types as null for now.
                                      "\"fileName\":\"" + fileName + "\"," +
                                      "\"base64FileBytes\":\"" + base64EncodedFile + "\"}";

                        streamWriter.Write(json);
                    }

                    var response = httpWebRequest.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Error("Inbound Connect upload failed with status code: " + response.StatusCode);
                        using (var streamReader = new StreamReaderWrapFactory().Create(response.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                        }
                        return new ActionResult(ActionId, 103);
                    }
                }
                catch (WebException ex)
                {
                    Logger.Error("Exception while uploading attachment to Inbound Connect:\r\n" + ex.Message);
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var response = (HttpWebResponse)ex.Response;
                        Logger.Error("Inbound Connect upload failed with status code: " + response.StatusCode);
                        using (var streamReader = new StreamReaderWrapFactory().Create(response.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            Logger.Error($"Error response: {result}");
                            var json = JsonConvert.DeserializeObject<dynamic>(result);
                            if (json?.error?.message != null)
                            {
                                return new ActionResult(ActionId, 104, json.error.message.ToString());
                            }

                        }
                    }
                    return new ActionResult(ActionId, 104);
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception while uploading attachment to Inbound Connect:\r\n" + ex.Message);
                    return new ActionResult(ActionId, 104);
                }
            }

            return new ActionResult();
        }
    }
}