Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.io
Imports System.Xml
Imports System.Xml.XPath
Namespace sdmxIstat
    Public Class sdmxStructureFile
        Dim entryList As XmlNodeList
        Dim codes As New Codes
        Dim codeLists As New CodeLists
        Dim descriptors As New Descriptors
        Dim components As New Components
        Dim keyFamilies As New KeyFamilies
        Dim dataflow As New DataFlows
        
        Public Function readAllCodeLists(ByVal doc As XmlDocument, ByVal myXmlnamespacemanager As XmlNamespaceManager, ByVal lang As String) As DataTable
            Dim dt As New DataTable
            Dim myDataRow As DataRow = Nothing
            Try
                entryList = doc.SelectNodes("descendant::structure:CodeList", myXmlnamespacemanager)
                dt.Columns.Add("artefact")
                dt.Columns.Add("id")
                dt.Columns.Add("name")
                dt.Columns.Add("agencyID")
                dt.Columns.Add("version")
                dt.Columns.Add("isFinal")
                'the current version of the web service does't response validFrom and validTo
                'dt.Columns.Add("validFrom")
                'dt.Columns.Add("validTo")
                For I As Integer = 0 To entryList.Count - 1
                    'SDMX_M_PERIODS, SDMX_Q_PERIODS, SDMX_A_PERIODS, SDMX_H_PERIODS are not considered
                    If entryList(I).Attributes("id").Value <> "SDMX_M_PERIODS" And entryList(I).Attributes("id").Value <> "SDMX_Q_PERIODS" And entryList(I).Attributes("id").Value <> "SDMX_A_PERIODS" And entryList(I).Attributes("id").Value <> "SDMX_H_PERIODS" Then
                        myDataRow = dt.NewRow()
                        myDataRow("artefact") = "cl"
                        myDataRow("id") = entryList(I).Attributes("id").Value
                        myDataRow("agencyID") = entryList(I).Attributes("agencyID").Value
                        myDataRow("version") = entryList(I).Attributes("version").Value
                        myDataRow("isFinal") = entryList(I).Attributes("isFinal").Value
                        'the current version of the web service does't handle validFrom and validTo
                        'myDataRow("validFrom") = entryList(I).Attributes("validFrom").Value
                        'myDataRow("validTo") = entryList(I).Attributes("validTo").Value
                        For Each x As XmlNode In entryList.Item(I)
                            Select Case x.Name
                                Case "structure:Name"
                                    If IsNothing(x.Attributes("xml:lang")) = False Then
                                        If x.Attributes("xml:lang").Value = lang Then
                                            myDataRow("name") = x.InnerText
                                        End If
                                    End If
                                    'Case "structure:Code"
                                    '    codes.pk_code = x.Attributes("value").Value
                                    '    codes.Description = x.ChildNodes(0).InnerText
                                    '    If Not IsNothing(x.Attributes("parentCode")) = True Then
                                    '        codes.fk_parent_code = x.Attributes("parentCode").Value
                                    '    Else
                                    '        codes.fk_parent_code = ""
                                    '    End If
                            End Select
                        Next
                        dt.Rows.Add(myDataRow)
                    End If
                Next
                Return dt
            Catch ex As Exception
                Throw New Exception("Error in sdmxStructureFile.codeListsLoad: " & ex.Message)
            End Try
        End Function
        Public Function readAllConceptSchemes(ByVal doc As XmlDocument, ByVal myXmlnamespacemanager As XmlNamespaceManager, ByVal lang As String) As DataTable
            Dim dt As New DataTable
            Dim myDataRow As DataRow = Nothing
            Try
                entryList = doc.SelectNodes("descendant::structure:ConceptScheme", myXmlnamespacemanager)
                dt.Columns.Add("artefact")
                dt.Columns.Add("id")
                dt.Columns.Add("name")
                dt.Columns.Add("agencyID")
                dt.Columns.Add("version")
                dt.Columns.Add("isFinal")
                'the current version of the web service does't response validFrom and validTo
                'dt.Columns.Add("validFrom")
                'dt.Columns.Add("validTo")
                For I As Integer = 0 To entryList.Count - 1
                    myDataRow = dt.NewRow()
                    myDataRow("artefact") = "cs"
                    myDataRow("id") = entryList(I).Attributes("id").Value
                    myDataRow("agencyID") = entryList(I).Attributes("agencyID").Value
                    myDataRow("version") = entryList(I).Attributes("version").Value
                    myDataRow("isFinal") = entryList(I).Attributes("isFinal").Value
                    'the current version of the web service does't handle validFrom and validTo
                    'myDataRow("validFrom") = entryList(I).Attributes("validFrom").Value
                    'myDataRow("validTo") = entryList(I).Attributes("validTo").Value
                    For Each x As XmlNode In entryList.Item(I)
                        Select Case x.Name
                            Case "structure:Name"
                                If IsNothing(x.Attributes("xml:lang")) = False Then
                                    If x.Attributes("xml:lang").Value = lang Then
                                        myDataRow("name") = x.InnerText
                                    End If
                                End If
                                'Case "structure:Code"
                                '    codes.pk_code = x.Attributes("value").Value
                                '    codes.Description = x.ChildNodes(0).InnerText
                                '    If Not IsNothing(x.Attributes("parentCode")) = True Then
                                '        codes.fk_parent_code = x.Attributes("parentCode").Value
                                '    Else
                                '        codes.fk_parent_code = ""
                                '    End If
                        End Select
                    Next
                    dt.Rows.Add(myDataRow)
                Next
                Return dt
            Catch ex As Exception
                Throw New Exception("Error in sdmxStructureFile.codeListsLoad: " & ex.Message)
            End Try
        End Function
        Public Function readCodeList(ByVal doc As XmlDocument, ByVal myXmlnamespacemanager As XmlNamespaceManager, ByVal idCL As String) As DataTable
            Dim dt As DataTable = Nothing
            Dim myDataRow As DataRow = Nothing
            Try
                entryList = doc.SelectNodes("descendant::structure:CodeList", myXmlnamespacemanager)
                For I As Integer = 0 To entryList.Count - 1
                    'SDMX_M_PERIODS, SDMX_Q_PERIODS, SDMX_A_PERIODS, SDMX_H_PERIODS are not considered
                    If entryList(I).Attributes("id").Value <> "SDMX_M_PERIODS" And entryList(I).Attributes("id").Value <> "SDMX_Q_PERIODS" And entryList(I).Attributes("id").Value <> "SDMX_A_PERIODS" And entryList(I).Attributes("id").Value <> "SDMX_H_PERIODS" Then
                        codes.fk_code_list = entryList(I).Attributes("id").Value
                        codeLists.pk_code_list = entryList(I).Attributes("id").Value
                        codeLists.Agency = entryList(I).Attributes("agencyID").Value
                        For Each x As XmlNode In entryList.Item(I)
                            Select Case x.Name
                                Case "structure:Name"
                                    codeLists.name = x.InnerText
                                Case "structure:Code"
                                    codes.pk_code = x.Attributes("value").Value
                                    codes.Description = x.ChildNodes(0).InnerText
                                    If Not IsNothing(x.Attributes("parentCode")) = True Then
                                        codes.fk_parent_code = x.Attributes("parentCode").Value
                                    Else
                                        codes.fk_parent_code = ""
                                    End If
                            End Select
                        Next
                    End If
                Next
                Return dt
            Catch ex As Exception
                Throw New Exception("Error in sdmxStructureFile.codeListsLoad: " & ex.Message)
            End Try
        End Function
        'Sub DataFlowLoad(ByVal conn As OleDbConnection, ByVal doc As XmlDocument, ByVal myXmlnamespacemanager As XmlNamespaceManager)
        '    'CASE OF LOADING OF DATAFLOW BY THE STRUCTURE FILE
        '    Try
        '        entryList = doc.SelectNodes("descendant::structure:DataFlow", myXmlnamespacemanager)
        '        For I As Integer = 0 To entryList.Count - 1
        '            dataflow.pk_dataFlow = entryList(I).Attributes("id").Value
        '            MDIdefault.ToolStripStatusLabel.Text = "Importing " & dataflow.pk_dataFlow & ". Please wait ...."
        '            MDIdefault.StatusStrip.Refresh()
        '            For Each x As XmlNode In entryList.Item(I)
        '                Select Case x.Name
        '                    Case "structure:Description"
        '                        dataflow.Description = x.InnerText
        '                    Case "structure:KeyFamilyRef"
        '                        dataflow.fk_keyFamily = x.InnerText
        '                End Select
        '            Next
        '            manageDSDTables.loadDataFlows(conn, dataflow)
        '        Next
        '    Catch ex As Exception
        '        Throw New Exception("Error in sdmxStructureFile.DataFlowLoad: " & ex.Message)
        '    End Try
        'End Sub
        'Public Function cercaKeyFamily(ByVal pathXML As String) As KeyFamilies
        '    Dim doc As New XmlDocument
        '    Dim strKF As String = ""
        '    Dim xmlNameTable As New System.Xml.NameTable
        '    Dim myXmlNamespaceManager As System.Xml.XmlNamespaceManager
        '    Try
        '        doc.Load(pathXML)
        '        myXmlNamespaceManager = New System.Xml.XmlNamespaceManager(doc.NameTable)
        '        myXmlNamespaceManager.AddNamespace("common", "http://www.SDMX.org/resources/SDMXML/schemas/v2_0/common")
        '        myXmlNamespaceManager.AddNamespace("structure", "http://www.SDMX.org/resources/SDMXML/schemas/v2_0/structure")
        '        myXmlNamespaceManager.AddNamespace("xsi:schemaLocation", "http://www.SDMX.org/resources/SDMXML/schemas/v2_0/message SDMXMessage.xsd")
        '        entryList = doc.SelectNodes("descendant::structure:KeyFamily", myXmlNamespaceManager)
        '        If entryList.Count <> 0 Then
        '            strKF = entryList(0).Attributes("id").Value
        '            Dim mykeyfamily As KeyFamilies
        '            mykeyfamily = New KeyFamilies
        '            mykeyfamily.key_family = entryList(0).Attributes("id").Value
        '            mykeyfamily.agencyid = entryList(0).Attributes("agencyID").Value
        '            mykeyfamily.Version = entryList(0).Attributes("version").Value
        '            mykeyfamily.isFinal = entryList(0).Attributes("isFinal").Value
        '            mykeyfamily.name = entryList(0).ChildNodes(0).InnerText
        '            entryList = doc.SelectNodes("descendant::structure:CrossSectionalMeasure", myXmlNamespaceManager)
        '            If entryList.Count > 0 Then
        '                mykeyfamily.Flag_CS = "1"
        '            Else
        '                mykeyfamily.Flag_CS = "0"
        '            End If
        '            Return mykeyfamily
        '        Else
        '            Dim mykeyfamily As KeyFamilies
        '            mykeyfamily = New KeyFamilies
        '            Return mykeyfamily
        '        End If
        '    Catch ex As Exception
        '        Throw New Exception("Error in sdmxStructureFile.cercaKeyFamily: " & ex.Message)
        '    End Try
        'End Function
        'Public Sub keyFamiliesLoad(ByVal conn As OleDbConnection, ByVal doc As XmlDocument, ByRef myKey_family As KeyFamilies, ByVal myXmlnamespacemanager As XmlNamespaceManager, ByRef descriptorsAL As ArrayList, ByRef componentsAL As ArrayList)
        '    Try
        '        Dim entryListGroup As XmlNodeList = Nothing
        '        Dim xG As XmlNode = Nothing

        '        '---DIMENSION
        '        Dim components As Components
        '        Dim descriptors As Descriptors
        '        Dim jD As Integer = 0
        '        Dim jA As Integer = 0
        '        Dim jX As Integer = 0
        '        Dim jP As Integer = 0
        '        Dim jG As Integer = 0
        '        Dim blFlag_cs As Boolean = False
        '        Dim blFlag_xcs As Boolean = False
        '        entryList = doc.SelectNodes("descendant::structure:Components", myXmlnamespacemanager)
        '        For Each x As XmlNode In entryList(0).ChildNodes
        '            If x.NodeType = XmlNodeType.Comment Then
        '                'salta il nodo
        '            Else
        '                components = New Components
        '                descriptors = New Descriptors
        '                components.fk_key_family = myKey_family.key_family
        '                descriptors.fk_key_family = myKey_family.key_family
        '                blFlag_cs = False
        '                If Not IsNothing(x.Attributes("conceptRef")) = True Then
        '                    components.pk_component = x.Attributes("conceptRef").Value
        '                    descriptors.fk_component = x.Attributes("conceptRef").Value
        '                End If
        '                If Not IsNothing(x.Attributes("crossSectionalAttachDataSet")) = True Then
        '                    blFlag_cs = True
        '                    If x.Attributes("crossSectionalAttachDataSet").Value = "true" Then
        '                        components.CrossSectionalAttachDataset = "1"
        '                    Else
        '                        components.CrossSectionalAttachDataset = "0"
        '                    End If
        '                End If
        '                If Not IsNothing(x.Attributes("crossSectionalAttachGroup")) = True Then
        '                    blFlag_cs = True
        '                    If x.Attributes("crossSectionalAttachGroup").Value = "true" Then
        '                        components.CrossSectionalAttachGroup = "1"
        '                    Else
        '                        components.CrossSectionalAttachGroup = "0"
        '                    End If
        '                End If
        '                If Not IsNothing(x.Attributes("crossSectionalAttachSection")) = True Then
        '                    blFlag_cs = True
        '                    If x.Attributes("crossSectionalAttachSection").Value = "true" Then
        '                        components.CrossSectionalAttachSection = "1"
        '                    Else
        '                        components.CrossSectionalAttachSection = "0"
        '                    End If
        '                End If
        '                If Not IsNothing(x.Attributes("crossSectionalAttachObservation")) = True Then
        '                    blFlag_cs = True
        '                    If x.Attributes("crossSectionalAttachObservation").Value = "true" Then
        '                        components.CrossSectionalAttachObs = "1"
        '                    Else
        '                        components.CrossSectionalAttachObs = "0"
        '                    End If
        '                End If
        '                Select Case x.Name
        '                    Case "structure:Dimension"
        '                        jD = jD + 1
        '                        If Not IsNothing(x.Attributes("isMeasureDimension")) = True Then
        '                            components.component_type = "MD"
        '                        Else
        '                            components.component_type = "D"
        '                        End If


        '                        components.usage_status = "M"
        '                        components.order = jD
        '                        If Not IsNothing(x.Attributes("codelist")) = True Then
        '                            components.fk_codelist = x.Attributes("codelist").Value
        '                        End If
        '                        descriptors.descriptor_type = "K"
        '                        descriptors.pk_descriptor = myKey_family.key_family & "_K"
        '                        descriptors.component_descriptor_type = "DimensionRef"
        '                    Case "structure:TimeDimension"
        '                        jD = jD + 1
        '                        components.component_type = "TD"
        '                        components.usage_status = "M"
        '                        components.order = jD
        '                        descriptors.descriptor_type = "K"
        '                        descriptors.pk_descriptor = myKey_family.key_family & "_K"
        '                        descriptors.component_descriptor_type = "TimeDimension"
        '                    Case "structure:Attribute"
        '                        jA = jA + 1
        '                        components.component_type = "A"
        '                        'If Not IsNothing(x.Attributes("attachmentLevel")) = True And blFlag_cs = False Then comme da laura agg riga sotto
        '                        If Not IsNothing(x.Attributes("attachmentLevel")) = True Then
        '                            If x.Attributes("attachmentLevel").Value = "DataSet" Then
        '                                components.attachment_level = "D"
        '                            ElseIf x.Attributes("attachmentLevel").Value = "Group" Then
        '                                components.attachment_level = "G"
        '                            ElseIf x.Attributes("attachmentLevel").Value = "Series" Then
        '                                components.attachment_level = "S"
        '                            ElseIf x.Attributes("attachmentLevel").Value = "Observation" Then
        '                                components.attachment_level = "O"
        '                            End If
        '                        End If
        '                        If Not IsNothing(x.Attributes("assignmentStatus")) = True Then
        '                            If x.Attributes("assignmentStatus").Value = "Mandatory" Then
        '                                components.usage_status = "M"
        '                            ElseIf x.Attributes("assignmentStatus").Value = "Optional" Then
        '                                components.usage_status = "O"
        '                            ElseIf x.Attributes("assignmentStatus").Value = "Conditional" Then
        '                                components.usage_status = "C"
        '                            End If
        '                        End If
        '                        If Not IsNothing(x.Attributes("codelist")) = True Then
        '                            components.fk_codelist = x.Attributes("codelist").Value
        '                        End If
        '                        descriptors.descriptor_type = "A"
        '                        descriptors.pk_descriptor = myKey_family.key_family & "_A"
        '                        descriptors.component_descriptor_type = "Attribute"
        '                    Case "structure:CrossSectionalMeasure"
        '                        blFlag_xcs = True
        '                        jX = jX + 1
        '                        components.component_type = "X"
        '                        components.usage_status = "M"
        '                        descriptors.descriptor_type = "M"
        '                        descriptors.pk_descriptor = myKey_family.key_family & "_M"
        '                        descriptors.component_descriptor_type = "AttachmentMeasure"
        '                    Case "structure:PrimaryMeasure"
        '                        jP = jP + 1
        '                        components.component_type = "M"
        '                        components.usage_status = "M"
        '                        descriptors.descriptor_type = "M"
        '                        descriptors.pk_descriptor = myKey_family.key_family & "_M"
        '                        descriptors.component_descriptor_type = "PrimaryMeasure"
        '                End Select
        '                If x.Name = "structure:Group" Then
        '                    If Not IsNothing(x.Attributes("id")) = True Then
        '                        descriptors.group_id = x.Attributes("id").Value
        '                    End If
        '                    For Each xG In x.ChildNodes
        '                        jG = jG + 1
        '                        If jG > 1 Then
        '                            descriptors = New Descriptors
        '                            descriptors.fk_key_family = myKey_family.key_family
        '                        End If
        '                        descriptors.fk_component = xG.InnerText
        '                        descriptors.descriptor_type = "G"
        '                        descriptors.pk_descriptor = myKey_family.key_family & "_G"
        '                        descriptors.component_descriptor_type = "DimensionRef"
        '                        descriptorsAL.Add(descriptors)
        '                    Next
        '                Else
        '                    descriptorsAL.Add(descriptors)
        '                    componentsAL.Add(components)
        '                End If
        '                If myKey_family.Flag_CS = "0" Or myKey_family.Flag_CS = "" Then
        '                    If blFlag_cs = True And blFlag_xcs = False Then
        '                        myKey_family.Flag_CS = "2" 'crosssectional without crosssectional_measure
        '                    ElseIf blFlag_xcs = True Then
        '                        myKey_family.Flag_CS = "1"
        '                    End If

        '                End If
        '                MDIdefault.ToolStripStatusLabel.Text = "Importing " & components.pk_component & " - " & components.component_type & ". Please wait ...."
        '                MDIdefault.StatusStrip.Refresh()
        '            End If
        '        Next

        '    Catch ex As Exception
        '        Throw New Exception("Error in sdmxStructureFile.keyFamiliesLoad " & ex.Message)
        '    End Try
        'End Sub

    End Class
End Namespace
