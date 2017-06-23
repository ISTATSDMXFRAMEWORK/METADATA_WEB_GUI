Imports System
Imports System.Data
Imports System.Xml
Imports WebServiceClient
Imports System.IO
Partial Class download
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim artefact, id, agency, version As String
        artefact = Request.Params("artefact")
        id = Request.Params("id")
        agency = Request.Params("agency")
        version = Request.Params("version")
        Response.ContentType = "application/xml"
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & agency & "+" & id & "+" & version & ".xml")
        Response.Charset = "iso-8859-5"
        Me.EnableViewState = False
        Dim _WsConfigurationSettings As New WebServiceClient.WsConfigurationSettings
        Dim _WebServiceClient As WebServiceClient.WebServiceClient = Nothing
        Dim sdmxRequest As New XmlDocument
        Dim sdmxResponse As New XmlDocument
        Dim strCurrentPath As String = Request.PhysicalPath
        Dim strPathQueryCL As String = Nothing
        Dim strRequest As String = Nothing
        Dim myXmlTextReader As XmlTextReader = Nothing
        Dim StreamTxt As StreamReader = Nothing
        Try
            Select Case artefact
                Case "cl"
                    strPathQueryCL = Left(strCurrentPath, InStrRev(strCurrentPath, "\")) & "queries\get_a_codelist.xml"
                Case "cs"
                    strPathQueryCL = Left(strCurrentPath, InStrRev(strCurrentPath, "\")) & "queries\get_a_conceptscheme.xml"
            End Select

            StreamTxt = File.OpenText(strPathQueryCL)
            strRequest = StreamTxt.ReadToEnd
            Dim str As String = strRequest.ToString
            str = Replace(str, "AAA", agency)
            str = Replace(str, "III", id)
            str = Replace(str, "000", version)

            _WsConfigurationSettings.EndPoint = "http://localhost/NSIWS2_1/NSIService.asmx"
            _WsConfigurationSettings.WSDL = "http://localhost/NSIWS2_1/NSIService.asmx?wsdl"
            _WsConfigurationSettings.Prefix = "web"
            _WsConfigurationSettings.Operation = "QueryStructure"
            _WsConfigurationSettings.UseSystemProxy = False
            _WsConfigurationSettings.EnableHTTPAuthenication = False
            _WebServiceClient = New WebServiceClient.WebServiceClient(_WsConfigurationSettings)
            sdmxRequest.LoadXml(str)
            sdmxResponse = _WebServiceClient.InvokeMethod(sdmxRequest)
            Response.Write(sdmxResponse.OuterXml.ToString)
            Response.End()
        Catch ex As Exception
            lblerror.Text = ex.Message
        Finally
            If IsNothing(StreamTxt) = False Then
                StreamTxt.Close()
                StreamTxt = Nothing
            End If
        End Try
    End Sub
End Class
