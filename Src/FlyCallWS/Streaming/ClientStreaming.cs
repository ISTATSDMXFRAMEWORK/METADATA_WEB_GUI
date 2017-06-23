using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace FlyCallWS.Streaming
{
    internal class ClientStreaming
    {

        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private FileStream _fs = null;
        //bool ParsedError = false;

        public bool GetResponse(HttpWebRequest myHttpWebRequest, FileStream fs)
        {

            try
            {
                _fs = fs;

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                // Gets the stream associated with the response.
                Stream responseStream = myHttpWebResponse.GetResponseStream();
                int totRead = 0;

                var result = new StringBuilder("");
                do
                {
                    byte[] buffer = new byte[BUFFER_SIZE];
                    totRead = responseStream.Read(buffer, 0, buffer.Length);
                    if (totRead == 0)
                    {
                        break;
                    }
                    _fs.Write(buffer, 0, totRead);
                    //result.Append(Encoding.UTF8.GetString(buffer, 0, read));
                }
                while (totRead <= BUFFER_SIZE);
                return true;

            }
            catch (WebException e)
            {
                if(e.Message.Contains("504"))
                    throw new Exception("No Response from Web Service");

                if (e.Response != null)
                {
                    Stream responseStream = e.Response.GetResponseStream();
                    int totRead = 0;

                    var result = new StringBuilder("");
                    do
                    {
                        byte[] buffer = new byte[BUFFER_SIZE];
                        totRead = responseStream.Read(buffer, 0, buffer.Length);
                        if (totRead == 0)
                        {
                            break;
                        }
                        _fs.Write(buffer, 0, totRead);
                        result.Append(Encoding.UTF8.GetString(buffer, 0, totRead));

                    }
                    while (totRead <= BUFFER_SIZE);
                    return false;
                }
                else
                    throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }


}

public class RequestState
{
    // This class stores the State of the request.
    const int BUFFER_SIZE = 1024;
    public StringBuilder requestData;
    public byte[] BufferRead;
    public HttpWebRequest request;
    public HttpWebResponse response;
    public Stream streamResponse;
    public RequestState()
    {
        BufferRead = new byte[BUFFER_SIZE];
        requestData = new StringBuilder("");
        request = null;
        streamResponse = null;


    }
}
