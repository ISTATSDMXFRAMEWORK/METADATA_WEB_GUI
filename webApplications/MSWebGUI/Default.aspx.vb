Imports WebServiceClient
Imports System
Imports System.Data
Imports System.Xml
Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        

    End Sub

    Protected Sub btnSendQuery_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSendQuery.Click
        Dim _WsConfigurationSettings As New WebServiceClient.WsConfigurationSettings
        Dim _WebServiceClient As WebServiceClient.WebServiceClient = Nothing
        Dim sdmxRequest As New XmlDocument
        Dim sdmxResponse As New XmlDocument
        Try
            _WsConfigurationSettings.EndPoint = "http://localhost/NSIWS2_1/NSIService.asmx"
            _WsConfigurationSettings.WSDL = "http://localhost/NSIWS2_1/NSIService.asmx?wsdl"
            _WsConfigurationSettings.Prefix = "web"
            _WsConfigurationSettings.Operation = "QueryStructure"
            _WsConfigurationSettings.UseSystemProxy = False
            _WsConfigurationSettings.EnableHTTPAuthenication = False
            _WebServiceClient = New WebServiceClient.WebServiceClient(_WsConfigurationSettings)
            sdmxRequest.LoadXml(txtRequest.Text)
            sdmxResponse = _WebServiceClient.InvokeMethod(sdmxRequest)
            txtResponse.Text = sdmxResponse.OuterXml.ToString
        Catch ex As Exception
            lblError.Text = ex.Message
        End Try
    End Sub
End Class
