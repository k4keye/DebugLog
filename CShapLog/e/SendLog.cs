﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CShapLog.e
{
    public class SendLog
    {
        static private SendLog instance;
        private SendLog()
        { }

        public static async void Call(string token , string log)
        {
            if (instance == null) instance = new SendLog();
            await instance.Send(token,log);
        }
        private async Task<bool> Send(string token, string log)
        {
            log = log.Replace("\r", "\\r"); //서버에게 보내는 log 는 개행문자가 다르다.
            log = log.Replace("\n", "\\n"); //서버에게 보내는 log 는 개행문자가 다르다.

            
            string uri = "server URI";
          
            string sendData = "{" + "\"log\":\"" + log + "\"}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Accept = "*/*";
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.KeepAlive = true;
            request.ServerCertificateValidationCallback = delegate { return true; };
            request.Timeout = 4000;
            request.Headers.Add("X-AUTH-TOKEN", token);
            try
            {

                StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync());
                writer.Write(sendData);
                writer.Close();

                await request.GetResponseAsync();
                return true;

            }
            catch (WebException we)
            {
                Logger.m("[WebException] : " + we.Message);

                return false;
            }
            catch (Exception LogE)
            {
                Logger.m(LogE.Message);
                return false;
            }
            finally
            {
                request.Abort();
            }
        }
    }
}
