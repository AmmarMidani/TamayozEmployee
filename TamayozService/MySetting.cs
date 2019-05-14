using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace TamayozService
{
    public static class MySetting
    {
        static string DatabaseURL = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TamayozEmployee";
        static string DatabaseFile = DatabaseURL + @"\db.json";
        static string DatabaseIconListFolder = DatabaseURL + @"\Icons\";
        static string DatabaseConstIconListFolder = DatabaseURL + @"\ConstIcons\";
        static string TempIconFolder = DatabaseURL + @"\temp\";
        static string DatabaseIconListFile = DatabaseURL + @"\IconsList.json";
        static string DatabaseNotification = DatabaseURL + @"\ConstNotificationList.json";

        static string Api_BaseUrl = "https://employees.t-tamayoz.com/api/";
        static string Api_ListIcons = "getNotifiIcons";
        static string Api_Login = "login";
        static string Api_ConstNotification = "getConstNotification";

        public static void CheckLocalFilesSystems()
        {
            if (!Directory.Exists(DatabaseURL)) //Check Directory
            {
                Directory.CreateDirectory(DatabaseURL);
            }
            if (!File.Exists(DatabaseFile)) //Check File
            {
                var SettingFile = File.Create(DatabaseFile);
                SettingFile.Close();
                JObject o = new JObject();
                o["Auther"] = "Ammar Midani (https://fb.com/eng.ammar.midani)";
                o["BuildVersion"] = "1";
                o["User"] = 0;
                o["Department"] = 0;
                TextWriter tw = new StreamWriter(DatabaseFile);
                tw.WriteLine(o);
                tw.Close();
            }
            if (!Directory.Exists(DatabaseIconListFolder)) //Check Directory
            {
                Directory.CreateDirectory(DatabaseIconListFolder);
            }
            if (!Directory.Exists(DatabaseConstIconListFolder)) //Check Directory
            {
                Directory.CreateDirectory(DatabaseConstIconListFolder);
            }
            if (!File.Exists(DatabaseIconListFile)) //Check File
            {
                var SettingFile = File.Create(DatabaseIconListFile);
                SettingFile.Close();
                TextWriter tw = new StreamWriter(DatabaseIconListFile);
                tw.WriteLine("[]");
                tw.Close();
            }
            if (!Directory.Exists(TempIconFolder)) //Check Directory
            {
                Directory.CreateDirectory(TempIconFolder);
            }
            if (!File.Exists(DatabaseNotification)) //Check File
            {
                var SettingFile = File.Create(DatabaseNotification);
                SettingFile.Close();
                TextWriter tw = new StreamWriter(DatabaseNotification);
                tw.WriteLine("[]");
                tw.Close();
            }
        }

        public static List<StaticNotification> RunTimer()
        {
            List<StaticNotification> all_times = new List<StaticNotification>();
            try
            {
                string result = File.ReadAllText(DatabaseNotification);
                JArray arr = JArray.Parse(result);
                foreach (var item in arr)
                {
                    //Sunday = 0 in C#
                    //Saturday = 0 in Our Server
                    int day_index = 0;
                    switch (DateTime.Now.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            day_index = 1;
                            break;
                        case DayOfWeek.Monday:
                            day_index = 2;
                            break;
                        case DayOfWeek.Tuesday:
                            day_index = 3;
                            break;
                        case DayOfWeek.Wednesday:
                            day_index = 4;
                            break;
                        case DayOfWeek.Thursday:
                            day_index = 5;
                            break;
                        case DayOfWeek.Friday:
                            day_index = 6;
                            break;
                        case DayOfWeek.Saturday:
                            day_index = 0;
                            break;
                    }
                    if (item["days"][day_index].ToString() == "1")
                    {
                        StaticNotification static_noti = new StaticNotification()
                        {
                            Id = int.Parse(item["id"].ToString()),
                            Content = item["content"].ToString(),
                            Title = item["title"].ToString(),
                            Icon_url = item["icon_url"].ToString(),
                            Local_icon_url = item["local_icon_url"].ToString(),
                            Time = item["time"].ToString(),
                            Days = item["days"] as JArray
                        };
                        all_times.Add(static_noti);
                    }
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.Message);
            }
            return all_times;

        }

        public static async Task GetIconList()
        {
            //Debug.WriteLine("GetIconList: Start Get List");
            var values = new Dictionary<string, string>
            {
                {"secureToken", "fc712a198e2520b9f3773518a519e415asdtwe"},
            };
            using (var client = new HttpClient())
            {
                try
                {
                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync(Api_BaseUrl + Api_ListIcons, content);
                    Task<string> responseString = response.Content.ReadAsStringAsync();
                    string outputJson = await responseString;
                    var SettingFile = File.Create(DatabaseIconListFile);
                    SettingFile.Close();
                    JToken token = JObject.Parse(outputJson);
                    int error_code = (int)token["error_code"];
                    File.WriteAllText(DatabaseIconListFile, "[]");
                    if (error_code == 0)
                    {
                        TextWriter tw = new StreamWriter(DatabaseIconListFile);
                        JArray arr = new JArray();
                        foreach (var item in token["data"])
                        {
                            var downloadFileUrl = item["icon_url"].ToString();
                            var destinationFilePath = DatabaseIconListFolder;
                            Uri uri = new Uri(downloadFileUrl);
                            destinationFilePath = destinationFilePath + Path.GetFileName(uri.LocalPath);
                            using (var hcdp = new HttpClientDownloadWithProgress(downloadFileUrl, destinationFilePath))
                            {
                                //hcdp.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                                //{
                                //    //Debug.WriteLine($"{progressPercentage}% ({totalBytesDownloaded}/{totalFileSize})");
                                //};
                                await hcdp.StartDownload();
                            }
                            JObject o = new JObject();
                            o["web"] = item["icon_url"];
                            o["local"] = destinationFilePath;
                            arr.Add(o);
                        }
                        tw.WriteLine(arr);
                        tw.Close();
                    }
                }
                catch (Exception)
                {
                    //ex.ToString();
                }
            }
            //Debug.WriteLine("GetIconList: End Get List");
        }

        public static async Task GetConstNotifications()
        {
            //Debug.WriteLine("GetConstNotifications: Start Get List");
            var values = new Dictionary<string, string>
            {
                {"secureToken", "fc712a198e2520b9f3773518a519e415asdtwe"},
            };
            using (var client = new HttpClient())
            {
                try
                {
                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync(Api_BaseUrl + Api_ConstNotification, content);
                    Task<string> responseString = response.Content.ReadAsStringAsync();
                    string outputJson = await responseString;
                    var SettingFile = File.Create(DatabaseNotification);
                    SettingFile.Close();
                    JToken token = JObject.Parse(outputJson);
                    int error_code = (int)token["error_code"];
                    File.WriteAllText(DatabaseNotification, "[]");
                    if (error_code == 0)
                    {
                        JArray arr = new JArray();
                        foreach (var item in token["data"])
                        {
                            var downloadFileUrl = item["icon_url"].ToString();
                            var destinationFilePath = DatabaseConstIconListFolder;
                            Uri uri = new Uri(downloadFileUrl);
                            destinationFilePath = destinationFilePath + Path.GetFileName(uri.LocalPath);
                            using (var hcdp = new HttpClientDownloadWithProgress(downloadFileUrl, destinationFilePath))
                            {
                                //hcdp.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                                //{
                                //    //Debug.WriteLine($"{progressPercentage}% ({totalBytesDownloaded}/{totalFileSize})");
                                //};
                                await hcdp.StartDownload();
                            }
                            JObject o = new JObject();

                            o["id"] = int.Parse(item["id"].ToString());
                            o["title"] = item["title"];
                            o["content"] = item["content"];
                            o["icon_url"] = item["icon_url"];
                            o["time"] = item["time"];
                            o["days"] = item["days"];
                            o["local_icon_url"] = destinationFilePath;
                            arr.Add(o);
                        }
                        TextWriter tw = new StreamWriter(DatabaseNotification);
                        tw.WriteLine(arr);
                        tw.Close();
                    }
                }
                catch (Exception ex)
                {
                    //Debug.WriteLine(ex.Message);
                }
            }
            //Debug.WriteLine("GetConstNotifications: End Get List");
        }

        public static async Task<Dictionary<string, string>> PostLoginAPI(string mobile, string password)
        {
            var values = new Dictionary<string, string>
            {
                {"mobile", mobile},
                {"password", password},
                {"secureToken", "fc712a198e2520b9f3773518a519e415asdtwe"},
            };
            using (var client = new HttpClient())
            {
                try
                {
                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync(Api_BaseUrl + Api_Login, content);
                    Task<string> responseString = response.Content.ReadAsStringAsync();
                    string outputJson = await responseString;
                    JToken token = JObject.Parse(outputJson);
                    int error_code = (int)token["error_code"];
                    if (error_code == 0)
                    {
                        string result = File.ReadAllText(DatabaseFile);
                        JObject o = JObject.Parse(result);
                        o["User"] = int.Parse(token["data"]["user_id"].ToString());
                        o["Department"] = int.Parse(token["data"]["department_id"].ToString());
                        o["Token"] = token["data"]["token"].ToString();
                        o["EmpName"] = token["data"]["employee_name"].ToString();
                        File.WriteAllText(DatabaseFile, string.Empty);
                        TextWriter tw = new StreamWriter(DatabaseFile);
                        tw.WriteLine(o);
                        tw.Close();
                        return new Dictionary<string, string>
                        {
                            {"Status", "1"},
                            {"Message", token["data"]["employee_name"].ToString()},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, string>
                        {
                            {"Status", "0"},
                            //{"Message", token["message"].ToString()},
                            {"Message", "رقم الجوال أو كلمة المرور غير صحيح"},
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Dictionary<string, string>
                    {
                        {"Status", "2"},
                        {"Message", "Somthing happend please call admin" + ex.Message},
                    };
                }
            }
        }

        public static int GetDepartmentID()
        {
            string result = File.ReadAllText(DatabaseFile);
            JObject o = JObject.Parse(result);
            if (o["Department"] != null)
            {
                return int.Parse(o["Department"].ToString());
            }
            return 0;
        }

        public static string GetIconFromURL(string Icon)
        {
            string result = File.ReadAllText(DatabaseIconListFile);
            JArray arr = JArray.Parse(result);
            foreach (var item in arr)
            {
                if (item["web"].ToString() == Icon)
                {
                    return item["local"].ToString();
                }
            }
            string temp_path = TempIconFolder + Path.GetFileName(new System.Uri(Icon).LocalPath);
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.DownloadFile(new System.Uri(Icon), temp_path);
            }
            return temp_path;
        }

        public static int GetUserID()
        {
            string result = File.ReadAllText(DatabaseFile);
            JObject o = JObject.Parse(result);
            if (o["User"] != null)
            {
                return int.Parse(o["User"].ToString());
            }
            return 0;
        }

        public static JArray GetConstIconsIDs()
        {
            string result = File.ReadAllText(DatabaseNotification);
            JArray return_arr = new JArray();
            JArray arr = JArray.Parse(result);
            foreach (var item in arr)
            {
                return_arr.Add(item["id"]);
            }
            return return_arr;
        }

        public static JArray GetConstIcons(string time, int day)
        {
            /**
             * 0 = saturday
             * 1 = sunday
             * 2 = monday
             * 3 = tuseday
             * 4 = wednesday
             * 5 = thursday
             * 6 = friday
             */
            string result = File.ReadAllText(DatabaseNotification);
            JArray return_arr = new JArray();
            JArray arr = JArray.Parse(result);
            foreach (var item in arr)
            {
                if (item["time"].ToString() == time && JArray.Parse(item["days"].ToString())[day].ToString() == "1")
                {
                    return_arr.Add(item);
                }
            }
            return return_arr;
        }

        public static void AddNewStatic(string id, string title, string content, string icon_url, string time, JArray days)
        {
            string result = File.ReadAllText(DatabaseNotification);
            JArray arr = JArray.Parse(result);
            JObject o = new JObject();
            o["id"] = int.Parse(id);
            o["title"] = title;
            o["content"] = content;
            o["icon_url"] = icon_url;
            o["time"] = time;
            o["days"] = days;

            var downloadFileUrl = icon_url.ToString();
            var destinationFilePath = DatabaseConstIconListFolder;
            Uri uri = new Uri(downloadFileUrl);
            destinationFilePath = destinationFilePath + Path.GetFileName(uri.LocalPath);
            using (var hcdp = new HttpClientDownloadWithProgress(downloadFileUrl, destinationFilePath))
            {
                hcdp.StartDownload().Wait();
            }
            o["local_icon_url"] = destinationFilePath;
            arr.Add(o);
            //File.WriteAllText(DatabaseConstIconListFolder, "[]");
            TextWriter tw = new StreamWriter(DatabaseNotification);
            tw.WriteLine(arr);
            tw.Close();
        }

        public static void WriteConstAfterDelete(List<TamayozService.StaticNotification> all_times)
        {
            JArray final_data = new JArray();
            foreach (var item in all_times)
            {
                JObject o = new JObject();
                o["id"] = item.Id;
                o["title"] = item.Title;
                o["content"] = item.Content;
                o["icon_url"] = item.Icon_url;
                o["time"] = item.Time;
                o["days"] = item.Days;
                o["local_icon_url"] = item.Local_icon_url;
                final_data.Add(o);
            }
            TextWriter tw = new StreamWriter(DatabaseNotification, false);
            tw.Write(final_data.ToString());
            tw.Close();
        }

        public static void RemoveAllFile()
        {
            try
            {
                Directory.Delete(DatabaseConstIconListFolder, true);
                Directory.Delete(DatabaseIconListFolder, true);
                Directory.Delete(TempIconFolder, true);
                File.Delete(DatabaseNotification);
                File.Delete(DatabaseFile);
                File.Delete(DatabaseIconListFile);
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.Message);
            }
        }

        private class HttpClientDownloadWithProgress : IDisposable
        {
            private readonly string _downloadUrl;
            private readonly string _destinationFilePath;

            private HttpClient _httpClient;

            public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

            public event ProgressChangedHandler ProgressChanged;

            public HttpClientDownloadWithProgress(string downloadUrl, string destinationFilePath)
            {
                _downloadUrl = downloadUrl;
                _destinationFilePath = destinationFilePath;
            }

            public async Task StartDownload()
            {
                _httpClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };

                using (var response = await _httpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                    await DownloadFileFromHttpResponseMessage(response);
            }

            private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                    await ProcessContentStream(totalBytes, contentStream);
            }

            private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
            {
                var totalBytesRead = 0L;
                var readCount = 0L;
                var buffer = new byte[8192];
                var isMoreToRead = true;

                using (var fileStream = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    do
                    {
                        var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            isMoreToRead = false;
                            TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                            continue;
                        }

                        await fileStream.WriteAsync(buffer, 0, bytesRead);

                        totalBytesRead += bytesRead;
                        readCount += 1;

                        if (readCount % 100 == 0)
                            TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                    }
                    while (isMoreToRead);
                }
            }

            private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
            {
                if (ProgressChanged == null)
                    return;

                double? progressPercentage = null;
                if (totalDownloadSize.HasValue)
                    progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);

                ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
            }

            public void Dispose()
            {
                _httpClient?.Dispose();
            }
        }
    }
}
