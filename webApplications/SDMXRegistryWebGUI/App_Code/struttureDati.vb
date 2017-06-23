Namespace sdmxIstat
    Public Class Codes
        Public pk_code As String
        Public Description As String
        Public fk_key_family As String
        Public fk_code_list As String
        Public fk_parent_code As String
    End Class
    Public Class CodeLists
        Public pk_code_list As String
        Public name As String
        Public fk_key_family As String
        Public Agency As String
    End Class
    Public Class DataFlows
        Public pk_dataFlow As String
        Public Description As String
        Public ref_period_begin As String
        Public ref_period_end As String
        Public fk_keyFamily As String
    End Class
    Public Class KeyFamilies
        Public key_family As String
        Public name As String
        Public agencyid As String
        Public Version As String
        Public isFinal As String
        Public Flag_CS As String 'indica se la key family e' cross sectional o no
    End Class
    Public Class Descriptors
        Public pk_descriptor As String = ""
        Public fk_key_family As String = ""
        Public fk_component As String = ""
        Public component_descriptor_type As String = ""
        Public descriptor_type As String = ""
        Public group_id As String = ""
    End Class
    Public Class Components
        Public fk_key_family As String = ""
        Public pk_component As String = ""
        Public fk_codelist As String = ""
        Public component_type As String = ""
        Public order As String = ""
        Public usage_status As String = ""
        Public attachment_level As String = ""
        Public CrossSectionalAttachSection As String = ""
        Public CrossSectionalAttachGroup As String = ""
        Public CrossSectionalAttachDataset As String = ""
        Public CrossSectionalAttachObs As String = ""
    End Class
    Public Class StoreMeta
        Private Nome As String
        Private Lunghezza As String
        Private Chiave As String

        Public Sub New()
            Nome = ""
            Lunghezza = ""
            Chiave = ""
        End Sub

        Public Sub New(ByVal strNome As String, ByVal strLunghezza As String, ByVal strChiave As String)
            Nome = strNome
            Lunghezza = strLunghezza
            Chiave = strChiave
        End Sub
        Public Property ItemNome() As String
            Get
                Return Nome
            End Get
            Set(ByVal Value As String)
                Nome = Value
            End Set
        End Property
        Public Property ItemLunghezza() As String
            Get
                Return Lunghezza
            End Get
            Set(ByVal Value As String)
                Lunghezza = Value
            End Set
        End Property
        Public Property ItemChiave() As String
            Get
                Return Chiave
            End Get
            Set(ByVal Value As String)
                Chiave = Value
            End Set
        End Property
    End Class
    Public Class ObsCS
        Public FREQ As String
        Public COUNTRY As String
        Public REF_PERIOD As String
        Public TIME_FORMAT As String
        Public DECI As String
        Public UNIT As String
        Public UNIT_MULT As String
        Public DEMO As String
        Public SEX As String
        Public OBS_VALUE As String
        Public OBS_STATUS As String
    End Class
 

End Namespace

