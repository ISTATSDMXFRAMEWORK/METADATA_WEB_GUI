Imports System
Imports System.Data
Imports System.Xml
Imports WebServiceClient
Imports System.IO

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub lnkBtnCodeLists_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkBtnCodeLists.Click
        Dim _WsConfigurationSettings As New WebServiceClient.WsConfigurationSettings
        Dim _WebServiceClient As WebServiceClient.WebServiceClient = Nothing
        Dim sdmxRequest As New XmlDocument
        Dim sdmxResponse As New XmlDocument
        Dim strCurrentPath As String = Request.PhysicalPath
        Dim strPathQueryCL As String = Nothing
        Dim strRequest As String = Nothing
        Dim myXmlTextReader As XmlTextReader = Nothing
        Dim StreamTxt As StreamReader = Nothing
        Dim myXmlNamespaceManager As XmlNamespaceManager
        Dim entryList As XmlNodeList = Nothing
        Dim mySdmxStructureFile As New sdmxIstat.sdmxStructureFile
        Dim dt As New DataTable
        Try
            strPathQueryCL = Left(strCurrentPath, InStrRev(strCurrentPath, "\")) & "queries\get_all_codelist.xml"
            StreamTxt = File.OpenText(strPathQueryCL)
            strRequest = StreamTxt.ReadToEnd
            _WsConfigurationSettings.EndPoint = "http://santiago.pc.istat.it/nsiws2_1/NSIService.asmx"
            _WsConfigurationSettings.WSDL = "http://santiago.pc.istat.it/nsiws2_1/NSIService.asmx?wsdl"
            _WsConfigurationSettings.Prefix = "web"
            _WsConfigurationSettings.Operation = "QueryStructure"
            _WsConfigurationSettings.UseSystemProxy = False
            _WsConfigurationSettings.EnableHTTPAuthenication = False
            _WebServiceClient = New WebServiceClient.WebServiceClient(_WsConfigurationSettings)
            sdmxRequest.LoadXml(strRequest)
            sdmxResponse = _WebServiceClient.InvokeMethod(sdmxRequest)
            'txtResponse.Text = sdmxResponse.OuterXml.ToString
            'indent the XML ----------------------
            Dim xmlWriter As XmlTextWriter
            Dim textWriter As StringWriter
            textWriter = New StringWriter()
            xmlWriter = New XmlTextWriter(textWriter)
            xmlWriter.Formatting = Formatting.Indented
            sdmxRequest.LoadXml(sdmxResponse.OuterXml.ToString)
            sdmxRequest.Save(xmlWriter)
            txtResponse.Text = textWriter.ToString()
            '--------------------------------------
            myXmlNamespaceManager = New System.Xml.XmlNamespaceManager(sdmxResponse.NameTable)
            myXmlNamespaceManager.AddNamespace("structure", "http://www.SDMX.org/resources/SDMXML/schemas/v2_0/structure")

            entryList = sdmxResponse.SelectNodes("descendant::structure:CodeList", myXmlNamespaceManager)
            If entryList.Count > 0 Then
                dt = mySdmxStructureFile.readAllCodeLists(sdmxResponse, myXmlNamespaceManager, ddlLang.SelectedValue)
                grdCodeLists.DataSource = dt
                grdCodeLists.DataBind()
            End If
            ViewState("artefact") = "cl"
        Catch ex As Exception
            lblError.Text = ex.Message
        Finally
            If IsNothing(StreamTxt) = False Then
                StreamTxt.Close()
                StreamTxt = Nothing
            End If
        End Try
    End Sub
    Dim artefact As String
    Function bindGridView() As DataTable
        Dim _WsConfigurationSettings As New WebServiceClient.WsConfigurationSettings
        Dim _WebServiceClient As WebServiceClient.WebServiceClient = Nothing
        Dim sdmxRequest As New XmlDocument
        Dim sdmxResponse As New XmlDocument
        Dim strCurrentPath As String = Request.PhysicalPath
        Dim strPathQueryCL As String = Nothing
        Dim strRequest As String = Nothing
        Dim myXmlTextReader As XmlTextReader = Nothing
        Dim StreamTxt As StreamReader = Nothing
        Dim myXmlNamespaceManager As XmlNamespaceManager
        Dim entryList As XmlNodeList = Nothing
        Dim mySdmxStructureFile As New sdmxIstat.sdmxStructureFile
        Dim dt As New DataTable
        Try
            Select Case artefact
                Case "cl"
                    strPathQueryCL = Left(strCurrentPath, InStrRev(strCurrentPath, "\")) & "queries\get_all_codelist.xml"
                Case "cs"
                    strPathQueryCL = Left(strCurrentPath, InStrRev(strCurrentPath, "\")) & "queries\get_all_conceptschemes.xml"
            End Select

            StreamTxt = File.OpenText(strPathQueryCL)
            strRequest = StreamTxt.ReadToEnd
            _WsConfigurationSettings.EndPoint = "http://localhost/NSIWS2_1/NSIService.asmx"
            _WsConfigurationSettings.WSDL = "http://localhost/NSIWS2_1/NSIService.asmx?wsdl"
            _WsConfigurationSettings.Prefix = "web"
            _WsConfigurationSettings.Operation = "QueryStructure"
            _WsConfigurationSettings.UseSystemProxy = False
            _WsConfigurationSettings.EnableHTTPAuthenication = False
            _WebServiceClient = New WebServiceClient.WebServiceClient(_WsConfigurationSettings)
            sdmxRequest.LoadXml(strRequest)
            sdmxResponse = _WebServiceClient.InvokeMethod(sdmxRequest)
            'txtResponse.Text = sdmxResponse.OuterXml.ToString
            'indent the XML ----------------------
            Dim xmlWriter As XmlTextWriter
            Dim textWriter As StringWriter
            textWriter = New StringWriter()
            xmlWriter = New XmlTextWriter(textWriter)
            xmlWriter.Formatting = Formatting.Indented
            sdmxRequest.LoadXml(sdmxResponse.OuterXml.ToString)
            sdmxRequest.Save(xmlWriter)
            txtResponse.Text = textWriter.ToString()
            '--------------------------------------
            myXmlNamespaceManager = New System.Xml.XmlNamespaceManager(sdmxResponse.NameTable)
            myXmlNamespaceManager.AddNamespace("structure", "http://www.SDMX.org/resources/SDMXML/schemas/v2_0/structure")
            Select Case artefact
                Case "cl"
                    entryList = sdmxResponse.SelectNodes("descendant::structure:CodeList", myXmlNamespaceManager)
                    If entryList.Count > 0 Then
                        dt = mySdmxStructureFile.readAllCodeLists(sdmxResponse, myXmlNamespaceManager, ddlLang.SelectedValue)
                    End If
                Case "cs"
                    entryList = sdmxResponse.SelectNodes("descendant::structure:CodeList", myXmlNamespaceManager)
                    If entryList.Count > 0 Then
                        dt = mySdmxStructureFile.readAllConceptSchemes(sdmxResponse, myXmlNamespaceManager, ddlLang.SelectedValue)
                    End If
            End Select
            Return dt
        Catch ex As Exception
            Throw New Exception
        Finally
            If IsNothing(StreamTxt) = False Then
                StreamTxt.Close()
                StreamTxt = Nothing
            End If
        End Try
    End Function
    Sub selectCL(ByVal src As Object, ByVal e As GridViewCommandEventArgs)
        If e.CommandName = "SDMX" Then
            'get the row index stored in the CommandArgument property
            Dim index As Long = Convert.ToInt64(e.CommandArgument)
            'get the GridViewRow where the command is raised
            Dim selectedRow As GridViewRow = DirectCast(e.CommandSource, GridView).Rows(index)
            'for bound fields, values are stored in the Text property of Cells [ fieldIndex ]
            artefact = ViewState("artefact")

            Dim id As String = selectedRow.Cells(1).Text
            Dim agency As String = selectedRow.Cells(3).Text
            Dim version As String = selectedRow.Cells(4).Text
            Response.Redirect("download.aspx?artefact=" & artefact & "&agency=" & agency & "&id=" & id & "&version=" & version)
        End If
    End Sub
    Protected Sub grdCodeLists_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles grdCodeLists.PageIndexChanging
        grdCodeLists.PageIndex = e.NewPageIndex
        grdCodeLists.DataSource = bindGridView()
        grdCodeLists.DataBind()
    End Sub

    Protected Sub lnkBtnConceptScheme_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkBtnConceptScheme.Click
        Dim _WsConfigurationSettings As New WebServiceClient.WsConfigurationSettings
        Dim _WebServiceClient As WebServiceClient.WebServiceClient = Nothing
        Dim sdmxRequest As New XmlDocument
        Dim sdmxResponse As New XmlDocument
        Dim strCurrentPath As String = Request.PhysicalPath
        Dim strPathQueryCL As String = Nothing
        Dim strRequest As String = Nothing
        Dim myXmlTextReader As XmlTextReader = Nothing
        Dim StreamTxt As StreamReader = Nothing
        Dim myXmlNamespaceManager As XmlNamespaceManager
        Dim entryList As XmlNodeList = Nothing
        Dim mySdmxStructureFile As New sdmxIstat.sdmxStructureFile
        Dim dt As New DataTable
        Try
            strPathQueryCL = Left(strCurrentPath, InStrRev(strCurrentPath, "\")) & "queries\get_all_conceptschemes.xml"
            StreamTxt = File.OpenText(strPathQueryCL)
            strRequest = StreamTxt.ReadToEnd
            _WsConfigurationSettings.EndPoint = "http://localhost/NSIWS2_1/NSIService.asmx"
            _WsConfigurationSettings.WSDL = "http://localhost/NSIWS2_1/NSIService.asmx?wsdl"
            _WsConfigurationSettings.Prefix = "web"
            _WsConfigurationSettings.Operation = "QueryStructure"
            _WsConfigurationSettings.UseSystemProxy = False
            _WsConfigurationSettings.EnableHTTPAuthenication = False
            _WebServiceClient = New WebServiceClient.WebServiceClient(_WsConfigurationSettings)
            sdmxRequest.LoadXml(strRequest)
            sdmxResponse = _WebServiceClient.InvokeMethod(sdmxRequest)
            'txtResponse.Text = sdmxResponse.OuterXml.ToString
            'indent the XML ----------------------
            Dim xmlWriter As XmlTextWriter
            Dim textWriter As StringWriter
            textWriter = New StringWriter()
            xmlWriter = New XmlTextWriter(textWriter)
            xmlWriter.Formatting = Formatting.Indented
            sdmxRequest.LoadXml(sdmxResponse.OuterXml.ToString)
            sdmxRequest.Save(xmlWriter)
            txtResponse.Text = textWriter.ToString()
            '--------------------------------------
            myXmlNamespaceManager = New System.Xml.XmlNamespaceManager(sdmxResponse.NameTable)
            myXmlNamespaceManager.AddNamespace("structure", "http://www.SDMX.org/resources/SDMXML/schemas/v2_0/structure")

            entryList = sdmxResponse.SelectNodes("descendant::structure:ConceptScheme", myXmlNamespaceManager)
            If entryList.Count > 0 Then
                dt = mySdmxStructureFile.readAllConceptSchemes(sdmxResponse, myXmlNamespaceManager, ddlLang.SelectedValue)
                grdCodeLists.DataSource = dt
                grdCodeLists.DataBind()
            End If
            ViewState("artefact") = "cs"
        Catch ex As Exception
            lblError.Text = ex.Message
        Finally
            If IsNothing(StreamTxt) = False Then
                StreamTxt.Close()
                StreamTxt = Nothing
            End If
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        artefact = ViewState("artefact")
    End Sub
End Class
