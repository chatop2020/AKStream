using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using SIPSorcery.Sys;

namespace LibCommon.Structs.GB28181.Sys
{
    public class SIPSorcerySMTP
    {
        // 以下，如果用到须要对应的配置
        private static readonly string m_smtpServer = AppState.GetConfigSetting("SMTPServer");
        private static readonly string m_smtpServerPort = AppState.GetConfigSetting("SMTPServerPort");
        private static readonly string m_smtpServerUseSSL = AppState.GetConfigSetting("SMTPServerUseSSL");
        private static readonly string m_smtpSendUsername = AppState.GetConfigSetting("SMTPServerUsername");
        private static readonly string m_smtpSendPassword = AppState.GetConfigSetting("SMTPServerPassword");

        public static void SendEmail(string toAddress, string fromAddress, string subject, string messageBody)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                SendEmailAsync(toAddress, fromAddress, null, null, subject, messageBody);
            });
        }

        public static void SendEmail(string toAddress, string fromAddress, string ccAddress, string bccAddress,
            string subject, string messageBody)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                SendEmailAsync(toAddress, fromAddress, ccAddress, bccAddress, subject, messageBody);
            });
        }

        private static void SendEmailAsync(string toAddress, string fromAddress, string ccAddress, string bccAddress,
            string subject, string messageBody)
        {
            if (toAddress.IsNullOrBlank())
            {
                throw new ApplicationException("An email cannot be sent with an empty To address.");
            }
            else
            {
                try
                {
                    // Send an email.
                    using var email = new MailMessage(fromAddress, toAddress, subject, messageBody)
                    {
                        BodyEncoding = Encoding.UTF8
                    };

                    // Get around bare line feed issue with IIS and qmail.
                    if (messageBody != null)
                    {
                        messageBody = Regex.Replace(messageBody, @"(?<!\r)\n", "\r\n");
                    }

                    if (!ccAddress.IsNullOrBlank())
                    {
                        email.CC.Add(new MailAddress(ccAddress));
                    }

                    if (!bccAddress.IsNullOrBlank())
                    {
                        email.Bcc.Add(new MailAddress(bccAddress));
                    }

                    if (!m_smtpServer.IsNullOrBlank())
                    {
                        RelayMail(email);
                    }
                    else
                    {
                        using var smtpClient = new SmtpClient
                        {
                            DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis
                        };
                        smtpClient.Send(email);
                        //logger.Debug("Email sent to " + toAddress);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        private static void RelayMail(MailMessage email)
        {
            try
            {
                int smtpPort = (m_smtpServerPort.IsNullOrBlank()) ? 25 : Convert.ToInt32(m_smtpServerPort);
                using SmtpClient smtpClient = new SmtpClient(m_smtpServer, smtpPort);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                if (!m_smtpServerUseSSL.IsNullOrBlank())
                {
                    smtpClient.EnableSsl = Convert.ToBoolean(m_smtpServerUseSSL);
                }

                if (!m_smtpSendUsername.IsNullOrBlank())
                {
                    smtpClient.Credentials = new NetworkCredential(m_smtpSendUsername, m_smtpSendPassword, "");
                }

                smtpClient.Send(email);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}