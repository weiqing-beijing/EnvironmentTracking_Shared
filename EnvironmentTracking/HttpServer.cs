using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;

namespace EnvironmentTracking
{
    public sealed class HttpServer : IDisposable
    {
        private const uint bufLen = 8192;
        private int defaultPort = 0808;
        private readonly StreamSocketListener sock;

        public event EventHandler RecivedMeg;

        public object[] TimeStamp { get; private set; }

        public HttpServer()
        {
            sock = new StreamSocketListener();

            sock.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
        }

        public async void StartServer()
        {
            await sock.BindServiceNameAsync(defaultPort.ToString());
        }

        private async void ProcessRequestAsync(StreamSocket socket)
        {
            // Read in the HTTP request, we only care about type 'GET'
            StringBuilder request = new StringBuilder();
            using (IInputStream input = socket.InputStream)
            {
                byte[] data = new byte[bufLen];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = bufLen;
                while (dataRead == bufLen)
                {
                    await input.ReadAsync(buffer, bufLen, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
                if (RecivedMeg != null)
                {
                    Debug.WriteLine(request.ToString());
                    RecivedMeg(request.ToString(), EventArgs.Empty);
                }
            }

            using (IOutputStream output = socket.OutputStream)
            {
                string requestMethod = request.ToString().Split('\n')[0];
                string[] requestParts = requestMethod.Split(' ');
                await WriteResponseAsync(requestParts, output);
            }
        }

        private async Task WriteResponseAsync(string[] requestTokens, IOutputStream outstream)
        {
            // Content body
            string respBody = string.Format(@"<html>
                                                    <head>
                                                        <title>SHT15 Sensor Values</title>
                                                        <meta http-equiv='refresh' content='2' />
                                                    </head>
                                                    <body>
                                                        <p><font size='6'><b>Windows 10 IoT Core and SHT15 Sensor</b></font></p>
                                                        <hr/>
                                                        <br/>
                                                        <table>
                                                            <tr>
                                                                <td><font size='3'>Time</font></td>
                                                                <td><font size='3'>{0}</font></td>
                                                            </tr>
                                                            <tr>
                                                                <td><font size='5'>Temperature</font></td>
                                                                <td><font size='6'><b>{1}&deg;C</b></font></td>
                                                            </tr>
                                                            <tr>
                                                                <td><font size='5'>Temperature</font></td>
                                                                <td><font size='6'><b>{2}F</b></font></td>
                                                            </tr>
                                                            <tr>
                                                                <td><font size='5'>Humidity</font></td>
                                                                <td><font size='6'><b>{3}%</b></font></td>
                                                            </tr>
                                                           
                                                        </table>
                                                    </body>
                                                  </html>",

                                            DateTime.Now.ToString("h:mm:ss tt"),
                                            String.Format("{0:0.00}", MainPage.TemperatureC),
                                            String.Format("{0:0.00}", MainPage.TemperatureF),
                                            String.Format("{0:0.00}", MainPage.Humidity));
                                            //String.Format("{0:0.00}", MainPage.CalculatedDewPoint));

            string htmlCode = "200 OK";

            using (Stream resp = outstream.AsStreamForWrite())
            {
                byte[] bodyArray = Encoding.UTF8.GetBytes(respBody);
                MemoryStream stream = new MemoryStream(bodyArray);

                // Response heeader
                string header = string.Format("HTTP/1.1 {0}\r\n" +
                                              "Content-Type: text/html\r\n" +
                                              "Content-Length: {1}\r\n" +
                                              "Connection: close\r\n\r\n",
                                              htmlCode, stream.Length);

                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await resp.WriteAsync(headerArray, 0, headerArray.Length);
                await stream.CopyToAsync(resp);
                await resp.FlushAsync();
            }
        }

        public void Dispose()
        {
            sock.Dispose();
        }
    }
}
